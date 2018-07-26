using MikuMikuLibrary.IO;
using System.IO;

namespace MikuMikuLibrary.Textures
{
    public enum TextureFormat
    {
        RGB = 1,
        RGBA = 2,
        DXT1 = 6,
        DXT3 = 7,
        DXT5 = 9,
        ATI1 = 10,
        ATI2 = 11,
    }

    public class TextureMipMap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public TextureFormat Format { get; set; }
        public int ID { get; set; }
        public byte[] Data { get; set; }

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
    }
}
