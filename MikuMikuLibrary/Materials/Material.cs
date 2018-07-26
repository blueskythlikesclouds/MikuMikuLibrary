using MikuMikuLibrary.IO;

namespace MikuMikuLibrary.Materials
{
    public class MaterialTexture
    {
        public int Field00 { get; set; }
        public int Field01 { get; set; }
        public int TextureID { get; set; }
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

        internal void Read( EndianBinaryReader reader )
        {
            Field00 = reader.ReadInt32();
            Field01 = reader.ReadInt32();
            TextureID = reader.ReadInt32();
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
            writer.Write( TextureID );
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
            TextureID = -1;
            Field02 = 240;
            Field05 = 1f;
        }
    }

    public class Material
    {
        public const int ByteSize = 0x4B0;

        public int Field00 { get; set; }
        public string Shader { get; set; }
        public MaterialTexture Diffuse { get; }
        public MaterialTexture Ambient { get; }
        public MaterialTexture Normal { get; }
        public MaterialTexture Specular { get; }
        public MaterialTexture ToonCurve { get; }
        public MaterialTexture Reflection { get; }
        public MaterialTexture SpecularPower { get; }
        public MaterialTexture Texture08 { get; }
        public int Field01 { get; set; }
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
        public string Name { get; set; }
        public float Field25 { get; set; }
        public float Field26 { get; set; }
        public float Field27 { get; set; }
        public float Field28 { get; set; }
        public float Field29 { get; set; }
        public float Field30 { get; set; }
        public float Field31 { get; set; }
        public float Field32 { get; set; }
        public float Field33 { get; set; }
        public float Field34 { get; set; }
        public float Field35 { get; set; }
        public float Field36 { get; set; }
        public float Field37 { get; set; }
        public float Field38 { get; set; }
        public float Field39 { get; set; }
        public float Field40 { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            int usedTextureSlotCount = reader.ReadInt32();
            Field00 = reader.ReadInt32();
            Shader = reader.ReadString( StringBinaryFormat.FixedLength, 8 );
            Diffuse.Read( reader );
            Ambient.Read( reader );
            Normal.Read( reader );
            Specular.Read( reader );
            ToonCurve.Read( reader );
            Reflection.Read( reader );
            SpecularPower.Read( reader );
            Texture08.Read( reader );
            Field01 = reader.ReadInt32();
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
            Name = reader.ReadString( StringBinaryFormat.FixedLength, 64 );
            Field25 = reader.ReadSingle();
            Field26 = reader.ReadSingle();
            Field27 = reader.ReadSingle();
            Field28 = reader.ReadSingle();
            Field29 = reader.ReadSingle();
            Field30 = reader.ReadSingle();
            Field31 = reader.ReadSingle();
            Field32 = reader.ReadSingle();
            Field33 = reader.ReadSingle();
            Field34 = reader.ReadSingle();
            Field35 = reader.ReadSingle();
            Field36 = reader.ReadSingle();
            Field37 = reader.ReadSingle();
            Field38 = reader.ReadSingle();
            Field39 = reader.ReadSingle();
            Field40 = reader.ReadSingle();
        }

        internal void Write( EndianBinaryWriter writer )
        {
            int usedTextureSlotCount = 0;
            if ( Diffuse.TextureID > 0 )
                usedTextureSlotCount++;
            if ( Ambient.TextureID > 0 )
                usedTextureSlotCount++;
            if ( Normal.TextureID > 0 )
                usedTextureSlotCount++;
            if ( Specular.TextureID > 0 )
                usedTextureSlotCount++;
            if ( ToonCurve.TextureID > 0 )
                usedTextureSlotCount++;
            if ( Reflection.TextureID > 0 )
                usedTextureSlotCount++;
            if ( SpecularPower.TextureID > 0 )
                usedTextureSlotCount++;
            if ( Texture08.TextureID > 0 )
                usedTextureSlotCount++;

            writer.Write( usedTextureSlotCount );
            writer.Write( Field00 );
            writer.Write( Shader, StringBinaryFormat.FixedLength, 8 );
            Diffuse.Write( writer );
            Ambient.Write( writer );
            Normal.Write( writer );
            Specular.Write( writer );
            ToonCurve.Write( writer );
            Reflection.Write( writer );
            SpecularPower.Write( writer );
            Texture08.Write( writer );
            writer.Write( Field01 );
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
            writer.Write( Name, StringBinaryFormat.FixedLength, 64 );
            writer.Write( Field25 );
            writer.Write( Field26 );
            writer.Write( Field27 );
            writer.Write( Field28 );
            writer.Write( Field29 );
            writer.Write( Field30 );
            writer.Write( Field31 );
            writer.Write( Field32 );
            writer.Write( Field33 );
            writer.Write( Field34 );
            writer.Write( Field35 );
            writer.Write( Field36 );
            writer.Write( Field37 );
            writer.Write( Field38 );
            writer.Write( Field39 );
            writer.Write( Field40 );
        }

        public Material()
        {
            Diffuse = new MaterialTexture();
            Ambient = new MaterialTexture();
            Normal = new MaterialTexture();
            Specular = new MaterialTexture();
            ToonCurve = new MaterialTexture();
            Reflection = new MaterialTexture();
            SpecularPower = new MaterialTexture();
            Texture08 = new MaterialTexture();
        }
    }
}
