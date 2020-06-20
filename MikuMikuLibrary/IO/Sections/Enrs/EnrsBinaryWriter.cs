using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.IO.Sections.Enrs
{
    public class EnrsBinaryWriter : EndianBinaryWriter
    {
        private readonly Dictionary<long, int> mPositionMap;

        public override void Write( short value )
        {
            if ( EndiannessHelper.Swap(  value ) != value )
                mPositionMap[ Position ] = sizeof( short );

            base.Write( value );
        }

        public override unsafe void Write( ushort value ) => 
            Write( *( short* ) &value );

        public override void Write( int value )
        {
            if ( EndiannessHelper.Swap(  value ) != value )
                mPositionMap[ Position ] = sizeof( int );

            base.Write( value );
        }

        public override unsafe void Write( uint value ) => 
            Write( *( int* ) &value );

        public override void Write( long value )
        {
            if ( EndiannessHelper.Swap(  value ) != value )
                mPositionMap[ Position ] = sizeof( long );

            base.Write( value );
        }

        public override unsafe void Write( ulong value ) => 
            Write( *( long* ) &value );

        public override unsafe void Write( float value ) => 
            Write( *( int* ) &value );

        public override unsafe void Write( double value ) => 
            Write( *( long* ) &value );

        public List<ScopeDescriptor> CreateScopeDescriptors( long minPosition, long maxPosition )
        {
            var scopeDescriptors = new List<ScopeDescriptor>();
            var fieldDescriptors = new List<FieldDescriptor>();

            FieldDescriptor previousFieldDescriptor = null;
            long previousPosition = minPosition;
            int previousByteSize = 0;

            foreach ( var kvp in mPositionMap.OrderBy( x => x.Key ).Where( 
                x => x.Key >= minPosition && x.Key < maxPosition ) )
            {
                long position = kvp.Key;
                int byteSize = kvp.Value;

                var valueType =
                    byteSize == 2 ? ValueType.Int16 :
                    byteSize == 4 ? ValueType.Int32 :
                    byteSize == 8 ? ValueType.Int64 :
                    throw new InvalidDataException( "Expected value of Int16, Int32 or Int64 type." );

                if ( previousFieldDescriptor == null || previousFieldDescriptor.ValueType != valueType ||
                     previousPosition + previousByteSize != position )
                {
                    fieldDescriptors.Add( previousFieldDescriptor = new FieldDescriptor
                    {
                        Position = position - minPosition,
                        RepeatCount = 1,
                        ValueType = valueType
                    } );
                }

                else
                {
                    previousFieldDescriptor.RepeatCount++;
                }

                previousPosition = position;
                previousByteSize = byteSize;
            }

            // TODO: Optimize
            var scopeDescriptor = new ScopeDescriptor { Position = fieldDescriptors[ 0 ].Position, RepeatCount = 1 };

            scopeDescriptor.FieldDescriptors.AddRange( fieldDescriptors );
            scopeDescriptors.Add( scopeDescriptor );

            return scopeDescriptors;
        }

        public EnrsBinaryWriter( Stream input, Encoding encoding, Endianness endianness, bool leaveOpen ) 
            : base( input, encoding, endianness, leaveOpen )
        {
            mPositionMap = new Dictionary<long, int>( 1024 * 1024 );
        }
    }
}