using System.Collections.Generic;
using System.Numerics;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Materials
{
    public enum MaterialTextureType
    {
        None = 0,
        Color = 1,
        Normal = 2,
        Specular = 3,
        Height = 4,
        Reflection = 5,
        Translucency = 6,
        Transparency = 7,
        EnvironmentSphere = 8,
        EnvironmentCube = 9
    }

    public enum MaterialTextureCoordinateTranslationType
    {
        None = 0,
        UV = 1,
        Sphere = 2,
        Cube = 3
    }

    public class MaterialTexture
    {
        public static readonly IReadOnlyDictionary<MaterialTextureType, int[]> PreferredIndices =
            new Dictionary<MaterialTextureType, int[]>
            {
                { MaterialTextureType.Color, new[] { 0, 1, 4 } },
                { MaterialTextureType.Normal, new[] { 2 } },
                { MaterialTextureType.Specular, new[] { 3 } },
                { MaterialTextureType.Reflection, new[] { 5 } },
                { MaterialTextureType.Translucency, new[] { 6 } },
                { MaterialTextureType.Transparency, new[] { 4 } },
                { MaterialTextureType.EnvironmentSphere, new[] { 5 } },
                { MaterialTextureType.EnvironmentCube, new[] { 5 } }
            };

        public uint SamplerFlags { get; set; }
        public uint TextureId { get; set; }
        public uint TextureFlags { get; set; }
        public string ExtraShaderName { get; set; }
        public float Weight { get; set; }
        public Matrix4x4 TextureCoordinateMatrix { get; set; }

        public bool RepeatU
        {
            get => BitHelper.Unpack( SamplerFlags, 0, 1 ) != 0;
            set => SamplerFlags = BitHelper.Pack( SamplerFlags, value ? 1u : 0, 0, 1 );
        }

        public bool RepeatV
        {
            get => BitHelper.Unpack( SamplerFlags, 1, 2 ) != 0;
            set => SamplerFlags = BitHelper.Pack( SamplerFlags, value ? 1u : 0, 1, 2 );
        }

        public bool MirrorU
        {
            get => BitHelper.Unpack( SamplerFlags, 2, 3 ) != 0;
            set => SamplerFlags = BitHelper.Pack( SamplerFlags, value ? 1u : 0, 2, 3 );
        }

        public bool MirrorV
        {
            get => BitHelper.Unpack( SamplerFlags, 3, 4 ) != 0;
            set => SamplerFlags = BitHelper.Pack( SamplerFlags, value ? 1u : 0, 3, 4 );
        }

        public bool IgnoreAlpha
        {
            get => BitHelper.Unpack( SamplerFlags, 4, 5 ) != 0;
            set => SamplerFlags = BitHelper.Pack( SamplerFlags, value ? 1u : 0, 4, 5 );
        }

        public uint Blend
        {
            get => BitHelper.Unpack( SamplerFlags, 5, 10 );
            set => SamplerFlags = BitHelper.Pack( SamplerFlags, value, 5, 10 );
        }

        public uint AlphaBlend
        {
            get => BitHelper.Unpack( SamplerFlags, 10, 15 );
            set => SamplerFlags = BitHelper.Pack( SamplerFlags, value, 10, 15 );
        }

        public bool Border
        {
            get => BitHelper.Unpack( SamplerFlags, 15, 16 ) != 0;
            set => SamplerFlags = BitHelper.Pack( SamplerFlags, value ? 1u : 0, 15, 16 );
        }

        public bool ClampToEdge
        {
            get => BitHelper.Unpack( SamplerFlags, 16, 17 ) != 0;
            set => SamplerFlags = BitHelper.Pack( SamplerFlags, value ? 1u : 0, 16, 17 );
        }

        public uint Filter
        {
            get => BitHelper.Unpack( SamplerFlags, 17, 20 );
            set => SamplerFlags = BitHelper.Pack( SamplerFlags, value, 17, 20 );
        }

        public uint MipMap
        {
            get => BitHelper.Unpack( SamplerFlags, 20, 22 );
            set => SamplerFlags = BitHelper.Pack( SamplerFlags, value, 20, 22 );
        }

        public uint MipMapBias
        {
            get => BitHelper.Unpack( SamplerFlags, 22, 30 );
            set => SamplerFlags = BitHelper.Pack( SamplerFlags, value, 22, 30 );
        }

        public uint AnisotropicFilter
        {
            get => BitHelper.Unpack( SamplerFlags, 30, 32 );
            set => SamplerFlags = BitHelper.Pack( SamplerFlags, value, 30, 32 );
        }

        public MaterialTextureType Type
        {
            get => ( MaterialTextureType ) BitHelper.Unpack( TextureFlags, 0, 4 );
            set => TextureFlags = BitHelper.Pack( TextureFlags, ( uint ) value, 0, 4 );
        }

        public uint TextureCoordinateIndex
        {
            get => BitHelper.Unpack( TextureFlags, 4, 8 );
            set => TextureFlags = BitHelper.Pack( TextureFlags, value, 4, 8 );
        }

        public MaterialTextureCoordinateTranslationType TextureCoordinateTranslationType
        {
            get => ( MaterialTextureCoordinateTranslationType ) BitHelper.Unpack( TextureFlags, 8, 11 );
            set => TextureFlags = BitHelper.Pack( TextureFlags, ( uint ) value, 8, 11 );
        }

        internal void Read( EndianBinaryReader reader )
        {
            SamplerFlags = reader.ReadUInt32();
            TextureId = reader.ReadUInt32();
            TextureFlags = reader.ReadUInt32();
            ExtraShaderName = reader.ReadString( StringBinaryFormat.FixedLength, 8 );
            Weight = reader.ReadSingle();
            TextureCoordinateMatrix = reader.ReadMatrix4x4();
            reader.SkipNulls( 8 * sizeof( float ) );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( SamplerFlags );
            writer.Write( TextureId );
            writer.Write( TextureFlags );
            writer.Write( ExtraShaderName, StringBinaryFormat.FixedLength, 8 );
            writer.Write( Weight );
            writer.Write( TextureCoordinateMatrix );
            writer.WriteNulls( 8 * sizeof( float ) );
        }

        public MaterialTexture()
        {
            TextureId = 0xFFFFFFFF;
            TextureFlags = 0xF0;
            Weight = 1.0f;
            TextureCoordinateMatrix = Matrix4x4.Identity;
        }
    }
}