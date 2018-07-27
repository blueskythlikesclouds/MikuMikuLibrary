using MikuMikuLibrary.IO;
using System.IO;

namespace MikuMikuLibrary.Textures
{
    public class SubTexture
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TextureFormat Format { get; private set; }
        public int ID { get; private set; }
        public byte[] Data { get; private set; }

        internal void Read( EndianBinaryReader reader )
        {
            var signature = reader.ReadString( StringBinaryFormat.FixedLength, 3 );
            if ( signature != "TXP" )
                throw new InvalidDataException( "Invalid signature (expected TXP)" );

            byte typeNum = reader.ReadByte();
            if ( typeNum != 2 )
                throw new InvalidDataException( "Invalid type number (expected 2)" );

            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            Format = ( TextureFormat )reader.ReadInt32();
            ID = reader.ReadInt32();

            int dataSize = reader.ReadInt32();
            Data = reader.ReadBytes( dataSize );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( "TXP", StringBinaryFormat.FixedLength, 3 );
            writer.Write( ( byte )2 );
            writer.Write( Width );
            writer.Write( Height );
            writer.Write( ( int )Format );
            writer.Write( ID );
            writer.Write( Data.Length );
            writer.Write( Data );
        }

        internal SubTexture( EndianBinaryReader reader )
        {
            Read( reader );
        }

        internal SubTexture( int width, int height, TextureFormat format, int id )
        {
            if ( width < 1 )
                width = 1;

            if ( height < 1 )
                height = 1;

            Width = width;
            Height = height;
            Format = format;
            ID = id;
            Data = new byte[ TextureFormatUtilities.CalculateDataSize( width, height, format ) ];
        }
    }
}
