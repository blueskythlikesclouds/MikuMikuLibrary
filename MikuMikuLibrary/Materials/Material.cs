using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Misc;

namespace MikuMikuLibrary.Materials
{
    public class Material
    {
        public const int BYTE_SIZE = 0x4B0;

        public int Field00 { get; set; }
        public string Shader { get; set; }
        public MaterialTexture Diffuse { get; }
        public MaterialTexture Ambient { get; }
        public MaterialTexture Normal { get; }
        public MaterialTexture Specular { get; }
        public MaterialTexture ToonCurve { get; }
        public MaterialTexture Reflection { get; }
        public MaterialTexture Tangent { get; }
        public MaterialTexture Texture08 { get; }
        public int Field01 { get; set; }
        public int Field02 { get; set; }
        public Color DiffuseColor { get; set; }
        public Color AmbientColor { get; set; }
        public Color SpecularColor { get; set; }
        public Color EmissionColor { get; set; }
        public float Shininess { get; set; }
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

        public bool IsAlphaEnabled
        {
            get => ( Field02 & 1 ) != 0;
            set
            {
                Field00 = ( Field00 & ~2 ) | ( ( value ? 1 : 0 ) << 1 );
                Field02 = ( Field02 & ~1 ) | ( value ? 1 : 0 );
            }
        }

        public IEnumerable<MaterialTexture> MaterialTextures
        {
            get
            {
                yield return Diffuse;
                yield return Ambient;
                yield return Normal;
                yield return Specular;
                yield return ToonCurve;
                yield return Reflection;
                yield return Tangent;
                yield return Texture08;
            }
        }

        public IEnumerable<MaterialTexture> ActiveMaterialTextures =>
            MaterialTextures.Where( x => x.IsActive );

        internal void Read( EndianBinaryReader reader )
        {
            reader.SeekCurrent( 4 );
            Field00 = reader.ReadInt32();
            Shader = reader.ReadString( StringBinaryFormat.FixedLength, 8 );
            Diffuse.Read( reader );
            Ambient.Read( reader );
            Normal.Read( reader );
            Specular.Read( reader );
            ToonCurve.Read( reader );
            Reflection.Read( reader );
            Tangent.Read( reader );
            Texture08.Read( reader );
            Field01 = reader.ReadInt32();
            Field02 = reader.ReadInt32();
            DiffuseColor = reader.ReadColor();
            AmbientColor = reader.ReadColor();
            SpecularColor = reader.ReadColor();
            EmissionColor = reader.ReadColor();
            Shininess = reader.ReadSingle();
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
            writer.Write( ActiveMaterialTextures.Count() );
            writer.Write( Field00 );
            writer.Write( Shader, StringBinaryFormat.FixedLength, 8 );
            Diffuse.Write( writer );
            Ambient.Write( writer );
            Normal.Write( writer );
            Specular.Write( writer );
            ToonCurve.Write( writer );
            Reflection.Write( writer );
            Tangent.Write( writer );
            Texture08.Write( writer );
            writer.Write( Field01 );
            writer.Write( Field02 );
            writer.Write( DiffuseColor );
            writer.Write( AmbientColor );
            writer.Write( SpecularColor );
            writer.Write( EmissionColor );
            writer.Write( Shininess );
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
            Tangent = new MaterialTexture();
            Texture08 = new MaterialTexture();
        }
    }
}