//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MikuMikuLibrary.IO.Common
{
    public enum AlignmentKind
    {
        None,
        Left,
        Center,
        Right,
    };

    public enum OffsetKind
    {
        Offset,
        Size,
        OffsetAndSize,
        SizeAndOffset,
    };

    public class EndianBinaryWriter : BinaryWriter
    {
        private class OffsetWrite
        {
            public long BaseOffset;
            public long FieldOffset;
            public Action WriteAction;
            public OffsetKind OffsetKind;
            public AlignmentKind AlignmentKind;
            public int Alignment;
            public byte AlignmentFillerByte;
        }

        private class StringTable
        {
            public long BaseOffset;
            public Dictionary<string, List<long>> Strings;
            public AlignmentKind AlignmentKind;
            public int Alignment;
            public StringBinaryFormat Format;
            public int FixedLength;

            public StringTable()
            {
                Strings = new Dictionary<string, List<long>>();
            }
        }

        private Endianness endianness;
        private bool swap;
        private Encoding encoding;
        private Stack<long> offsetStack;
        private Stack<long> baseOffsetStack;
        private Queue<OffsetWrite> offsetWriteQueue;
        private Stack<StringTable> stringTableStack;
        private List<long> offsetPositions;

        public Endianness Endianness
        {
            get { return endianness; }
            set
            {
                if ( value != EndiannessSwapUtilities.SystemEndianness )
                    swap = true;
                else
                    swap = false;

                endianness = value;
            }
        }

        public bool EndiannessNeedsSwapping
        {
            get { return swap; }
        }

        public long Position
        {
            get { return BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        public long BaseStreamLength
        {
            get { return BaseStream.Length; }
        }

        public List<long> OffsetPositions
        {
            get { return offsetPositions; }
        }

        public EndianBinaryWriter( Stream input, Endianness endianness )
            : base( input )
        {
            Init( Encoding.Default, endianness );
        }

        public EndianBinaryWriter( Stream input, Encoding encoding, Endianness endianness )
            : base( input, encoding )
        {
            Init( encoding, endianness );
        }

        public EndianBinaryWriter( Stream input, Encoding encoding, bool leaveOpen, Endianness endianness )
            : base( input, encoding, leaveOpen )
        {
            Init( encoding, endianness );
        }

        private void Init( Encoding encoding, Endianness endianness )
        {
            Endianness = endianness;
            this.encoding = encoding;
            offsetStack = new Stack<long>();
            baseOffsetStack = new Stack<long>();
            offsetWriteQueue = new Queue<OffsetWrite>();
            stringTableStack = new Stack<StringTable>();
            offsetPositions = new List<long>();
        }

        public void Seek( long offset, SeekOrigin origin )
        {
            BaseStream.Seek( offset, origin );
        }

        public void SeekBegin( long offset )
        {
            BaseStream.Seek( offset, SeekOrigin.Begin );
        }

        public void SeekCurrent( long offset )
        {
            BaseStream.Seek( offset, SeekOrigin.Current );
        }

        public void SeekEnd( long offset )
        {
            BaseStream.Seek( offset, SeekOrigin.End );
        }

        public void PushOffset()
        {
            offsetStack.Push( Position );
        }

        public long PeekOffset()
        {
            return offsetStack.Peek();
        }

        public long PopOffset()
        {
            return offsetStack.Pop();
        }

        public void SeekBeginToPoppedOffset()
        {
            SeekBegin( PopOffset() );
        }

        public void PushBaseOffset()
        {
            baseOffsetStack.Push( Position );
        }

        public long PeekBaseOffset()
        {
            return baseOffsetStack.Peek();
        }

        public long PopBaseOffset()
        {
            return baseOffsetStack.Pop();
        }

        public void WriteAlignmentPadding( int alignment, byte filler = 0 )
        {
            var difference =
                AlignmentUtilities.GetAlignedDifference( Position, alignment );

            while ( difference-- > 0 )
                base.Write( filler );
        }

        public void WriteNulls( int count )
        {
            while ( count-- > 0 )
                base.Write( ( byte )0 );
        }

        public void WriteAtOffset( long offset, Action body )
        {
            SeekBegin( offset );
            body.Invoke();
        }

        public void WriteAtOffsetAndSeekBack( long offset, Action body )
        {
            long previousOffset = Position;

            SeekBegin( offset );
            {
                body.Invoke();
            }
            SeekBegin( previousOffset );
        }

        public void EnqueueOffsetWrite( Action body )
        {
            EnqueueOffsetWrite( 0, 0, AlignmentKind.None, OffsetKind.Offset, body );
        }

        public void EnqueueOffsetWrite( OffsetKind offsetKind, Action body )
        {
            EnqueueOffsetWrite( 0, 0, AlignmentKind.None, offsetKind, body );
        }

        public void EnqueueOffsetWrite( int alignment, AlignmentKind alignmentKind, Action body )
        {
            EnqueueOffsetWrite( alignment, 0, alignmentKind, OffsetKind.Offset, body );
        }

        public void EnqueueOffsetWrite( int alignment, AlignmentKind alignmentKind, OffsetKind offsetKind, Action body )
        {
            EnqueueOffsetWrite( alignment, 0, alignmentKind, offsetKind, body );
        }

        public void EnqueueOffsetWrite( int alignment, byte alignmentFillerByte, AlignmentKind alignmentKind, Action body )
        {
            EnqueueOffsetWrite( alignment, alignmentFillerByte, alignmentKind, OffsetKind.Offset, body );
        }

        public void EnqueueOffsetWrite( int alignment, byte alignmentFillerByte, AlignmentKind alignmentKind, OffsetKind offsetKind, Action body )
        {
            offsetWriteQueue.Enqueue( new OffsetWrite
            {
                FieldOffset = Position,
                Alignment = alignment,
                AlignmentFillerByte = alignmentFillerByte,
                AlignmentKind = alignmentKind,
                OffsetKind = offsetKind,
                WriteAction = body,
                BaseOffset = baseOffsetStack.Any() ? baseOffsetStack.Peek() : 0,
            } );

            PrepareOffsetWrite( offsetKind );
        }

        public void EnqueueOffsetWriteIf( bool condition, Action body )
        {
            if ( condition )
                EnqueueOffsetWrite( body );
            else
                PrepareOffsetWrite( OffsetKind.Offset );
        }

        public void EnqueueOffsetWriteIf( bool condition, OffsetKind offsetKind, Action body )
        {
            if ( condition )
                EnqueueOffsetWrite( offsetKind, body );
            else
                PrepareOffsetWrite( offsetKind );
        }

        public void EnqueueOffsetWriteIf( bool condition, int alignment, AlignmentKind alignmentKind, Action body )
        {
            if ( condition )
                EnqueueOffsetWrite( alignment, alignmentKind, body );
            else
                PrepareOffsetWrite( OffsetKind.Offset );
        }

        public void EnqueueOffsetWriteIf( bool condition, int alignment, AlignmentKind alignmentKind, OffsetKind offsetKind, Action body )
        {
            if ( condition )
                EnqueueOffsetWrite( alignment, alignmentKind, offsetKind, body );
            else
                PrepareOffsetWrite( offsetKind );
        }

        public void EnqueueOffsetWriteIf( bool condition, int alignment, byte alignmentFillerByte, AlignmentKind alignmentKind, Action body )
        {
            if ( condition )
                EnqueueOffsetWrite( alignment, alignmentFillerByte, alignmentKind, body );
            else
                PrepareOffsetWrite( OffsetKind.Offset );
        }

        public void EnqueueOffsetWriteIf( bool condition, int alignment, byte alignmentFillerByte, AlignmentKind alignmentKind, OffsetKind offsetKind, Action body )
        {
            if ( condition )
                EnqueueOffsetWrite( alignment, alignmentFillerByte, alignmentKind, offsetKind, body );
            else
                PrepareOffsetWrite( offsetKind );
        }

        private void PrepareOffsetWrite( OffsetKind offsetKind )
        {
            switch ( offsetKind )
            {
                case OffsetKind.Offset:
                case OffsetKind.Size:
                    Write( 0 );
                    break;

                case OffsetKind.OffsetAndSize:
                case OffsetKind.SizeAndOffset:
                    Write( 0L );
                    break;
            }
        }

        private void DoOffsetWrite( OffsetWrite offsetWrite, long baseOffset )
        {
            offsetWriteQueue = new Queue<OffsetWrite>();

            if ( offsetWrite.AlignmentKind == AlignmentKind.Left ||
                offsetWrite.AlignmentKind == AlignmentKind.Center )
            {
                WriteAlignmentPadding( offsetWrite.Alignment, offsetWrite.AlignmentFillerByte );
            }

            long offset = Position;
            offsetWrite.WriteAction.Invoke();
            long endOffset = Position;

            if ( offsetWrite.AlignmentKind == AlignmentKind.Right ||
                offsetWrite.AlignmentKind == AlignmentKind.Center )
            {
                WriteAlignmentPadding( offsetWrite.Alignment, offsetWrite.AlignmentFillerByte );
            }

            if ( offsetWrite.BaseOffset != 0 )
                baseOffset = offsetWrite.BaseOffset;

            long endOffsetAligned = Position;
            SeekBegin( offsetWrite.FieldOffset );
            switch ( offsetWrite.OffsetKind )
            {
                case OffsetKind.Offset:
                    Write( ( uint )( offset - baseOffset ) );
                    break;
                case OffsetKind.Size:
                    Write( ( uint )( endOffset - offset ) );
                    break;
                case OffsetKind.OffsetAndSize:
                    Write( ( uint )( offset - baseOffset ) );
                    Write( ( uint )( endOffset - offset ) );
                    break;
                case OffsetKind.SizeAndOffset:
                    Write( ( uint )( endOffset - offset ) );
                    Write( ( uint )( offset - baseOffset ) );
                    break;
                default:
                    throw new ArgumentException( nameof( offsetWrite.OffsetKind ) );
            }
            SeekBegin( endOffsetAligned );

            // For the relocation table
            offsetPositions.Add( offsetWrite.FieldOffset );

            var offsetWriteQueueForThisOffsetWrite = offsetWriteQueue;
            while ( offsetWriteQueueForThisOffsetWrite.Count > 0 )
                DoOffsetWrite( offsetWriteQueueForThisOffsetWrite.Dequeue(), baseOffset );
        }

        public void DoEnqueuedOffsetWrites()
        {
            var offsetWriteQueue = this.offsetWriteQueue;

            while ( offsetWriteQueue.Count > 0 )
                DoOffsetWrite( offsetWriteQueue.Dequeue(), 0 );

            this.offsetWriteQueue.Clear();
        }

        public void DoEnqueuedOffsetWritesReversed()
        {
            offsetWriteQueue = new Queue<OffsetWrite>( offsetWriteQueue.Reverse() );
            DoEnqueuedOffsetWrites();
        }

        public void PushStringTable( StringBinaryFormat format, int fixedLength = -1 )
        {
            PushStringTable( 0, AlignmentKind.None, format, fixedLength );
        }

        public void PushStringTable( int alignment, AlignmentKind alignmentKind, StringBinaryFormat format, int fixedLength = -1 )
        {
            stringTableStack.Push( new StringTable
            {
                BaseOffset = baseOffsetStack.Count > 0 ? baseOffsetStack.Peek() : 0,
                Strings = new Dictionary<string, List<long>>(),
                AlignmentKind = alignmentKind,
                Alignment = alignment,
                Format = format,
                FixedLength = fixedLength,
            } );
        }

        public void PopStringTable()
        {
            var stringTable = stringTableStack.Pop();

            if ( stringTable.Strings.Count == 0 )
                return;

            if ( stringTable.AlignmentKind == AlignmentKind.Left ||
                stringTable.AlignmentKind == AlignmentKind.Center )
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
                    Write( ( uint )( stringOffset - stringTable.BaseOffset ) );
                    offsetPositions.Add( offset );
                }

                SeekBegin( endOffset );
            }

            if ( stringTable.AlignmentKind == AlignmentKind.Right ||
                stringTable.AlignmentKind == AlignmentKind.Center )
            {
                WriteAlignmentPadding( stringTable.Alignment );
            }
        }

        public void PopStringTables()
        {
            while ( stringTableStack.Count > 0 )
                PopStringTable();
        }

        public void PopStringTablesReversed()
        {
            if ( stringTableStack.Count == 0 )
                return;

            else if ( stringTableStack.Count == 1 )
                PopStringTable();

            else
            {
                var stringTableStack = new Stack<StringTable>( this.stringTableStack.Count );
                foreach ( var stringTable in this.stringTableStack )
                    stringTableStack.Push( stringTable );

                this.stringTableStack = stringTableStack;
                PopStringTables();
            }
        }

        public void AddStringToStringTable( string value )
        {
            var stringTable = stringTableStack.Peek();

            if ( !string.IsNullOrEmpty( value ) )
            {
                if ( stringTable.Strings.TryGetValue( value, out List<long> positions ) )
                    positions.Add( Position );
                else
                {
                    var offsets = new List<long>();
                    offsets.Add( Position );
                    stringTable.Strings.Add( value, offsets );
                }
            }

            Write( 0 );
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
            base.Write( swap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( short[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( ushort value )
        {
            base.Write( swap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( ushort[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( int value )
        {
            base.Write( swap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( int[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( uint value )
        {
            base.Write( swap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( uint[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( long value )
        {
            base.Write( swap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( long[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( ulong value )
        {
            base.Write( swap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( ulong[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( float value )
        {
            base.Write( swap ? EndiannessSwapUtilities.Swap( value ) : value );
        }

        public void Write( float[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public void Write( Half value )
        {
            base.Write( swap ? EndiannessSwapUtilities.Swap( value.Value ) : value.Value );
        }

        public void Write( Half[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public override void Write( decimal value )
        {
            base.Write( swap ? EndiannessSwapUtilities.Swap( value ) : value );
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
                        Write( encoding.GetBytes( value ) );
                        base.Write( ( byte )0 );
                    }
                    break;
                case StringBinaryFormat.FixedLength:
                    {
                        if ( fixedLength == -1 )
                        {
                            throw new ArgumentException( "Fixed length must be provided if format is set to fixed length", nameof( fixedLength ) );
                        }

                        var bytes = encoding.GetBytes( value );
                        if ( bytes.Length > fixedLength )
                        {
                            throw new ArgumentException( "Provided string is longer than fixed length", nameof( value ) );
                        }

                        Write( bytes );
                        fixedLength -= bytes.Length;

                        while ( fixedLength-- > 0 )
                            base.Write( ( byte )0 );
                    }
                    break;

                case StringBinaryFormat.PrefixedLength8:
                    {
                        base.Write( ( byte )value.Length );
                        Write( encoding.GetBytes( value ) );
                    }
                    break;

                case StringBinaryFormat.PrefixedLength16:
                    {
                        Write( ( ushort )value.Length );
                        Write( encoding.GetBytes( value ) );
                    }
                    break;

                case StringBinaryFormat.PrefixedLength32:
                    {
                        Write( ( uint )value.Length );
                        Write( encoding.GetBytes( value ) );
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
                DoEnqueuedOffsetWrites();
                PopStringTablesReversed();
            }

            base.Dispose( disposing );
        }
    }
}
