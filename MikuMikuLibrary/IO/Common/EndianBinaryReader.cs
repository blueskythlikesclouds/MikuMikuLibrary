//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using MikuMikuLibrary.Exceptions;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Misc;

namespace MikuMikuLibrary.IO.Common
{
    public class EndianBinaryReader : BinaryReader
    {
        private StringBuilder mStringBuilder;
        private Endianness mEndianness;
        private Encoding mEncoding;
        private Stack<long> mOffsets;
        private Stack<long> mBaseOffsets;

        public Endianness Endianness
        {
            get => mEndianness;
            set
            {
                SwapBytes = value != EndiannessSwapUtilities.SystemEndianness;
                mEndianness = value;
            }
        }

        public bool SwapBytes { get; private set; }

        public AddressSpace AddressSpace { get; set; }

        public long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public long Length => BaseStream.Length;

        public long BaseOffset
        {
            get => mBaseOffsets.Count > 0 ? mBaseOffsets.Peek() : 0;
            set
            {
                if ( mBaseOffsets.Count > 0 && mBaseOffsets.Peek() != value || mBaseOffsets.Count == 0 )
                    mBaseOffsets.Push( value );
            }
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

        public void PushOffset( long offset )
        {
            mOffsets.Push( offset );
        }

        public void PushOffset()
        {
            mOffsets.Push( Position );
        }

        public long PeekOffset()
        {
            return mOffsets.Peek();
        }

        public long PopOffset()
        {
            return mOffsets.Pop();
        }

        public long SeekBeginToPoppedOffset()
        {
            return BaseStream.Seek( mOffsets.Pop(), SeekOrigin.Begin );
        }

        public void PushBaseOffset( long offset )
        {
            mBaseOffsets.Push( offset );
        }

        public void PushBaseOffset()
        {
            mBaseOffsets.Push( Position );
        }

        public long PeekBaseOffset()
        {
            return mBaseOffsets.Peek();
        }

        public long PopBaseOffset()
        {
            return mBaseOffsets.Pop();
        }

        public void Align( int alignment )
        {
            SeekCurrent( AlignmentUtilities.GetAlignedDifference( Position, alignment ) );
        }

        public long ReadOffset()
        {
            long offset;

            if ( AddressSpace == AddressSpace.Int32 )
            {
                offset = ReadUInt32();
            }

            else if ( AddressSpace == AddressSpace.Int64 )
            {
                Align( 8 );
                offset = ReadInt64();
            }

            else
            {
                throw new ArgumentException( nameof( AddressSpace ) );
            }

            return offset;
        }

        public long[] ReadOffsets( int count )
        {
            if ( AddressSpace == AddressSpace.Int64 )
                Align( 8 );

            var offsets = new long[ count ];
            for ( int i = 0; i < count; i++ )
            {
                long offset;
                if ( AddressSpace == AddressSpace.Int32 )
                    offset = ReadUInt32();
                else if ( AddressSpace == AddressSpace.Int64 )
                    offset = ReadInt64();
                else
                    throw new ArgumentException( nameof( AddressSpace ) );

                offsets[ i ] = offset;
            }

            return offsets;
        }

        public void ReadOffset( Action<EndianBinaryReader> action )
        {
            long offset = ReadOffset();
            if ( offset != 0 )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                action( this );
                SeekBegin( current );
            }
        }

        public void ReadOffset( int count, Action<EndianBinaryReader, int> action )
        {
            long offset = ReadOffset();
            if ( offset != 0 )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                for ( int i = 0; i < count; i++ )
                    action( this, i );
                SeekBegin( current );
            }
        }

        public void ReadOffsetIf( bool condition, Action<EndianBinaryReader> action )
        {
            long offset = ReadOffset();
            if ( offset != 0 && condition )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                action( this );
                SeekBegin( current );
            }
        }

        public void ReadOffset( Action action )
        {
            long offset = ReadOffset();
            if ( offset != 0 )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                action();
                SeekBegin( current );
            }
        }

        public void ReadOffset( int count, Action<int> action )
        {
            long offset = ReadOffset();
            if ( offset != 0 )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                for ( int i = 0; i < count; i++ )
                    action( i );
                SeekBegin( current );
            }
        }

        public void ReadOffsetIf( bool condition, Action action )
        {
            long offset = ReadOffset();
            if ( offset != 0 && condition )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                action();
                SeekBegin( current );
            }
        }

        public void ReadAtOffset( long offset, Action<EndianBinaryReader> action )
        {
            if ( offset != 0 )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                action( this );
                SeekBegin( current );
            }
        }

        public void ReadAtOffset( long offset, int count, Action<EndianBinaryReader, int> action )
        {
            if ( offset != 0 )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                for ( int i = 0; i < count; i++ )
                    action( this, i );
                SeekBegin( current );
            }
        }

        public void ReadAtOffsetIf( bool condition, long offset, Action<EndianBinaryReader> action )
        {
            if ( offset != 0 && condition )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                action( this );
                SeekBegin( current );
            }
        }

        public void ReadAtOffset( long offset, Action action )
        {
            if ( offset != 0 )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                action();
                SeekBegin( current );
            }
        }

        public void ReadAtOffset( long offset, int count, Action<int> action )
        {
            if ( offset != 0 )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                for ( int i = 0; i < count; i++ )
                    action( i );
                SeekBegin( current );
            }
        }

        public void ReadAtOffsetIf( bool condition, long offset, Action action )
        {
            if ( offset != 0 && condition )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                action();
                SeekBegin( current );
            }
        }

        public sbyte[] ReadSBytes( int count )
        {
            var array = new sbyte[ count ];
            for ( int i = 0; i < array.Length; i++ )
                array[ i ] = ReadSByte();

            return array;
        }

        public bool[] ReadBooleans( int count )
        {
            var array = new bool[ count ];
            for ( int i = 0; i < array.Length; i++ )
                array[ i ] = ReadBoolean();

            return array;
        }

        public override short ReadInt16()
        {
            if ( SwapBytes )
                return EndiannessSwapUtilities.Swap( base.ReadInt16() );
            return base.ReadInt16();
        }

        public short[] ReadInt16s( int count )
        {
            var array = new short[ count ];
            for ( int i = 0; i < array.Length; i++ ) array[ i ] = ReadInt16();

            return array;
        }

        public override ushort ReadUInt16()
        {
            if ( SwapBytes )
                return EndiannessSwapUtilities.Swap( base.ReadUInt16() );
            return base.ReadUInt16();
        }

        public ushort[] ReadUInt16s( int count )
        {
            var array = new ushort[ count ];
            for ( int i = 0; i < array.Length; i++ ) array[ i ] = ReadUInt16();

            return array;
        }

        public override decimal ReadDecimal()
        {
            if ( SwapBytes )
                return EndiannessSwapUtilities.Swap( base.ReadDecimal() );
            return base.ReadDecimal();
        }

        public decimal[] ReadDecimals( int count )
        {
            var array = new decimal[ count ];
            for ( int i = 0; i < array.Length; i++ ) array[ i ] = ReadDecimal();

            return array;
        }

        public override double ReadDouble()
        {
            if ( SwapBytes )
                return EndiannessSwapUtilities.Swap( base.ReadDouble() );
            return base.ReadDouble();
        }

        public double[] ReadDoubles( int count )
        {
            var array = new double[ count ];
            for ( int i = 0; i < array.Length; i++ ) array[ i ] = ReadDouble();

            return array;
        }

        public override int ReadInt32()
        {
            if ( SwapBytes )
                return EndiannessSwapUtilities.Swap( base.ReadInt32() );
            return base.ReadInt32();
        }

        public int[] ReadInt32s( int count )
        {
            var array = new int[ count ];
            for ( int i = 0; i < array.Length; i++ ) array[ i ] = ReadInt32();

            return array;
        }

        public override long ReadInt64()
        {
            if ( SwapBytes )
                return EndiannessSwapUtilities.Swap( base.ReadInt64() );
            return base.ReadInt64();
        }

        public long[] ReadInt64s( int count )
        {
            var array = new long[ count ];
            for ( int i = 0; i < array.Length; i++ ) array[ i ] = ReadInt64();

            return array;
        }

        public Half ReadHalf()
        {
            if ( SwapBytes )
                return Half.ToHalf( EndiannessSwapUtilities.Swap( base.ReadUInt16() ) );
            return Half.ToHalf( base.ReadUInt16() );
        }

        public Half[] ReadHalfs( int count )
        {
            var array = new Half[ count ];
            for ( int i = 0; i < array.Length; i++ )
                array[ i ] = ReadHalf();

            return array;
        }

        public override float ReadSingle()
        {
            if ( SwapBytes )
                return EndiannessSwapUtilities.Swap( base.ReadSingle() );
            return base.ReadSingle();
        }

        public float[] ReadSingles( int count )
        {
            var array = new float[ count ];
            for ( int i = 0; i < array.Length; i++ ) array[ i ] = ReadSingle();

            return array;
        }

        public override uint ReadUInt32()
        {
            if ( SwapBytes )
                return EndiannessSwapUtilities.Swap( base.ReadUInt32() );
            return base.ReadUInt32();
        }

        public uint[] ReadUInt32s( int count )
        {
            var array = new uint[ count ];
            for ( int i = 0; i < array.Length; i++ ) array[ i ] = ReadUInt32();

            return array;
        }

        public override ulong ReadUInt64()
        {
            if ( SwapBytes )
                return EndiannessSwapUtilities.Swap( base.ReadUInt64() );
            return base.ReadUInt64();
        }

        public ulong[] ReadUInt64s( int count )
        {
            var array = new ulong[ count ];
            for ( int i = 0; i < array.Length; i++ ) array[ i ] = ReadUInt64();

            return array;
        }

        public string ReadString( StringBinaryFormat format, int fixedLength = -1 )
        {
            mStringBuilder.Clear();

            switch ( format )
            {
                case StringBinaryFormat.NullTerminated:
                {
                    char b;
                    while ( ( b = ReadChar() ) != 0 )
                        mStringBuilder.Append( b );
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
                            mStringBuilder.Append( b );
                    }
                }
                    break;

                case StringBinaryFormat.PrefixedLength8:
                {
                    byte length = ReadByte();
                    for ( int i = 0; i < length; i++ )
                        mStringBuilder.Append( ReadChar() );
                }
                    break;

                case StringBinaryFormat.PrefixedLength16:
                {
                    ushort length = ReadUInt16();
                    for ( int i = 0; i < length; i++ )
                        mStringBuilder.Append( ReadChar() );
                }
                    break;

                case StringBinaryFormat.PrefixedLength32:
                {
                    uint length = ReadUInt32();
                    for ( int i = 0; i < length; i++ )
                        mStringBuilder.Append( ReadChar() );
                }
                    break;

                default:
                    throw new ArgumentException( "Unknown string format", nameof( format ) );
            }

            return mStringBuilder.ToString();
        }

        public string PeekString( StringBinaryFormat format, int fixedLength = -1 )
        {
            string ret;

            long current = Position;
            {
                ret = ReadString( format, fixedLength );
            }
            SeekBegin( current );

            return ret;
        }

        public string[] ReadStrings( int count, StringBinaryFormat format, int fixedLength = -1 )
        {
            var value = new string[ count ];
            for ( int i = 0; i < value.Length; i++ )
                value[ i ] = ReadString( format, fixedLength );

            return value;
        }

        public string ReadStringOffset( StringBinaryFormat format, int fixedLength = -1 )
        {
            long offset = ReadOffset();
            if ( offset != 0 )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                string value = ReadString( format, fixedLength );
                SeekBegin( current );
                return value;
            }

            return null;
        }

        public string ReadStringAtOffset( long offset, StringBinaryFormat format, int fixedLength = -1 )
        {
            if ( offset != 0 )
            {
                long current = Position;
                SeekBegin( BaseOffset + offset );
                string value = ReadString( format, fixedLength );
                SeekBegin( current );
                return value;
            }

            return null;
        }

        public Vector2 ReadVector2()
        {
            return new Vector2( ReadSingle(), ReadSingle() );
        }

        public Vector2 ReadVector2( VectorBinaryFormat format )
        {
            switch ( format )
            {
                case VectorBinaryFormat.Single:
                    return new Vector2( ReadSingle(), ReadSingle() );

                case VectorBinaryFormat.Half:
                    return new Vector2( ReadHalf(), ReadHalf() );

                case VectorBinaryFormat.Int16:
                    return new Vector2( ReadInt16() / 32768f, ReadInt16() / 32768f );

                default:
                    throw new ArgumentException( nameof( format ) );
            }
        }

        public Vector2[] ReadVector2s( int count )
        {
            var value = new Vector2[ count ];
            for ( int i = 0; i < value.Length; i++ )
                value[ i ] = ReadVector2();

            return value;
        }

        public Vector3 ReadVector3()
        {
            return new Vector3( ReadSingle(), ReadSingle(), ReadSingle() );
        }

        public Vector3 ReadVector3( VectorBinaryFormat format )
        {
            switch ( format )
            {
                case VectorBinaryFormat.Single:
                    return new Vector3( ReadSingle(), ReadSingle(), ReadSingle() );

                case VectorBinaryFormat.Int16:
                    return new Vector3( ReadInt16() / 32768f, ReadInt16() / 32768f, ReadInt16() / 32768f );

                default:
                    throw new ArgumentException( nameof( format ) );
            }
        }

        public Vector3[] ReadVector3s( int count )
        {
            var value = new Vector3[ count ];
            for ( int i = 0; i < value.Length; i++ )
                value[ i ] = ReadVector3();

            return value;
        }

        public Vector4 ReadVector4()
        {
            return new Vector4( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
        }

        public Vector4 ReadVector4( VectorBinaryFormat format )
        {
            switch ( format )
            {
                case VectorBinaryFormat.Single:
                    return new Vector4( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );

                case VectorBinaryFormat.Half:
                    return new Vector4( ReadHalf(), ReadHalf(), ReadHalf(), ReadHalf() );

                case VectorBinaryFormat.Int16:
                    return new Vector4( ReadInt16() / 32768f, ReadInt16() / 32768f, ReadInt16() / 32768f,
                        ReadInt16() / 32768f );

                default:
                    throw new ArgumentException( nameof( format ) );
            }
        }

        public Vector4[] ReadVector4s( int count )
        {
            var value = new Vector4[ count ];
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
            var value = new Matrix4x4[ count ];
            for ( int i = 0; i < value.Length; i++ )
                value[ i ] = ReadMatrix4x4();

            return value;
        }

        public Color ReadColor()
        {
            return new Color( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
        }

        public Color ReadColor( VectorBinaryFormat format )
        {
            switch ( format )
            {
                case VectorBinaryFormat.Single:
                    return new Color( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );

                case VectorBinaryFormat.Half:
                    return new Color( ReadHalf(), ReadHalf(), ReadHalf(), ReadHalf() );

                case VectorBinaryFormat.Int16:
                    return new Color( ReadInt16() / 32768f, ReadInt16() / 32768f, ReadInt16() / 32768f,
                        ReadInt16() / 32768f );

                default:
                    throw new ArgumentException( nameof( format ) );
            }
        }

        public Color[] ReadColors( int count )
        {
            var value = new Color[ count ];
            for ( int i = 0; i < value.Length; i++ )
                value[ i ] = ReadColor();

            return value;
        }

        public BoundingSphere ReadBoundingSphere()
        {
            return new BoundingSphere
            {
                Center = ReadVector3(),
                Radius = ReadSingle()
            };
        }

        public BoundingBox ReadBoundingBox()
        {
            return new BoundingBox
            {
                Center = ReadVector3(),
                Width = ReadSingle(),
                Height = ReadSingle(),
                Depth = ReadSingle()
            };
        }

        public int ReadSignature( int expectedSignature )
        {
            int signature = ReadInt32();
            if ( signature != expectedSignature )
                throw new InvalidSignatureException( signature, expectedSignature );

            return signature;
        }

        public int ReadSignature( params int[] expectedSignatures )
        {
            int signature = ReadInt32();
            if ( !expectedSignatures.Contains( signature ) )
                throw new InvalidSignatureException( signature, expectedSignatures );

            return signature;
        }

        public int ReadSignature( IEnumerable<int> expectedSignatures )
        {
            int signature = ReadInt32();
            if ( !expectedSignatures.Contains( signature ) )
                throw new InvalidSignatureException( signature, expectedSignatures );

            return signature;
        }

        public string ReadSignature( int length, string expectedSignature )
        {
            string signature = ReadString( StringBinaryFormat.FixedLength, length );
            if ( signature != expectedSignature )
                throw new InvalidSignatureException( signature, expectedSignature );

            return signature;
        }

        public string ReadSignature( int length, params string[] expectedSignatures )
        {
            string signature = ReadString( StringBinaryFormat.FixedLength, length );
            if ( !expectedSignatures.Contains( signature ) )
                throw new InvalidSignatureException( signature, expectedSignatures );

            return signature;
        }

        public string ReadSignature( int length, IEnumerable<string> expectedSignatures )
        {
            string signature = ReadString( StringBinaryFormat.FixedLength, length );
            if ( !expectedSignatures.Contains( signature ) )
                throw new InvalidSignatureException( signature, expectedSignatures );

            return signature;
        }

        private void Init( Encoding encoding, Endianness endianness, AddressSpace addressSpace )
        {
            mStringBuilder = new StringBuilder();
            mEncoding = encoding;
            AddressSpace = addressSpace;
            mOffsets = new Stack<long>();
            mBaseOffsets = new Stack<long>();
            Endianness = endianness;
        }

        public EndianBinaryReader( Stream input, Endianness endianness )
            : base( input )
        {
            Init( Encoding.Default, endianness, AddressSpace.Int32 );
        }

        public EndianBinaryReader( Stream input, Endianness endianness, AddressSpace addressSpace )
            : base( input )
        {
            Init( Encoding.Default, endianness, addressSpace );
        }

        public EndianBinaryReader( Stream input, Encoding encoding, Endianness endianness )
            : base( input, encoding )
        {
            Init( encoding, endianness, AddressSpace.Int32 );
        }

        public EndianBinaryReader( Stream input, Encoding encoding, Endianness endianness, AddressSpace addressSpace )
            : base( input, encoding )
        {
            Init( encoding, endianness, addressSpace );
        }

        public EndianBinaryReader( Stream input, Encoding encoding, bool leaveOpen, Endianness endianness )
            : base( input, encoding, leaveOpen )
        {
            Init( encoding, endianness, AddressSpace.Int32 );
        }

        public EndianBinaryReader( Stream input, Encoding encoding, bool leaveOpen, Endianness endianness,
            AddressSpace addressSpace )
            : base( input, encoding, leaveOpen )
        {
            Init( encoding, endianness, addressSpace );
        }
    }
}