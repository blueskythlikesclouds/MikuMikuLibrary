using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Materials
{
    public class MaterialTexture
    {
        public int Field00 { get; set; }
        public int Field01 { get; set; }
        public int TextureId { get; set; }
        public int Field02 { get; set; }
        public float Field03 { get; set; }
        public float Field04 { get; set; }
        public float Field05 { get; set; }
        public float Field06 { get; set; }
        public float Field07 { get; set; }
        public float Field08 { get; set; }
        public float Field09 { get; set; }
        public float Field10 { get; set; }
        public float Field11 { get; set; }
        public float Field12 { get; set; }
        public float Field13 { get; set; }
        public float Field14 { get; set; }
        public float Field15 { get; set; }
        public float Field16 { get; set; }
        public float Field17 { get; set; }
        public float Field18 { get; set; }
        public float Field19 { get; set; }
        public float Field20 { get; set; }
        public float Field21 { get; set; }
        public float Field22 { get; set; }
        public float Field23 { get; set; }
        public float Field24 { get; set; }
        public float Field25 { get; set; }
        public float Field26 { get; set; }
        public float Field27 { get; set; }
        public float Field28 { get; set; }

        public bool IsActive => Field02 != 240;

        internal void Read( EndianBinaryReader reader )
        {
            Field00 = reader.ReadInt32();
            Field01 = reader.ReadInt32();
            TextureId = reader.ReadInt32();
            Field02 = reader.ReadInt32();
            Field03 = reader.ReadSingle();
            Field04 = reader.ReadSingle();
            Field05 = reader.ReadSingle();
            Field06 = reader.ReadSingle();
            Field07 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Field09 = reader.ReadSingle();
            Field10 = reader.ReadSingle();
            Field11 = reader.ReadSingle();
            Field12 = reader.ReadSingle();
            Field13 = reader.ReadSingle();
            Field14 = reader.ReadSingle();
            Field15 = reader.ReadSingle();
            Field16 = reader.ReadSingle();
            Field17 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field19 = reader.ReadSingle();
            Field20 = reader.ReadSingle();
            Field21 = reader.ReadSingle();
            Field22 = reader.ReadSingle();
            Field23 = reader.ReadSingle();
            Field24 = reader.ReadSingle();
            Field25 = reader.ReadSingle();
            Field26 = reader.ReadSingle();
            Field27 = reader.ReadSingle();
            Field28 = reader.ReadSingle();
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( Field00 );
            writer.Write( Field01 );
            writer.Write( TextureId );
            writer.Write( Field02 );
            writer.Write( Field03 );
            writer.Write( Field04 );
            writer.Write( Field05 );
            writer.Write( Field06 );
            writer.Write( Field07 );
            writer.Write( Field08 );
            writer.Write( Field09 );
            writer.Write( Field10 );
            writer.Write( Field11 );
            writer.Write( Field12 );
            writer.Write( Field13 );
            writer.Write( Field14 );
            writer.Write( Field15 );
            writer.Write( Field16 );
            writer.Write( Field17 );
            writer.Write( Field18 );
            writer.Write( Field19 );
            writer.Write( Field20 );
            writer.Write( Field21 );
            writer.Write( Field22 );
            writer.Write( Field23 );
            writer.Write( Field24 );
            writer.Write( Field25 );
            writer.Write( Field26 );
            writer.Write( Field27 );
            writer.Write( Field28 );
        }

        public MaterialTexture()
        {
            TextureId = -1;
            Field02 = 240;
            Field05 = 1f;
        }
    }
}
