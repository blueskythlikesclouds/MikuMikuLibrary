//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Misc;

namespace MikuMikuLibrary.IO.Common
{
    public class EndianBinaryWriter : BinaryWriter
    {
        private Endianness mEndianness;
        private Encoding mEncoding;
        private Stack<long> mOffsets;
        private Stack<long> mBaseOffsets;
        private List<ScheduledWrite> mScheduledWrites;
        private Stack<StringTable> mStringTables;
        private List<long> mOffsetPositions;

        public Endianness Endianness
        {
            get => mEndianness;
            set
            {
                SwapBytes = value != EndiannessHelper.SystemEndianness;
                mEndianness = value;
            }
        }

        public AddressSpace AddressSpace { get; set; }

        public bool SwapBytes { get; private set; }

        public long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public long Length => BaseStream.Length;

        public IReadOnlyList<long> OffsetPositions => mOffsetPositions;

        public long BaseOffset
        {
            get => mBaseOffsets.Count > 0 ? mBaseOffsets.Peek() : 0;
            set
            {
                if ( mBaseOffsets.Count > 0 && mBaseOffsets.Peek() != value || mBaseOffsets.Count == 0 )
                    mBaseOffsets.Push( value );
            }
        }

        public void Seek( long offset, SeekOrigin origin ) => 
            BaseStream.Seek( offset, origin );

        public void SeekBegin( long offset ) => 
            BaseStream.Seek( offset, SeekOrigin.Begin );

        public void SeekCurrent( long offset ) => 
            BaseStream.Seek( offset, SeekOrigin.Current );

        public void SeekEnd( long offset ) => 
            BaseStream.Seek( offset, SeekOrigin.End );

        public void PushOffset( long offset ) => 
            mOffsets.Push( offset );

        public void PushOffset() => 
            mOffsets.Push( Position );

        public long PeekOffset() => 
            mOffsets.Peek();

        public long PopOffset() => 
            mOffsets.Pop();

        public long SeekBeginToPoppedOffset() => 
            BaseStream.Seek( mOffsets.Pop(), SeekOrigin.Begin );

        public void PushBaseOffset( long offset ) => 
            mBaseOffsets.Push( offset );

        public void PushBaseOffset() => 
            mBaseOffsets.Push( Position );

        public long PeekBaseOffset() => 
            mBaseOffsets.Peek();

        public long PopBaseOffset() => 
            mBaseOffsets.Pop();

        public void Align( int alignment, byte filler = 0 )
        {
            int difference = AlignmentHelper.GetAlignedDifference( Position, alignment );

            int fourByte = ( filler << 24 ) | ( filler << 16 ) | ( filler << 8 ) | filler;

            for ( int i = 0; i < difference / 4; i++ )
                Write( fourByte );

            for ( int i = 0; i < difference % 4; i++ )
                Write( filler );
        }

        public void WriteNulls( int count )
        {
            for ( int i = 0; i < count / 4; i++ )
                Write( 0 );

            for ( int i = 0; i < count % 4; i++ )
                Write( ( byte ) 0 );
        }

        public void WriteOffset( long offset )
        {
            if ( AddressSpace == AddressSpace.Int32 )
                Write( ( uint ) offset );

            if ( AddressSpace == AddressSpace.Int64 )
            {
                Align( 8 );
                Write( offset );
            }
        }

        public void WriteAtOffset( long offset, Action action )
        {
            long current = Position;

            SeekBegin( offset );
            {
                action();
            }
            SeekBegin( current );
        }

        public void ScheduleWriteOffset( Func<long> action ) => 
            ScheduleWriteOffset( 0, 0, AlignmentMode.None, OffsetMode.Offset, action );

        public void ScheduleWriteOffset( Action action ) => 
            ScheduleWriteOffset( 0, 0, AlignmentMode.None, OffsetMode.Offset, action );

        public void ScheduleWriteOffset( OffsetMode offsetMode, Func<long> action ) => 
            ScheduleWriteOffset( 0, 0, AlignmentMode.None, offsetMode, action );

        public void ScheduleWriteOffset( OffsetMode offsetMode, Action action ) => 
            ScheduleWriteOffset( 0, 0, AlignmentMode.None, offsetMode, action );

        public void ScheduleWriteOffset( int alignment, AlignmentMode alignmentMode, Func<long> action ) => 
            ScheduleWriteOffset( 0, alignment, alignmentMode, OffsetMode.Offset, action );

        public void ScheduleWriteOffset( int alignment, AlignmentMode alignmentMode, Action action ) => 
            ScheduleWriteOffset( 0, alignment, alignmentMode, OffsetMode.Offset, action );

        public void ScheduleWriteOffset( int alignment, AlignmentMode alignmentMode, OffsetMode offsetMode, Func<long> action ) => 
            ScheduleWriteOffset( 0, alignment, alignmentMode, offsetMode, action );

        public void ScheduleWriteOffset( int alignment, AlignmentMode alignmentMode, OffsetMode offsetMode, Action action ) => 
            ScheduleWriteOffset( 0, alignment, alignmentMode, offsetMode, action );

        public void ScheduleWriteOffset( int priority, Func<long> action ) => 
            ScheduleWriteOffset( priority, 0, AlignmentMode.None, OffsetMode.Offset, action );

        public void ScheduleWriteOffset( int priority, Action action ) => 
            ScheduleWriteOffset( priority, 0, AlignmentMode.None, OffsetMode.Offset, action );

        public void ScheduleWriteOffset( int priority, OffsetMode offsetMode, Func<long> action ) => 
            ScheduleWriteOffset( priority, 0, AlignmentMode.None, offsetMode, action );

        public void ScheduleWriteOffset( int priority, OffsetMode offsetMode, Action action ) => 
            ScheduleWriteOffset( priority, 0, AlignmentMode.None, offsetMode, action );

        public void ScheduleWriteOffset( int priority, int alignment, AlignmentMode alignmentMode, Func<long> action ) => 
            ScheduleWriteOffset( priority, alignment, alignmentMode, OffsetMode.Offset, action );

        public void ScheduleWriteOffset( int priority, int alignment, AlignmentMode alignmentMode, Action action ) => 
            ScheduleWriteOffset( priority, alignment, alignmentMode, OffsetMode.Offset, action );

        public void ScheduleWriteOffset( int priority, int alignment, AlignmentMode alignmentMode, OffsetMode offsetMode, Func<long> action )
        {
            mScheduledWrites.Add( new ScheduledWrite
            {
                FieldOffset = PrepareWriteOffset( offsetMode ),
                Priority = priority,
                Alignment = alignment,
                AlignmentMode = alignmentMode,
                OffsetMode = offsetMode,
                Action = action,
                BaseOffset = BaseOffset
            } );
        }

        public void ScheduleWriteOffset( int priority, int alignment, AlignmentMode alignmentMode, OffsetMode offsetMode, Action action )
        {
            ScheduleWriteOffset( priority, alignment, alignmentMode, offsetMode, () =>
            {
                long current = Position;
                action();
                return current;
            } );
        }

        public void ScheduleWriteOffsetIf( bool condition, Func<long> action ) => 
            ScheduleWriteOffsetIf( condition, 0, 0, AlignmentMode.None, OffsetMode.Offset, action );

        public void ScheduleWriteOffsetIf( bool condition, Action action ) => 
            ScheduleWriteOffsetIf( condition, 0, 0, AlignmentMode.None, OffsetMode.Offset, action );

        public void ScheduleWriteOffsetIf( bool condition, OffsetMode offsetMode, Func<long> action ) => 
            ScheduleWriteOffsetIf( condition, 0, 0, AlignmentMode.None, offsetMode, action );

        public void ScheduleWriteOffsetIf( bool condition, OffsetMode offsetMode, Action action ) => 
            ScheduleWriteOffsetIf( condition, 0, 0, AlignmentMode.None, offsetMode, action );

        public void ScheduleWriteOffsetIf( bool condition, int alignment, AlignmentMode alignmentMode, Func<long> action ) => 
            ScheduleWriteOffsetIf( condition, 0, alignment, alignmentMode, OffsetMode.Offset, action );

        public void ScheduleWriteOffsetIf( bool condition, int alignment, AlignmentMode alignmentMode, Action action ) => 
            ScheduleWriteOffsetIf( condition, 0, alignment, alignmentMode, OffsetMode.Offset, action );

        public void ScheduleWriteOffsetIf( bool condition, int alignment, AlignmentMode alignmentMode, OffsetMode offsetMode, Func<long> action ) => 
            ScheduleWriteOffsetIf( condition, 0, alignment, alignmentMode, offsetMode, action );

        public void ScheduleWriteOffsetIf( bool condition, int alignment, AlignmentMode alignmentMode, OffsetMode offsetMode, Action action ) => 
            ScheduleWriteOffsetIf( condition, 0, alignment, alignmentMode, offsetMode, action );

        public void ScheduleWriteOffsetIf( bool condition, int priority, Func<long> action ) => 
            ScheduleWriteOffsetIf( condition, priority, 0, AlignmentMode.None, OffsetMode.Offset, action );

        public void ScheduleWriteOffsetIf( bool condition, int priority, Action action ) => 
            ScheduleWriteOffsetIf( condition, priority, 0, AlignmentMode.None, OffsetMode.Offset, action );

        public void ScheduleWriteOffsetIf( bool condition, int priority, OffsetMode offsetMode, Func<long> action ) => 
            ScheduleWriteOffsetIf( condition, priority, 0, AlignmentMode.None, offsetMode, action );

        public void ScheduleWriteOffsetIf( bool condition, int priority, OffsetMode offsetMode, Action action ) => 
            ScheduleWriteOffsetIf( condition, priority, 0, AlignmentMode.None, offsetMode, action );

        public void ScheduleWriteOffsetIf( bool condition, int priority, int alignment, AlignmentMode alignmentMode, Func<long> action ) => 
            ScheduleWriteOffsetIf( condition, priority, alignment, alignmentMode, OffsetMode.Offset, action );

        public void ScheduleWriteOffsetIf( bool condition, int priority, int alignment, AlignmentMode alignmentMode, Action action ) => 
            ScheduleWriteOffsetIf( condition, priority, alignment, alignmentMode, OffsetMode.Offset, action );

        public void ScheduleWriteOffsetIf( bool condition, int priority, int alignment, AlignmentMode alignmentMode, OffsetMode offsetMode, Func<long> action )
        {
            if ( condition )
                ScheduleWriteOffset( priority, alignment, alignmentMode, offsetMode, action );
            else
                PrepareWriteOffset( offsetMode );
        }

        public void ScheduleWriteOffsetIf( bool condition, int priority, int alignment, AlignmentMode alignmentMode, OffsetMode offsetMode, Action action )
        {
            if ( condition )
                ScheduleWriteOffset( priority, alignment, alignmentMode, offsetMode, action );
            else
                PrepareWriteOffset( offsetMode );
        }

        private long PrepareWriteOffset( OffsetMode offsetMode )
        {
            long offset;

            if ( AddressSpace == AddressSpace.Int64 )
            {
                Align( 8 );
                offset = Position;
                Write( 0L );
            }

            else
            {
                offset = Position;
                Write( 0 );
            }


            if ( offsetMode != OffsetMode.OffsetAndSize && offsetMode != OffsetMode.SizeAndOffset )
                return offset;

            if ( AddressSpace == AddressSpace.Int64 )
                Write( 0L );
            else
                Write( 0 );

            return offset;
        }

        private void PerformScheduledWrites( int first, int last, long baseOffset )
        {
            int priority = 0;
            bool increment;

            do
            {
                increment = false;

                for ( int current = first; current != mScheduledWrites.Count && current != last + 1; current++ )
                {
                    var scheduledWrite = mScheduledWrites[ current ];

                    if ( scheduledWrite.Priority == priority )
                        PerformScheduledWrite( scheduledWrite, baseOffset );

                    else if ( scheduledWrite.Priority > priority )
                        increment = true;
                }

                priority++;
            } while ( increment );
        }

        private void PerformScheduledWrite( ScheduledWrite scheduledWrite, long baseOffset )
        {
            if ( scheduledWrite.AlignmentMode == AlignmentMode.Left || scheduledWrite.AlignmentMode == AlignmentMode.Center )
                Align( scheduledWrite.Alignment );

            int first = mScheduledWrites.Count - 1;

            long startOffset = scheduledWrite.Action();
            long endOffset = Position;

            int last = mScheduledWrites.Count - 1;

            if ( scheduledWrite.AlignmentMode == AlignmentMode.Right || scheduledWrite.AlignmentMode == AlignmentMode.Center )
                Align( scheduledWrite.Alignment );

            if ( scheduledWrite.BaseOffset > 0 )
                baseOffset = scheduledWrite.BaseOffset;

            PerformScheduledWrites( first + 1, last, baseOffset );

            mOffsetPositions.Add( scheduledWrite.FieldOffset );

            WriteAtOffset( scheduledWrite.FieldOffset, () =>
            {
                switch ( scheduledWrite.OffsetMode )
                {
                    case OffsetMode.Offset:
                        WriteOffset( startOffset - baseOffset );
                        break;
                    case OffsetMode.Size:
                        WriteOffset( endOffset - startOffset );
                        break;
                    case OffsetMode.OffsetAndSize:
                        WriteOffset( startOffset - baseOffset );
                        WriteOffset( endOffset - startOffset );
                        break;
                    case OffsetMode.SizeAndOffset:
                        WriteOffset( endOffset - startOffset );
                        WriteOffset( startOffset - baseOffset );
                        break;
                    default:
                        throw new ArgumentException( nameof( scheduledWrite.OffsetMode ) );
                }
            } );
        }

        public void PerformScheduledWrites()
        {
            if ( mScheduledWrites.Count == 0 )
                return;

            PerformScheduledWrites( 0, mScheduledWrites.Count - 1, 0 );

            mScheduledWrites.Clear();
        }

        public void PerformScheduledWritesReversed()
        {
            mScheduledWrites.Reverse();
            PerformScheduledWrites();
        }

        public void PushStringTable( StringBinaryFormat format, int fixedLength = -1 )
        {
            PushStringTable( 0, AlignmentMode.None, format, fixedLength );
        }

        public void PushStringTable( int alignment, AlignmentMode alignmentMode, StringBinaryFormat format, int fixedLength = -1 )
        {
            mStringTables.Push( new StringTable
            {
                BaseOffset = BaseOffset,
                Strings = new Dictionary<string, List<long>>(),
                AlignmentMode = alignmentMode,
                Alignment = alignment,
                Format = format,
                FixedLength = fixedLength
            } );
        }

        private void WriteStringTable( StringTable stringTable )
        {
            if ( stringTable.Strings.Count == 0 )
                return;

            if ( stringTable.AlignmentMode == AlignmentMode.Left || stringTable.AlignmentMode == AlignmentMode.Center )
                Align( stringTable.Alignment );

            foreach ( var keyValuePair in stringTable.Strings.OrderBy( x => x.Value[ 0 ] ) )
            {
                long stringOffset = Position;
                Write( keyValuePair.Key, stringTable.Format, stringTable.FixedLength );
                long endOffset = Position;

                foreach ( long offset in keyValuePair.Value )
                {
                    SeekBegin( offset );
                    WriteOffset( stringOffset - stringTable.BaseOffset );
                    mOffsetPositions.Add( offset );
                }

                SeekBegin( endOffset );
            }

            if ( stringTable.AlignmentMode == AlignmentMode.Right || stringTable.AlignmentMode == AlignmentMode.Center )
                Align( stringTable.Alignment );
        }

        public void PopStringTable() => 
            WriteStringTable( mStringTables.Pop() );

        public void PopStringTables()
        {
            while ( mStringTables.Count > 0 )
                PopStringTable();
        }

        public void PopStringTablesReversed()
        {
            if ( mStringTables.Count == 0 ) 
                return;

            if ( mStringTables.Count != 1 )
            {
                foreach ( var stringTable in mStringTables )
                    WriteStringTable( stringTable );

                mStringTables.Clear();
            }

            else
            {
                PopStringTable();
            }
        }

        public void AddStringToStringTable( string value )
        {
            var stringTable = mStringTables.Peek();

            long position = PrepareWriteOffset( OffsetMode.Offset );

            if ( value == null ) 
                return;

            if ( !stringTable.Strings.TryGetValue( value, out var positions ) )
            {
                var offsets = new List<long>();
                offsets.Add( position );
                stringTable.Strings.Add( value, offsets );
            }

            else
            {
                positions.Add( position );
            }
        }

        public void Write( sbyte[] values )
        {
            for ( int i = 0; i < values.Length; i++ )
                base.Write( values[ i ] );
        }

        public void Write( bool[] values )
        {
            for ( int i = 0; i < values.Length; i++ )
                Write( values[ i ] );
        }

        public override void Write( short value ) => 
            base.Write( SwapBytes ? EndiannessHelper.Swap( value ) : value );

        public void Write( short[] values )
        {
            foreach ( short value in values )
                Write( value );
        }

        public override void Write( ushort value ) => 
            base.Write( SwapBytes ? EndiannessHelper.Swap( value ) : value );

        public void Write( ushort[] values )
        {
            foreach ( ushort value in values )
                Write( value );
        }

        public override void Write( int value ) => 
            base.Write( SwapBytes ? EndiannessHelper.Swap( value ) : value );

        public void Write( int[] values )
        {
            foreach ( int value in values )
                Write( value );
        }

        public override void Write( uint value ) => 
            base.Write( SwapBytes ? EndiannessHelper.Swap( value ) : value );

        public void Write( uint[] values )
        {
            foreach ( uint value in values )
                Write( value );
        }

        public override void Write( long value ) => 
            base.Write( SwapBytes ? EndiannessHelper.Swap( value ) : value );

        public void Write( long[] values )
        {
            foreach ( long value in values )
                Write( value );
        }

        public override void Write( ulong value ) => 
            base.Write( SwapBytes ? EndiannessHelper.Swap( value ) : value );

        public void Write( ulong[] values )
        {
            foreach ( ulong value in values )
                Write( value );
        }

        public override void Write( float value ) => 
            base.Write( SwapBytes ? EndiannessHelper.Swap( value ) : value );

        public void Write( float[] values )
        {
            foreach ( float value in values )
                Write( value );
        }

        public void Write( Half value ) => 
            Write( value.Value );

        public void Write( Half[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( decimal value ) => 
            base.Write( SwapBytes ? EndiannessHelper.Swap( value ) : value );

        public void Write( decimal[] values )
        {
            foreach ( decimal value in values )
                Write( value );
        }

        public void Write( string value, StringBinaryFormat format, int fixedLength = -1 )
        {
            value = value ?? string.Empty;

            switch ( format )
            {
                case StringBinaryFormat.NullTerminated:
                {
                    Write( mEncoding.GetBytes( value ) );
                    base.Write( ( byte ) 0 );
                    break;
                }

                case StringBinaryFormat.FixedLength:
                {
                    if ( fixedLength == -1 )
                        throw new ArgumentException( "Fixed length must be provided if format is set to fixed length", nameof( fixedLength ) );

                    var bytes = mEncoding.GetBytes( value );

                    if ( bytes.Length > fixedLength )
                        throw new ArgumentException( "Provided string is longer than fixed length", nameof( value ) );

                    Write( bytes );

                    fixedLength -= bytes.Length;

                    while ( fixedLength-- > 0 )
                        base.Write( ( byte ) 0 );

                    break;
                }

                case StringBinaryFormat.PrefixedLength8:
                {
                    base.Write( ( byte ) value.Length );
                    Write( mEncoding.GetBytes( value ) );
                    break;
                }

                case StringBinaryFormat.PrefixedLength16:
                {
                    Write( ( ushort ) value.Length );
                    Write( mEncoding.GetBytes( value ) );
                    break;
                }

                case StringBinaryFormat.PrefixedLength32:
                {
                    Write( ( uint ) value.Length );
                    Write( mEncoding.GetBytes( value ) );
                    break;
                }

                default:
                    throw new ArgumentException( "Invalid format specified", nameof( format ) );
            }
        }

        public void Write( Vector2 value )
        {
            Write( value.X );
            Write( value.Y );
        }

        public void Write( Vector2 value, VectorBinaryFormat format )
        {
            switch ( format )
            {
                case VectorBinaryFormat.Single:
                    Write( value.X );
                    Write( value.Y );
                    break;

                case VectorBinaryFormat.Half:
                    Write( ( Half ) value.X );
                    Write( ( Half ) value.Y );
                    break;

                case VectorBinaryFormat.Int16:
                    Write( ( ushort ) BitHelper.QuantizeSnorm( value.X, 16 ) );
                    Write( ( ushort ) BitHelper.QuantizeSnorm( value.Y, 16 ) );
                    break;

                default:
                    throw new ArgumentException( nameof( format ) );
            }
        }

        public void Write( Vector2[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public void Write( Vector3 value )
        {
            Write( value.X );
            Write( value.Y );
            Write( value.Z );
        }

        public void Write( Vector3 value, VectorBinaryFormat format )
        {
            switch ( format )
            {
                case VectorBinaryFormat.Single:
                    Write( value.X );
                    Write( value.Y );
                    Write( value.Z );
                    break;

                case VectorBinaryFormat.Half:
                    Write( ( Half ) value.X );
                    Write( ( Half ) value.Y );
                    Write( ( Half ) value.Z );
                    break;

                case VectorBinaryFormat.Int16:
                    Write( ( ushort ) BitHelper.QuantizeSnorm( value.X, 16 ) );
                    Write( ( ushort ) BitHelper.QuantizeSnorm( value.Y, 16 ) );
                    Write( ( ushort ) BitHelper.QuantizeSnorm( value.Z, 16 ) );
                    break;

                default:
                    throw new ArgumentException( nameof( format ) );
            }
        }

        public void Write( Vector3[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public void Write( Vector4 value )
        {
            Write( value.X );
            Write( value.Y );
            Write( value.Z );
            Write( value.W );
        }

        public void Write( Vector4 value, VectorBinaryFormat format )
        {
            switch ( format )
            {
                case VectorBinaryFormat.Single:
                    Write( value.X );
                    Write( value.Y );
                    Write( value.Z );
                    Write( value.W );
                    break;

                case VectorBinaryFormat.Half:
                    Write( ( Half ) value.X );
                    Write( ( Half ) value.Y );
                    Write( ( Half ) value.Z );
                    Write( ( Half ) value.W );
                    break;

                case VectorBinaryFormat.Int16:
                    Write( ( ushort ) BitHelper.QuantizeSnorm( value.X, 16 ) );
                    Write( ( ushort ) BitHelper.QuantizeSnorm( value.Y, 16 ) );
                    Write( ( ushort ) BitHelper.QuantizeSnorm( value.Z, 16 ) );
                    Write( ( ushort ) BitHelper.QuantizeSnorm( value.W, 16 ) );
                    break;

                default:
                    throw new ArgumentException( nameof( format ) );
            }
        }

        public void Write( Vector4[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public void Write( Matrix4x4 value )
        {
            Write( value.M11 );
            Write( value.M21 );
            Write( value.M31 );
            Write( value.M41 );

            Write( value.M12 );
            Write( value.M22 );
            Write( value.M32 );
            Write( value.M42 );

            Write( value.M13 );
            Write( value.M23 );
            Write( value.M33 );
            Write( value.M43 );

            Write( value.M14 );
            Write( value.M24 );
            Write( value.M34 );
            Write( value.M44 );
        }

        public void Write( Matrix4x4[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public void Write( Color value )
        {
            Write( value.R );
            Write( value.G );
            Write( value.B );
            Write( value.A );
        }

        public void Write( Color value, VectorBinaryFormat format )
        {
            switch ( format )
            {
                case VectorBinaryFormat.Single:
                    Write( value.R );
                    Write( value.G );
                    Write( value.B );
                    Write( value.A );
                    break;

                case VectorBinaryFormat.Half:
                    Write( ( Half ) value.R );
                    Write( ( Half ) value.G );
                    Write( ( Half ) value.B );
                    Write( ( Half ) value.A );
                    break;

                case VectorBinaryFormat.UInt8:
                    Write( ( byte ) ( value.R * 255f ) );
                    Write( ( byte ) ( value.G * 255f ) );
                    Write( ( byte ) ( value.B * 255f ) );
                    Write( ( byte ) ( value.A * 255f ) );
                    break;

                case VectorBinaryFormat.Int16:
                    Write( ( ushort ) BitHelper.QuantizeSnorm( value.R, 16 ) );
                    Write( ( ushort ) BitHelper.QuantizeSnorm( value.G, 16 ) );
                    Write( ( ushort ) BitHelper.QuantizeSnorm( value.B, 16 ) );
                    Write( ( ushort ) BitHelper.QuantizeSnorm( value.A, 16 ) );
                    break;

                default:
                    throw new ArgumentException( nameof( format ) );
            }
        }

        public void Write( Color[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public void Write( BoundingSphere boundingSphere )
        {
            Write( boundingSphere.Center );
            Write( boundingSphere.Radius );
        }

        public void Write( BoundingBox boundingBox )
        {
            Write( boundingBox.Center );
            Write( boundingBox.Width );
            Write( boundingBox.Height );
            Write( boundingBox.Depth );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                PerformScheduledWrites();
                PopStringTablesReversed();
            }

            base.Dispose( disposing );
        }

        private void Init( Encoding encoding, Endianness endianness, AddressSpace addressSpace )
        {
            Endianness = endianness;
            AddressSpace = addressSpace;
            mEncoding = encoding;
            mOffsets = new Stack<long>();
            mBaseOffsets = new Stack<long>();
            mScheduledWrites = new List<ScheduledWrite>();
            mStringTables = new Stack<StringTable>();
            mOffsetPositions = new List<long>();
        }

        public EndianBinaryWriter( Stream input, Endianness endianness )
            : base( input )
        {
            Init( Encoding.Default, endianness, AddressSpace.Int32 );
        }

        public EndianBinaryWriter( Stream input, Endianness endianness, AddressSpace addressSpace )
            : base( input )
        {
            Init( Encoding.Default, endianness, addressSpace );
        }

        public EndianBinaryWriter( Stream input, Encoding encoding, Endianness endianness )
            : base( input, encoding )
        {
            Init( encoding, endianness, AddressSpace.Int32 );
        }

        public EndianBinaryWriter( Stream input, Encoding encoding, Endianness endianness, AddressSpace addressSpace )
            : base( input, encoding )
        {
            Init( encoding, endianness, addressSpace );
        }

        public EndianBinaryWriter( Stream input, Encoding encoding, Endianness endianness, bool leaveOpen )
            : base( input, encoding, leaveOpen )
        {
            Init( encoding, endianness, AddressSpace.Int32 );
        }

        public EndianBinaryWriter( Stream input, Encoding encoding, Endianness endianness, AddressSpace addressSpace, bool leaveOpen )
            : base( input, encoding, leaveOpen )
        {
            Init( encoding, endianness, addressSpace );
        }

        private class ScheduledWrite
        {
            public long BaseOffset;
            public long FieldOffset;
            public Func<long> Action;
            public int Priority;
            public OffsetMode OffsetMode;
            public AlignmentMode AlignmentMode;
            public int Alignment;
        }

        private class StringTable
        {
            public long BaseOffset;
            public Dictionary<string, List<long>> Strings;
            public AlignmentMode AlignmentMode;
            public int Alignment;
            public StringBinaryFormat Format;
            public int FixedLength;

            public StringTable() => 
                Strings = new Dictionary<string, List<long>>();
        }
    }

    public enum AlignmentMode
    {
        None,
        Left,
        Center,
        Right
    }

    public enum OffsetMode
    {
        Offset,
        Size,
        OffsetAndSize,
        SizeAndOffset
    }
}