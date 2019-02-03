//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using MikuMikuLibrary.Maths;
using MikuMikuLibrary.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MikuMikuLibrary.IO.Common
{
    public enum AlignmentMode
    {
        None,
        Left,
        Center,
        Right,
    };

    public enum OffsetMode
    {
        Offset,
        Size,
        OffsetAndSize,
        SizeAndOffset,
    };

    public class EndianBinaryWriter : BinaryWriter
    {
        private class ScheduledWrite
        {
            public long BaseOffset;
            public long FieldOffset;
            public Action Action;
            public OffsetMode OffsetMode;
            public AlignmentMode AlignmentMode;
            public int Alignment;
            public byte AlignmentFillerByte;
        }

        private class StringTable
        {
            public long BaseOffset;
            public Dictionary<string, List<long>> Strings;
            public AlignmentMode AlignmentMode;
            public int Alignment;
            public StringBinaryFormat Format;
            public int FixedLength;

            public StringTable()
            {
                Strings = new Dictionary<string, List<long>>();
            }
        }

        private Endianness mEndianness;
        private bool mSwap;
        private Encoding mEncoding;
        private Stack<long> mOffsets;
        private Stack<long> mBaseOffsets;
        private LinkedList<ScheduledWrite> mScheduledWrites;
        private Stack<StringTable> mStringTables;
        private List<long> mOffsetPositions;

        public Endianness Endianness
        {
            get => mEndianness;
            set
            {
                mSwap = value != EndiannessSwapUtilities.SystemEndianness;
                mEndianness = value;
            }
        }

        public AddressSpace AddressSpace { get; set; }

        public bool SwapBytes => mSwap;

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

        public void Seek( long offset, SeekOrigin origin ) => BaseStream.Seek( offset, origin );
        public void SeekBegin( long offset ) => BaseStream.Seek( offset, SeekOrigin.Begin );
        public void SeekCurrent( long offset ) => BaseStream.Seek( offset, SeekOrigin.Current );
        public void SeekEnd( long offset ) => BaseStream.Seek( offset, SeekOrigin.End );

        public void PushOffset( long offset ) => mOffsets.Push( offset );
        public void PushOffset() => mOffsets.Push( Position );
        public long PeekOffset() => mOffsets.Peek();
        public long PopOffset() => mOffsets.Pop();
        public long SeekBeginToPoppedOffset() => BaseStream.Seek( mOffsets.Pop(), SeekOrigin.Begin );

        public void PushBaseOffset( long offset ) => mBaseOffsets.Push( offset );
        public void PushBaseOffset() => mBaseOffsets.Push( Position );
        public long PeekBaseOffset() => mBaseOffsets.Peek();
        public long PopBaseOffset() => mBaseOffsets.Pop();

        public void WriteAlignmentPadding( int alignment, byte filler = 0 )
        {
            int difference = AlignmentUtilities.GetAlignedDifference( Position, alignment );

            int fourByte = filler << 24 | filler << 16 | filler << 8 | filler;

            for ( int i = 0; i < difference / 4; i++ )
                Write( fourByte );

            for ( int i = 0; i < difference % 4; i++ )
                Write( filler );
        }

        public void WriteNulls( int count )
        {
            for ( int i = 0; i < count / 4; i++ )
                Write( ( int )0 );

            for ( int i = 0; i < count % 4; i++ )
                Write( ( byte )0 );
        }

        public void WriteOffset( long offset )
        {
            if ( AddressSpace == AddressSpace.Int32 )
                Write( ( uint )offset );

            if ( AddressSpace == AddressSpace.Int64 )
            {
                WriteAlignmentPadding( 8 );
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

        public void ScheduleWriteOffset( Action action ) =>
            ScheduleWriteOffset( 0, 0, AlignmentMode.None, OffsetMode.Offset, action );

        public void ScheduleWriteOffset( OffsetMode offsetMode, Action action ) =>
            ScheduleWriteOffset( 0, 0, AlignmentMode.None, offsetMode, action );

        public void ScheduleWriteOffset( int alignment, AlignmentMode alignmentMode, Action action ) =>
            ScheduleWriteOffset( alignment, 0, alignmentMode, OffsetMode.Offset, action );

        public void ScheduleWriteOffset( int alignment, AlignmentMode alignmentMode, OffsetMode offsetMode, Action action ) =>
            ScheduleWriteOffset( alignment, 0, alignmentMode, offsetMode, action );

        public void ScheduleWriteOffset( int alignment, byte alignmentFillerByte, AlignmentMode alignmentMode, Action action ) =>
            ScheduleWriteOffset( alignment, alignmentFillerByte, alignmentMode, OffsetMode.Offset, action );

        public void ScheduleWriteOffset( int alignment, byte alignmentFillerByte, AlignmentMode alignmentMode, OffsetMode offsetMode, Action action )
        {
            mScheduledWrites.AddLast( new ScheduledWrite
            {
                FieldOffset = PrepareWriteOffset( offsetMode ),
                Alignment = alignment,
                AlignmentFillerByte = alignmentFillerByte,
                AlignmentMode = alignmentMode,
                OffsetMode = offsetMode,
                Action = action,
                BaseOffset = BaseOffset,
            } );
        }

        public void ScheduleWriteOffsetIf( bool condition, Action action )
        {
            if ( condition )
                ScheduleWriteOffset( action );
            else
                PrepareWriteOffset( OffsetMode.Offset );
        }

        public void ScheduleWriteOffsetIf( bool condition, OffsetMode offsetMode, Action action )
        {
            if ( condition )
                ScheduleWriteOffset( offsetMode, action );
            else
                PrepareWriteOffset( offsetMode );
        }

        public void ScheduleWriteOffsetIf( bool condition, int alignment, AlignmentMode alignmentMode, Action action )
        {
            if ( condition )
                ScheduleWriteOffset( alignment, alignmentMode, action );
            else
                PrepareWriteOffset( OffsetMode.Offset );
        }

        public void ScheduleWriteOffsetIf( bool condition, int alignment, AlignmentMode alignmentMode, OffsetMode offsetMode, Action action )
        {
            if ( condition )
                ScheduleWriteOffset( alignment, alignmentMode, offsetMode, action );
            else
                PrepareWriteOffset( offsetMode );
        }

        public void ScheduleWriteOffsetIf( bool condition, int alignment, byte alignmentFillerByte, AlignmentMode alignmentMode, Action action )
        {
            if ( condition )
                ScheduleWriteOffset( alignment, alignmentFillerByte, alignmentMode, action );
            else
                PrepareWriteOffset( OffsetMode.Offset );
        }

        public void ScheduleWriteOffsetIf( bool condition, int alignment, byte alignmentFillerByte, AlignmentMode alignmentMode, OffsetMode offsetMode, Action action )
        {
            if ( condition )
                ScheduleWriteOffset( alignment, alignmentFillerByte, alignmentMode, offsetMode, action );
            else
                PrepareWriteOffset( offsetMode );
        }

        private long PrepareWriteOffset( OffsetMode offsetMode )
        {
            long offset;

            if ( AddressSpace == AddressSpace.Int64 )
            {
                WriteAlignmentPadding( 8 );
                offset = Position;
                Write( 0L );
            }
            else
            {
                offset = Position;
                Write( 0 );
            }


            if ( offsetMode == OffsetMode.OffsetAndSize || offsetMode == OffsetMode.SizeAndOffset )
            {
                if ( AddressSpace == AddressSpace.Int64 )
                    Write( 0L );
                else
                    Write( 0 );
            }

            return offset;
        }

        private void PerformScheduledWrite( LinkedListNode<ScheduledWrite> scheduledWriteNode, long baseOffset )
        {
            var scheduledWrite = scheduledWriteNode.Value;

            if ( scheduledWrite.AlignmentMode == AlignmentMode.Left || scheduledWrite.AlignmentMode == AlignmentMode.Center )
                WriteAlignmentPadding( scheduledWrite.Alignment, scheduledWrite.AlignmentFillerByte );

            var first = mScheduledWrites.Last;

            long startOffset = Position;
            {
                scheduledWrite.Action();
            }
            long endOffset = Position;

            var last = mScheduledWrites.Last;

            if ( scheduledWrite.AlignmentMode == AlignmentMode.Left || scheduledWrite.AlignmentMode == AlignmentMode.Center )
                WriteAlignmentPadding( scheduledWrite.Alignment, scheduledWrite.AlignmentFillerByte );

            if ( scheduledWrite.BaseOffset > 0 )
                baseOffset = scheduledWrite.BaseOffset;

            for ( var current = first.Next; current != null && current != last.Next; current = current.Next )
                PerformScheduledWrite( current, baseOffset );

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

            var first = mScheduledWrites.First;
            var last = mScheduledWrites.Last;

            for ( var current = first; current != null && current != last.Next; current = current.Next )
                PerformScheduledWrite( current, 0 );

            mScheduledWrites.Clear();
        }

        public void PerformScheduledWritesReversed()
        {
            mScheduledWrites = new LinkedList<ScheduledWrite>( mScheduledWrites.Reverse() );
            PerformScheduledWrites();
        }

        public void PushStringTable( StringBinaryFormat format, int fixedLength = -1 ) =>
            PushStringTable( 0, AlignmentMode.None, format, fixedLength );

        public void PushStringTable( int alignment, AlignmentMode alignmentMode, StringBinaryFormat format, int fixedLength = -1 )
        {
            mStringTables.Push( new StringTable
            {
                BaseOffset = BaseOffset,
                Strings = new Dictionary<string, List<long>>(),
                AlignmentMode = alignmentMode,
                Alignment = alignment,
                Format = format,
                FixedLength = fixedLength,
            } );
        }

        private void WriteStringTable( StringTable stringTable )
        {
            if ( stringTable.Strings.Count == 0 )
                return;

            if ( stringTable.AlignmentMode == AlignmentMode.Left ||
                stringTable.AlignmentMode == AlignmentMode.Center )
            {
                WriteAlignmentPadding( stringTable.Alignment );
            }

            foreach ( var keyValuePair in stringTable.Strings.OrderBy( x => x.Value[ 0 ] ) )
            {
                long stringOffset = Position;
                Write( keyValuePair.Key, stringTable.Format, stringTable.FixedLength );
                long endOffset = Position;

                foreach ( var offset in keyValuePair.Value )
                {
                    SeekBegin( offset );
                    WriteOffset( stringOffset - stringTable.BaseOffset );
                    mOffsetPositions.Add( offset );
                }

                SeekBegin( endOffset );
            }

            if ( stringTable.AlignmentMode == AlignmentMode.Right ||
                stringTable.AlignmentMode == AlignmentMode.Center )
            {
                WriteAlignmentPadding( stringTable.Alignment );
            }
        }

        public void PopStringTable()
        {
            WriteStringTable( mStringTables.Pop() );
        }

        public void PopStringTables()
        {
            while ( mStringTables.Count > 0 )
                PopStringTable();
        }

        public void PopStringTablesReversed()
        {
            if ( mStringTables.Count == 0 )
                return;

            else if ( mStringTables.Count == 1 )
                PopStringTable();

            else
            {
                foreach ( var stringTable in mStringTables )
                    WriteStringTable( stringTable );

                mStringTables.Clear();
            }
        }

        public void AddStringToStringTable( string value )
        {
            var stringTable = mStringTables.Peek();

            var position = PrepareWriteOffset( OffsetMode.Offset );

            if ( !string.IsNullOrEmpty( value ) )
            {
                if ( stringTable.Strings.TryGetValue( value, out List<long> positions ) )
                    positions.Add( position );
                else
                {
                    var offsets = new List<long>();
                    offsets.Add( position );
                    stringTable.Strings.Add( value, offsets );
                }
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

        public override void Write( short value )
        {
            base.Write( mSwap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( short[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( ushort value )
        {
            base.Write( mSwap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( ushort[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( int value )
        {
            base.Write( mSwap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( int[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( uint value )
        {
            base.Write( mSwap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( uint[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( long value )
        {
            base.Write( mSwap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( long[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( ulong value )
        {
            base.Write( mSwap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( ulong[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( float value )
        {
            base.Write( mSwap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( float[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public void Write( Half value )
        {
            base.Write( mSwap ? EndiannessSwapUtilities.Swap( value.Value ) : value.Value );
        }

        public void Write( Half[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( decimal value )
        {
            base.Write( mSwap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( decimal[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public void Write( string value, StringBinaryFormat format, int fixedLength = -1 )
        {
            switch ( format )
            {
                case StringBinaryFormat.NullTerminated:
                    {
                        Write( mEncoding.GetBytes( value ) );
                        base.Write( ( byte )0 );
                    }
                    break;
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
                            base.Write( ( byte )0 );
                    }
                    break;

                case StringBinaryFormat.PrefixedLength8:
                    {
                        base.Write( ( byte )value.Length );
                        Write( mEncoding.GetBytes( value ) );
                    }
                    break;

                case StringBinaryFormat.PrefixedLength16:
                    {
                        Write( ( ushort )value.Length );
                        Write( mEncoding.GetBytes( value ) );
                    }
                    break;

                case StringBinaryFormat.PrefixedLength32:
                    {
                        Write( ( uint )value.Length );
                        Write( mEncoding.GetBytes( value ) );
                    }
                    break;

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
                    Write( ( Half )value.X );
                    Write( ( Half )value.Y );
                    break;

                case VectorBinaryFormat.Int16:
                    Write( ( short )( value.X * 32768f ) );
                    Write( ( short )( value.Y * 32768f ) );
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
                    Write( ( Half )value.X );
                    Write( ( Half )value.Y );
                    Write( ( Half )value.Z );
                    break;

                case VectorBinaryFormat.Int16:
                    Write( ( short )( value.X * 32768f ) );
                    Write( ( short )( value.Y * 32768f ) );
                    Write( ( short )( value.Z * 32768f ) );
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
                    Write( ( Half )value.X );
                    Write( ( Half )value.Y );
                    Write( ( Half )value.Z );
                    Write( ( Half )value.W );
                    break;

                case VectorBinaryFormat.Int16:
                    Write( ( short )( value.X * 32768f ) );
                    Write( ( short )( value.Y * 32768f ) );
                    Write( ( short )( value.Z * 32768f ) );
                    Write( ( short )( value.W * 32768f ) );
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
            Write( value.M12 );
            Write( value.M13 );
            Write( value.M14 );
            Write( value.M21 );
            Write( value.M22 );
            Write( value.M23 );
            Write( value.M24 );
            Write( value.M31 );
            Write( value.M32 );
            Write( value.M33 );
            Write( value.M34 );
            Write( value.M41 );
            Write( value.M42 );
            Write( value.M43 );
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
                    Write( ( Half )value.R );
                    Write( ( Half )value.G );
                    Write( ( Half )value.B );
                    Write( ( Half )value.A );
                    break;

                case VectorBinaryFormat.Int16:
                    Write( ( short )( value.R * 32768f ) );
                    Write( ( short )( value.G * 32768f ) );
                    Write( ( short )( value.B * 32768f ) );
                    Write( ( short )( value.A * 32768f ) );
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

        public EndianBinaryWriter( Stream input, Encoding encoding, bool leaveOpen, Endianness endianness )
            : base( input, encoding, leaveOpen )
        {
            Init( encoding, endianness, AddressSpace.Int32 );
        }

        public EndianBinaryWriter( Stream input, Encoding encoding, bool leaveOpen, Endianness endianness, AddressSpace addressSpace )
            : base( input, encoding, leaveOpen )
        {
            Init( encoding, endianness, addressSpace );
        }

        private void Init( Encoding encoding, Endianness endianness, AddressSpace addressSpace )
        {
            Endianness = endianness;
            AddressSpace = addressSpace;
            mEncoding = encoding;
            mOffsets = new Stack<long>();
            mBaseOffsets = new Stack<long>();
            mScheduledWrites = new LinkedList<ScheduledWrite>();
            mStringTables = new Stack<StringTable>();
            mOffsetPositions = new List<long>();
        }
    }
}
