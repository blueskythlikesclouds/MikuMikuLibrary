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

    public class EndianBinaryWriter : BinaryWriter
    {
        private class OffsetWrite
        {
            public long BaseOffset;
            public long FieldOffset;
            public Action Body;
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
                Write( filler );
        }

        public void WriteNulls( int count )
        {
            while ( count-- > 0 )
                Write( ( byte )0 );
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
            body.Invoke();

            SeekBegin( previousOffset );
        }

        public void EnqueueOffsetWrite( Action body )
        {
            offsetWriteQueue.Enqueue( new OffsetWrite
            {
                FieldOffset = Position,
                Body = body,
                AlignmentKind = AlignmentKind.None,
                Alignment = 0,
                AlignmentFillerByte = 0,
                BaseOffset = baseOffsetStack.Count > 0 ? baseOffsetStack.Peek() : 0,
            } );

            Write( 0u );
        }

        public void EnqueueOffsetWriteAligned( int alignment, AlignmentKind alignmentKind, Action body )
        {
            offsetWriteQueue.Enqueue( new OffsetWrite
            {
                FieldOffset = Position,
                Body = body,
                AlignmentKind = alignmentKind,
                Alignment = alignment,
                AlignmentFillerByte = 0,
                BaseOffset = baseOffsetStack.Count > 0 ? baseOffsetStack.Peek() : 0,
            } );

            Write( 0u );
        }

        public void EnqueueOffsetWriteAligned( int alignment, byte alignmentFillerByte, AlignmentKind alignmentKind, Action body )
        {
            offsetWriteQueue.Enqueue( new OffsetWrite
            {
                FieldOffset = Position,
                Body = body,
                AlignmentKind = alignmentKind,
                Alignment = alignment,
                AlignmentFillerByte = alignmentFillerByte,
                BaseOffset = baseOffsetStack.Count > 0 ? baseOffsetStack.Peek() : 0,
            } );

            Write( 0u );
        }

        public void EnqueueOffsetWriteIf( bool condition, Action body )
        {
            if ( condition )
                EnqueueOffsetWrite( body );
            else
                Write( 0 );
        }

        public void EnqueueOffsetWriteAlignedIf( bool condition, int alignment, AlignmentKind alignmentKind, Action body )
        {
            if ( condition )
                EnqueueOffsetWriteAligned( alignment, alignmentKind, body );
            else
                Write( 0 );
        }

        public void EnqueueOffsetWriteAlignedIf( bool condition, int alignment, byte alignmentFillerByte, AlignmentKind alignmentKind, Action body )
        {
            if ( condition )
                EnqueueOffsetWriteAligned( alignment, alignmentFillerByte, alignmentKind, body );
            else
                Write( 0 );
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
            offsetWrite.Body.Invoke();

            if ( offsetWrite.AlignmentKind == AlignmentKind.Right ||
                offsetWrite.AlignmentKind == AlignmentKind.Center )
            {
                WriteAlignmentPadding( offsetWrite.Alignment, offsetWrite.AlignmentFillerByte );
            }

            if ( offsetWrite.BaseOffset != 0 )
                baseOffset = offsetWrite.BaseOffset;

            long endOffset = Position;
            SeekBegin( offsetWrite.FieldOffset );
            Write( ( uint )( offset - baseOffset ) );
            SeekBegin( endOffset );

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
            stringTableStack.Push( new StringTable
            {
                BaseOffset = baseOffsetStack.Count > 0 ? baseOffsetStack.Peek() : 0,
                Strings = new Dictionary<string, List<long>>(),
                AlignmentKind = AlignmentKind.None,
                Alignment = 0,
                Format = format,
                FixedLength = fixedLength,
            } );
        }

        public void PushStringTableAligned( int alignment, AlignmentKind alignmentKind, StringBinaryFormat format, int fixedLength = -1 )
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
                var stringTableStack = new Stack<StringTable>();
                foreach ( var stringTable in this.stringTableStack )
                    stringTableStack.Push( stringTable );

                this.stringTableStack = stringTableStack;
                PopStringTables();
            }
        }

        public void AddStringToStringTable( string value )
        {
            var stringTable = stringTableStack.Peek();

            if ( stringTable.Strings.ContainsKey( value ) )
                stringTable.Strings[ value ].Add( Position );
            else
            {
                var offsets = new List<long>();
                offsets.Add( Position );
                stringTable.Strings.Add( value, offsets );
            }

            Write( 0 );
        }

        public void Write( sbyte[] values )
        {
            for ( int i = 0; i < values.Length; i++ )
                Write( values[ i ] );
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

        public void WriteHalf( Half value )
        {
            base.Write( swap ? EndiannessSwapUtilities.Swap( value.value ) : value.value );
        }

        public void WriteHalfs( Half[] values )
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
                        Write( ( byte )0 );
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
                            Write( ( byte )0 );
                    }
                    break;

                case StringBinaryFormat.PrefixedLength8:
                    {
                        Write( ( byte )value.Length );
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

        public void Write( Vector2[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public void WriteVector2Half( Vector2 value )
        {
            WriteHalf( ( Half )value.X );
            WriteHalf( ( Half )value.Y );
        }

        public void Write( Vector3 value )
        {
            Write( value.X );
            Write( value.Y );
            Write( value.Z );
        }

        public void Write( Vector3[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public void WriteVector3Int16( Vector3 value )
        {
            Write( ( short )( value.X * 32768f ) );
            Write( ( short )( value.Y * 32768f ) );
            Write( ( short )( value.Z * 32768f ) );
        }

        public void Write( Vector4 value )
        {
            Write( value.X );
            Write( value.Y );
            Write( value.Z );
            Write( value.W );
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

        public void Write( Color[] values )
        {
            foreach ( var value in values )
                Write( value );
        }

        public void WriteColorHalf( Color value )
        {
            WriteHalf( ( Half )value.R );
            WriteHalf( ( Half )value.G );
            WriteHalf( ( Half )value.B );
            WriteHalf( ( Half )value.A );
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
