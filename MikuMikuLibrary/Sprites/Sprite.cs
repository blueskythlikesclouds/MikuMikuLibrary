using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Sprites
{
    public enum ResolutionMode
    {
        QVGA,
        VGA,
        SVGA,
        XGA,
        Mode4,
        Mode5,
        UXGA,
        WVGA,
        Mode8,
        WXGA,
        Mode10,
        WUXGA,
        WQXGA,
        HDTV720,
        HDTV1080,
        Mode15,
        Mode16,
        Mode17,
        Custom
    }

    public class Sprite
    {
        public string Name { get; set; }
        public ResolutionMode ResolutionMode { get; set; }
        public int TextureIndex { get; set; }
        public float NdcX { get; set; }
        public float NdcY { get; set; }
        public float NdcWidth { get; set; }
        public float NdcHeight { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            TextureIndex = reader.ReadInt32();
            reader.SeekCurrent( 4 );
            NdcX = reader.ReadSingle();
            NdcY = reader.ReadSingle();
            NdcWidth = reader.ReadSingle();
            NdcHeight = reader.ReadSingle();
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Width = reader.ReadSingle();
            Height = reader.ReadSingle();
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( TextureIndex );
            writer.Write( 0 );
            writer.Write( NdcX );
            writer.Write( NdcY );
            writer.Write( NdcWidth );
            writer.Write( NdcHeight );
            writer.Write( X );
            writer.Write( Y );
            writer.Write( Width );
            writer.Write( Height );
        }

        internal void ReadMode( EndianBinaryReader reader )
        {
            reader.SeekCurrent( 4 );
            ResolutionMode = ( ResolutionMode ) reader.ReadInt32();
        }

        internal void WriteMode( EndianBinaryWriter writer )
        {
            writer.Write( 0 );
            writer.Write( ( int ) ResolutionMode );
        }
    }
}