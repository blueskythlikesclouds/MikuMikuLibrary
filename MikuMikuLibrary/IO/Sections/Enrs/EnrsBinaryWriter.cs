using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.IO.Sections.Enrs
{
    public class EnrsBinaryWriter : EndianBinaryWriter
    {
        private readonly long mBeginPosition;
        private readonly List<byte> mBitArray;

        private void SetBitArrayValue( long offset, byte value )
        {
            if ( offset < 0 )
                return;

            int index = ( int ) ( offset / 4 );

            while ( mBitArray.Count < index + 1 )
                mBitArray.Add( 0xFF );

            int bitOffset = ( int ) ( ( offset % 4 ) * 2 );
            mBitArray[ index ] = BitHelper.Pack( mBitArray[ index ], value, bitOffset, bitOffset + 2 );
        }

        private int GetBitArrayValue( long offset )
        {
            int index = ( int ) ( offset / 4 );

            if ( index < 0 || index >= mBitArray.Count )
                return 3;

            int bitOffset = ( int ) ( ( offset % 4 ) * 2 );
            return BitHelper.Unpack( mBitArray[ index ], bitOffset, bitOffset + 2 );
        }

        public override void Write( short value )
        {
            if ( EndiannessHelper.Swap( value ) != value )
                SetBitArrayValue( Position - mBeginPosition, ( int ) ValueType.Int16 );

            base.Write( value );
        }

        public override unsafe void Write( ushort value ) => 
            Write( *( short* ) &value );

        public override void Write( int value )
        {
            if ( EndiannessHelper.Swap( value ) != value )
                SetBitArrayValue( Position - mBeginPosition, ( int ) ValueType.Int32 );

            base.Write( value );
        }

        public override unsafe void Write( uint value ) => 
            Write( *( int* ) &value );

        public override void Write( long value )
        {
            if ( EndiannessHelper.Swap( value ) != value )
                SetBitArrayValue( Position - mBeginPosition, ( int ) ValueType.Int64 );

            base.Write( value );
        }

        public override unsafe void Write( ulong value ) => 
            Write( *( long* ) &value );

        public override unsafe void Write( float value ) => 
            Write( *( int* ) &value );

        public override unsafe void Write( double value ) => 
            Write( *( long* ) &value );

        public List<ScopeDescriptor> CreateScopeDescriptors( long dataSize )
        {
            var scopeDescriptors = new List<ScopeDescriptor>();
            var fieldDescriptors = new List<FieldDescriptor>();

            for ( long offset = 0; offset < dataSize; )
            {
                int type = GetBitArrayValue( offset );

                if ( type == 3 )
                {
                    offset++;
                    continue;
                }

                var fieldDescriptor = new FieldDescriptor
                {
                    Position = offset,
                    ValueType = ( ValueType ) type,
                    RepeatCount = 1
                };

                int byteSize = 2 << type;

                for ( ; offset < dataSize; )
                {
                    offset += byteSize;

                    if ( GetBitArrayValue( offset ) != type )
                        break;

                    fieldDescriptor.RepeatCount++;
                }

                fieldDescriptors.Add( fieldDescriptor );
            }

            // TODO: Optimize
            var scopeDescriptor = new ScopeDescriptor { Position = fieldDescriptors[ 0 ].Position, RepeatCount = 1 };

            scopeDescriptor.FieldDescriptors.AddRange( fieldDescriptors );
            scopeDescriptors.Add( scopeDescriptor );

            return scopeDescriptors;
        }

        public EnrsBinaryWriter( Stream input, Encoding encoding, Endianness endianness, bool leaveOpen, long beginPosition ) 
            : base( input, encoding, endianness, leaveOpen )
        {
            mBeginPosition = beginPosition;
            mBitArray = new List<byte>( 1024 * 1024 );
        }
    }
}