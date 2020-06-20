using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MikuMikuLibrary.IO.Sections.Enrs
{
    public class ScopeDescriptor
    {
        public long Position { get; set; }
        public int RepeatCount { get; set; }
        public List<FieldDescriptor> FieldDescriptors { get; }

        public static void WriteDescriptors( BinaryWriter writer, IEnumerable<ScopeDescriptor> scopeDescriptors )
        {
            long lastScopePosition = 0;
            foreach ( var scopeDescriptor in scopeDescriptors.OrderBy( x => x.Position ) )
            {
                WritePackedValue( writer, ( int ) ( scopeDescriptor.Position - lastScopePosition ) );
                WritePackedValue( writer, scopeDescriptor.FieldDescriptors.Count );
                WritePackedValue( writer, 0 ); // ByteSize
                WritePackedValue( writer, scopeDescriptor.RepeatCount );

                long lastFieldEndPosition = scopeDescriptor.Position;

                foreach ( var fieldDescriptor in scopeDescriptor.FieldDescriptors )
                {
                    WritePackedValue( writer, ( int ) ( fieldDescriptor.Position - lastFieldEndPosition ), fieldDescriptor.ValueType );
                    WritePackedValue( writer, fieldDescriptor.RepeatCount );

                    lastFieldEndPosition = fieldDescriptor.Position + ( 2 << ( int ) fieldDescriptor.ValueType ) * fieldDescriptor.RepeatCount;
                }

                lastScopePosition = scopeDescriptor.Position;
            }
        }

        private static void WritePackedValue( BinaryWriter writer, int value, ValueType valueType )
        {
            int or = ( int ) valueType << 4;

            if ( value < 0x10 )
            {
                writer.Write( ( byte ) ( value | or ) );
            }

            else if ( value < 0x1000 )
            {
                writer.Write( ( byte ) ( ( value >> 8 ) | 0x40 | or ) );
                writer.Write( ( byte ) ( value & 0xFF ) );
            }

            else if ( value < 0x10000000 )
            {
                writer.Write( ( byte ) ( ( value >> 24 ) | 0x80 | or ) );
                writer.Write( ( byte ) ( ( value >> 16 ) & 0xFF ) );
                writer.Write( ( byte ) ( ( value >> 8 ) & 0xFF ) );
                writer.Write( ( byte ) ( value & 0xFF ) );
            }

            else
            {
                throw new ArgumentOutOfRangeException( nameof( value ) );
            }
        }

        private static void WritePackedValue( BinaryWriter writer, int value )
        {
            if ( value < 0x40 )
            {
                writer.Write( ( byte ) value );
            }

            else if ( value < 0x4000 )
            {
                writer.Write( ( byte ) ( ( value >> 8 ) | 0x40 ) );
                writer.Write( ( byte ) ( value & 0xFF ) );
            }

            else if ( value < 0x40000000 )
            {
                writer.Write( ( byte ) ( ( value >> 24 ) | 0x80 ) );
                writer.Write( ( byte ) ( ( value >> 16 ) & 0xFF ) );
                writer.Write( ( byte ) ( ( value >> 8 ) & 0xFF ) );
                writer.Write( ( byte ) ( value & 0xFF ) );
            }

            else
            {
                throw new ArgumentOutOfRangeException( nameof( value ) );
            }
        }

        public ScopeDescriptor()
        {
            FieldDescriptors = new List<FieldDescriptor>();
        }
    }
}