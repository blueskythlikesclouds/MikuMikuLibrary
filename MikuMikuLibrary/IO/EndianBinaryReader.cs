//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using MikuMikuLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace MikuMikuLibrary.IO
{
    public class EndianBinaryReader : BinaryReader
    {
        private StringBuilder stringBuilder;
        private Endianness endianness;
        private bool swap;
        private Encoding encoding;
        private Stack<long> offsetStack;
        private Stack<long> baseOffsetStack;

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

        public EndianBinaryReader( Stream input, Endianness endianness )
            : base( input )
        {
            Init( Encoding.Default, endianness );
        }

        public EndianBinaryReader( Stream input, Encoding encoding, Endianness endianness )
            : base( input, encoding )
        {
            Init( encoding, endianness );
        }

        public EndianBinaryReader( Stream input, Encoding encoding, bool leaveOpen, Endianness endianness )
            : base( input, encoding, leaveOpen )
        {
            Init( encoding, endianness );
        }

        private void Init( Encoding encoding, Endianness endianness )
        {
            stringBuilder = new StringBuilder();
            this.encoding = encoding;
            offsetStack = new Stack<long>();
            baseOffsetStack = new Stack<long>();
            Endianness = endianness;
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

        public void PushOffsetAndSeekBegin( long offset )
        {
            PushOffset();
            SeekBegin( offset );
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

        public void ReadAlignmentPadding( int alignment )
        {
            SeekCurrent( AlignmentUtilities.GetAlignedDifference( Position, alignment ) );
        }

        public void ReadAtOffset( long offset, Action body )
        {
            SeekBegin( ( baseOffsetStack.Count > 0 ?
                baseOffsetStack.Peek() : 0 ) + offset );

            body.Invoke();
        }

        public void ReadAtOffsetIf( bool condition, long offset, Action body )
        {
            if ( condition )
                ReadAtOffset( offset, body );
        }

        public void ReadAtOffsetIfNotZero( long offset, Action body )
        {
            if ( offset > 0 )
                ReadAtOffset( offset, body );
        }

        public void ReadAtOffsetAndSeekBack( long offset, Action body )
        {
            long positionTemp = Position;

            SeekBegin( ( baseOffsetStack.Count > 0 ?
                baseOffsetStack.Peek() : 0 ) + offset );

            body.Invoke();

            SeekBegin( positionTemp );
        }

        public void ReadAtOffsetAndSeekBackIf( bool condition, long offset, Action body )
        {
            if ( condition )
                ReadAtOffsetAndSeekBack( offset, body );
        }

        public void ReadAtOffsetAndSeekBackIfNotZero( long offset, Action body )
        {
            if ( offset > 0 )
                ReadAtOffsetAndSeekBack( offset, body );
        }

        public string ReadStringAtOffset( long offset, StringBinaryFormat format, int fixedLength = -1 )
        {
            if ( offset > 0 )
            {
                if ( baseOffsetStack.Count > 0 )
                    offset += baseOffsetStack.Peek();

                long previousOffset = Position;
                SeekBegin( offset );
                string value = ReadString( format, fixedLength );
                SeekBegin( previousOffset );
                return value;
            }
            return null;
        }

        public string ReadStringPtr( StringBinaryFormat format, int fixedLength = -1 )
        {
            long offset = ReadUInt32();
            if ( offset > 0 )
            {
                if ( baseOffsetStack.Count > 0 )
                    offset += baseOffsetStack.Peek();

                long previousOffset = Position;
                SeekBegin( offset );
                string value = ReadString( format, fixedLength );
                SeekBegin( previousOffset );
                return value;
            }
            return null;
        }

        public sbyte[] ReadSBytes( int count )
        {
            sbyte[] array = new sbyte[ count ];
            for ( int i = 0; i < array.Length; i++ )
                array[ i ] = ReadSByte();

            return array;
        }

        public bool[] ReadBooleans( int count )
        {
            bool[] array = new bool[ count ];
            for ( int i = 0; i < array.Length; i++ )
                array[ i ] = ReadBoolean();

            return array;
        }

        public override short ReadInt16()
        {
            if ( swap )
                return EndiannessSwapUtilities.Swap( base.ReadInt16() );
            else
                return base.ReadInt16();
        }

        public short[] ReadInt16s( int count )
        {
            short[] array = new short[ count ];
            for ( int i = 0; i < array.Length; i++ )
            {
                array[ i ] = ReadInt16();
            }

            return array;
        }

        public override ushort ReadUInt16()
        {
            if ( swap )
                return EndiannessSwapUtilities.Swap( base.ReadUInt16() );
            else
                return base.ReadUInt16();
        }

        public ushort[] ReadUInt16s( int count )
        {
            ushort[] array = new ushort[ count ];
            for ( int i = 0; i < array.Length; i++ )
            {
                array[ i ] = ReadUInt16();
            }

            return array;
        }

        public override decimal ReadDecimal()
        {
            if ( swap )
                return EndiannessSwapUtilities.Swap( base.ReadDecimal() );
            else
                return base.ReadDecimal();
        }

        public decimal[] ReadDecimals( int count )
        {
            decimal[] array = new decimal[ count ];
            for ( int i = 0; i < array.Length; i++ )
            {
                array[ i ] = ReadDecimal();
            }

            return array;
        }

        public override double ReadDouble()
        {
            if ( swap )
                return EndiannessSwapUtilities.Swap( base.ReadDouble() );
            else
                return base.ReadDouble();
        }

        public double[] ReadDoubles( int count )
        {
            double[] array = new double[ count ];
            for ( int i = 0; i < array.Length; i++ )
            {
                array[ i ] = ReadDouble();
            }

            return array;
        }

        public override int ReadInt32()
        {
            if ( swap )
                return EndiannessSwapUtilities.Swap( base.ReadInt32() );
            else
                return base.ReadInt32();
        }

        public int[] ReadInt32s( int count )
        {
            int[] array = new int[ count ];
            for ( int i = 0; i < array.Length; i++ )
            {
                array[ i ] = ReadInt32();
            }

            return array;
        }

        public override long ReadInt64()
        {
            if ( swap )
                return EndiannessSwapUtilities.Swap( base.ReadInt64() );
            else
                return base.ReadInt64();
        }

        public long[] ReadInt64s( int count )
        {
            long[] array = new long[ count ];
            for ( int i = 0; i < array.Length; i++ )
            {
                array[ i ] = ReadInt64();
            }

            return array;
        }

        public override float ReadSingle()
        {
            if ( swap )
                return EndiannessSwapUtilities.Swap( base.ReadSingle() );
            else
                return base.ReadSingle();
        }

        public float[] ReadSingles( int count )
        {
            float[] array = new float[ count ];
            for ( int i = 0; i < array.Length; i++ )
            {
                array[ i ] = ReadSingle();
            }

            return array;
        }

        public override uint ReadUInt32()
        {
            if ( swap )
                return EndiannessSwapUtilities.Swap( base.ReadUInt32() );
            else
                return base.ReadUInt32();
        }

        public uint[] ReadUInt32s( int count )
        {
            uint[] array = new uint[ count ];
            for ( int i = 0; i < array.Length; i++ )
            {
                array[ i ] = ReadUInt32();
            }

            return array;
        }

        public override ulong ReadUInt64()
        {
            if ( swap )
                return EndiannessSwapUtilities.Swap( base.ReadUInt64() );
            else
                return base.ReadUInt64();
        }

        public ulong[] ReadUInt64s( int count )
        {
            ulong[] array = new ulong[ count ];
            for ( int i = 0; i < array.Length; i++ )
            {
                array[ i ] = ReadUInt64();
            }

            return array;
        }

        public string ReadString( StringBinaryFormat format, int fixedLength = -1 )
        {
            stringBuilder.Clear();

            switch ( format )
            {
                case StringBinaryFormat.NullTerminated:
                    {
                        char b;
                        while ( ( b = ReadChar() ) != 0 )
                            stringBuilder.Append( b );
                    }
                    break;

                case StringBinaryFormat.FixedLength:
                    {
                        if ( fixedLength == -1 )
                            throw new ArgumentException( "Invalid fixed length specified" );

                        char b;
                        for ( int i = 0; i < fixedLength; i++ )
                        {
                            b = ReadChar();
                            if ( b != 0 )
                                stringBuilder.Append( b );
                        }
                    }
                    break;

                case StringBinaryFormat.PrefixedLength8:
                    {
                        byte length = ReadByte();
                        for ( int i = 0; i < length; i++ )
                            stringBuilder.Append( ReadChar() );
                    }
                    break;

                case StringBinaryFormat.PrefixedLength16:
                    {
                        ushort length = ReadUInt16();
                        for ( int i = 0; i < length; i++ )
                            stringBuilder.Append( ReadChar() );
                    }
                    break;

                case StringBinaryFormat.PrefixedLength32:
                    {
                        uint length = ReadUInt32();
                        for ( int i = 0; i < length; i++ )
                            stringBuilder.Append( ReadChar() );
                    }
                    break;

                default:
                    throw new ArgumentException( "Unknown string format", nameof( format ) );
            }

            return stringBuilder.ToString();
        }

        public string[] ReadStrings( int count, StringBinaryFormat format, int fixedLength = -1 )
        {
            string[] value = new string[ count ];
            for ( int i = 0; i < value.Length; i++ )
                value[ i ] = ReadString( format, fixedLength );

            return value;
        }

        public Vector2 ReadVector2()
        {
            return new Vector2( ReadSingle(), ReadSingle() );
        }

        public Vector2[] ReadVector2s( int count )
        {
            Vector2[] value = new Vector2[ count ];
            for ( int i = 0; i < value.Length; i++ )
                value[ i ] = ReadVector2();

            return value;
        }

        public Vector3 ReadVector3()
        {
            return new Vector3( ReadSingle(), ReadSingle(), ReadSingle() );
        }

        public Vector3[] ReadVector3s( int count )
        {
            Vector3[] value = new Vector3[ count ];
            for ( int i = 0; i < value.Length; i++ )
                value[ i ] = ReadVector3();

            return value;
        }

        public Vector4 ReadVector4()
        {
            return new Vector4( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
        }

        public Vector4[] ReadVector4s( int count )
        {
            Vector4[] value = new Vector4[ count ];
            for ( int i = 0; i < value.Length; i++ )
                value[ i ] = ReadVector4();

            return value;
        }

        public Matrix4x4 ReadMatrix4x4()
        {
            return new Matrix4x4(
                 ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(),
                 ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(),
                 ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(),
                 ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
        }

        public Matrix4x4[] ReadMatrix4x4s( int count )
        {
            Matrix4x4[] value = new Matrix4x4[ count ];
            for ( int i = 0; i < value.Length; i++ )
                value[ i ] = ReadMatrix4x4();

            return value;
        }

        public Color ReadColor()
        {
            return new Color( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
        }

        public Color[] ReadColors( int count )
        {
            Color[] value = new Color[ count ];
            for ( int i = 0; i < value.Length; i++ )
                value[ i ] = ReadColor();

            return value;
        }
    }
}
