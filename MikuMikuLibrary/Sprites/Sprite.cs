using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Sprites
{
    public class Sprite
    {
        public string Name { get; set; }
        public int Field00 { get; set; }
        public int Field01 { get; set; }
        public int TextureIndex { get; set; }
        public float Field02 { get; set; }
        public float NdcX { get; set; }
        public float NdcY { get; set; }
        public float NdcWidth { get; set; }
        public float NdcHeight { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        internal void ReadFirst( EndianBinaryReader reader )
        {
            TextureIndex = reader.ReadInt32();
            Field02 = reader.ReadSingle();
            NdcX = reader.ReadSingle();
            NdcY = reader.ReadSingle();
            NdcWidth = reader.ReadSingle();
            NdcHeight = reader.ReadSingle();
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Width = reader.ReadSingle();
            Height = reader.ReadSingle();
        }

        internal void WriteFirst( EndianBinaryWriter writer )
        {
            writer.Write( TextureIndex );
            writer.Write( Field02 );
            writer.Write( NdcX );
            writer.Write( NdcY );
            writer.Write( NdcWidth );
            writer.Write( NdcHeight );
            writer.Write( X );
            writer.Write( Y );
            writer.Write( Width );
            writer.Write( Height );
        }

        internal void ReadSecond( EndianBinaryReader reader )
        {
            Field00 = reader.ReadInt32();
            Field01 = reader.ReadInt32();
        }

        internal void WriteSecond( EndianBinaryWriter writer )
        {
            writer.Write( Field00 );
            writer.Write( Field01 );
        }
    }
}
