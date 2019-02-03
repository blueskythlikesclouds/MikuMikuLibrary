using MikuMikuLibrary.IO.Common;
using System;
using System.IO;

namespace MikuMikuLibrary.Textures
{
    public class SubTexture
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TextureFormat Format { get; private set; }
        public int Id { get; private set; }
        public byte[] Data { get; private set; }

        internal void Read( EndianBinaryReader reader )
        {
            var signature = reader.ReadInt32();
            if ( signature != 0x02505854 )
                throw new InvalidDataException( "Invalid signature (expected TXP with type 2)" );

            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            Format = ( TextureFormat )reader.ReadInt32();
            Id = reader.ReadInt32();

            int dataSize = reader.ReadInt32();
            Data = reader.ReadBytes( dataSize );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( 0x02505854 );
            writer.Write( Width );
            writer.Write( Height );
            writer.Write( ( int )Format );
            writer.Write( Id );
            writer.Write( Data.Length );
            writer.Write( Data );
        }

        internal SubTexture( EndianBinaryReader reader )
        {
            Read( reader );
        }

        internal SubTexture( int width, int height, TextureFormat format, int id )
        {
            Width = Math.Max( 1, width );
            Height = Math.Max( 1, height );
            Format = format;
            Id = id;
            Data = new byte[ TextureFormatUtilities.CalculateDataSize( width, height, format ) ];
        }
    }
}
