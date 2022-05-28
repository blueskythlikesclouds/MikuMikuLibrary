using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Archives.CriMw
{
    public class UtfRow : Dictionary<string, object>
    {
        public UtfRow() : base( StringComparer.OrdinalIgnoreCase )
        {
        }

        public T Get<T>( string name )
        {
            return ( T ) Convert.ChangeType( this[ name ], typeof( T ) );
        }
    }

    public class UtfTable
    {
        internal static readonly byte[] Signature = { 0x40, 0x55, 0x54, 0x46 };

        internal static readonly Type[] FieldTypes =
        {
            typeof( byte ),
            typeof( sbyte ),
            typeof( ushort ),
            typeof( short ),
            typeof( uint ),
            typeof( int ),
            typeof( ulong ),
            typeof( long ),
            typeof( float ),
            typeof( double ),
            typeof( string ),
            typeof( byte[] ),
        };
        
        internal struct Field
        {
            public Type Type;
            public string Name;
            public bool HasDefaultValue;
            public object DefaultValue;
            public bool IsValid;
        }

        public static UtfRow[] Read( EndianBinaryReader reader )
        {
            if ( reader.ReadString( StringBinaryFormat.FixedLength, 4 ) != "@UTF" )
                throw new InvalidDataException( "Invalid UTF table signature." );

            int length = reader.ReadInt32();

            reader.PushBaseOffset();

            ushort encoding = reader.ReadUInt16();

            if ( encoding == 1 )
                reader.Encoding = Encoding.UTF8;
            else
                reader.Encoding = Encoding.GetEncoding( "shift-jis" );

            ushort rowsOffset = reader.ReadUInt16();
            long stringPoolOffset = reader.ReadUInt32();
            long dataPoolOffset = reader.ReadUInt32();
            long nameOffset = reader.ReadUInt32();
            ushort fieldCount = reader.ReadUInt16();
            ushort rowStride = reader.ReadUInt16();
            int rowCount = reader.ReadInt32();

            var fields = new Field[ fieldCount ];
            
            for ( int i = 0; i < fieldCount; i++ )
            {
                byte flags = reader.ReadByte();

                var field = new Field
                {
                    Type = FieldTypes[ flags & 0xF ],
                    Name = ( flags & 0x10 ) != 0 ? ReadString() : string.Empty,
                    HasDefaultValue = ( flags & 0x20 ) != 0
                };

                if ( field.HasDefaultValue )
                    field.DefaultValue = ReadValue( field.Type );

                field.IsValid = ( flags & 0x40 ) != 0;

                fields[ i ] = field;
            }

            var rows = new UtfRow[ rowCount ];

            for ( int i = 0; i < rowCount; i++ )
            {
                rows[ i ] = new UtfRow();

                reader.ReadAtOffset( rowsOffset + i * rowStride, () =>
                {
                    foreach ( var field in fields )
                    {
                        object value = field.Type.IsValueType ? Activator.CreateInstance( field.Type ) : null;

                        if ( field.HasDefaultValue )
                            value = field.DefaultValue;

                        else if ( field.IsValid )
                            value = ReadValue( field.Type );

                        rows[ i ][ field.Name ] = value;
                    }
                } );
            }

            string ReadString()
            {
                long offset = reader.ReadUInt32();
                
                if ( offset == 0 )
                    return string.Empty;

                return reader.ReadStringAtOffset( stringPoolOffset + offset, StringBinaryFormat.NullTerminated );
            }

            object ReadValue( Type type )
            {
                if ( type == typeof( byte ) ) return reader.ReadByte();
                if ( type == typeof( sbyte ) ) return reader.ReadSByte();
                if ( type == typeof( ushort ) ) return reader.ReadUInt16();
                if ( type == typeof( short ) ) return reader.ReadInt16();
                if ( type == typeof( uint ) ) return reader.ReadUInt32();
                if ( type == typeof( int ) ) return reader.ReadInt16();
                if ( type == typeof( ulong ) ) return reader.ReadUInt64();
                if ( type == typeof( long ) ) return reader.ReadInt64();
                if ( type == typeof( float ) ) return reader.ReadSingle();
                if ( type == typeof( double ) ) return reader.ReadDouble();
                if ( type == typeof( string ) ) return ReadString();
                if ( type == typeof( byte[] ) )
                {
                    long offset = reader.ReadUInt32();
                    int len = reader.ReadInt32();

                    var bytes = Array.Empty<byte>();

                    if ( offset > 0 )
                        reader.ReadAtOffset( dataPoolOffset + offset, () => bytes = reader.ReadBytes( len ) );

                    return bytes;
                }

                throw new ArgumentException( "Unknown type" );
            }

            return rows;
        }

        public static UtfRow[] Read( byte[] bytes )
        {
            if ( bytes[ 0 ] != Signature[ 0 ] ||
                 bytes[ 1 ] != Signature[ 1 ] || 
                 bytes[ 2 ] != Signature[ 2 ] ||
                 bytes[ 3 ] != Signature[ 3 ] )
            {
                // Masked UTF table.
                int current = 25951;

                for ( int i = 0; i < bytes.Length; i++ )
                {
                    bytes[ i ] ^= ( byte ) ( current & 0xFF );
                    current *= 16661;
                }
            }

            using ( var reader = new EndianBinaryReader( new MemoryStream( bytes ), Endianness.Big ) )
                return Read( reader );
        }

        public static UtfRow[] ReadFromChunk( EndianBinaryReader reader, string expectedSignature )
        {
            if ( reader.ReadString( StringBinaryFormat.FixedLength, 4 ) != expectedSignature )
                throw new InvalidDataException( $"Invalid signature, expected {expectedSignature}" );

            reader.SeekCurrent( 4 );
            int length = reader.ReadInt32();
            reader.SeekCurrent( 4 );

            return Read( reader.ReadBytes( length ) );
        }
    }
}