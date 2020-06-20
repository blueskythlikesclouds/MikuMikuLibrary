using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Misc;

namespace MikuMikuLibrary.Materials
{
    [Flags]
    public enum MaterialFlags
    {
        Color = 1 << 0,
        ColorAlpha = 1 << 1,
        ColorL1 = 1 << 2,
        ColorL1Alpha = 1 << 3,
        ColorL2 = 1 << 4,
        ColorL2Alpha = 1 << 5,
        Transparency = 1 << 6,
        Specular = 1 << 7,
        Normal = 1 << 8,
        NormalAlt = 1 << 9,
        Environment = 1 << 10,
        ColorL3 = 1 << 11,
        ColorL3Alpha = 1 << 12,
        Translucency = 1 << 13,
        Flag14 = 1 << 14,
        OverrideIBL = 1 << 15
    }

    public enum VertexTranslationType
    {
        Default = 0,
        Envelope = 1,
        Morphing = 2
    }

    public enum ColorSourceType
    {
        MaterialColor = 0,
        VertexColor = 1,
        VertexMorph = 2,
    }

    public enum BumpMapType
    {
        None = 0,
        Dot = 1,
        Env = 2
    }

    public enum SpecularQuality
    {
        Low = 0,
        High = 1
    }

    public enum AnisoDirection
    {
        Normal = 0,
        U = 1,
        V = 2,
        Radial = 3
    }

    public enum BlendFactor
    {
        Zero = 0,
        One = 1,
        SrcColor = 2,
        InverseSrcColor = 3,
        SrcAlpha = 4,
        InverseSrcAlpha = 5,
        DstAlpha = 6,
        InverseDstAlpha = 7,
        DstColor = 8,
        InverseDstColor = 9
    }

    public class Material
    {
        public const int BYTE_SIZE = 0x4B0;

        public static readonly IReadOnlyList<string> ShaderNames = new[]
        {
            "BLINN",
            "CHARA",
            "CLOTH",
            "EYEBALL",
            "FLOOR",
            "HAIR",
            "ITEM",
            "PUDDLE",
            "SKIN",
            "SKY",
            "STAGE",
            "TIGHTS",
            "WATER01"
        };

        public MaterialFlags Flags { get; set; }
        public string ShaderName { get; set; }
        public uint ShaderFlags { get; set; }
        public MaterialTexture[] MaterialTextures { get; }
        public uint BlendFlags { get; set; }
        public Color Diffuse { get; set; }
        public Color Ambient { get; set; }
        public Color Specular { get; set; }
        public Color Emission { get; set; }
        public float Shininess { get; set; }
        public float Intensity { get; set; }
        public BoundingSphere ReservedSphere { get; set; }
        public string Name { get; set; }
        public float BumpDepth { get; set; }

        public VertexTranslationType VertexTranslationType
        {
            get => ( VertexTranslationType ) BitHelper.Unpack( ShaderFlags, 0, 2 );
            set => ShaderFlags = BitHelper.Pack( ShaderFlags, ( uint ) value, 0, 2 );
        }

        public ColorSourceType ColorSourceType
        {
            get => ( ColorSourceType ) BitHelper.Unpack( ShaderFlags, 2, 4 );
            set => ShaderFlags = BitHelper.Pack( ShaderFlags, ( uint ) value, 2, 4 );
        }

        public bool LambertShading
        {
            get => BitHelper.Unpack( ShaderFlags, 4, 5 ) != 0;
            set => ShaderFlags = BitHelper.Pack( ShaderFlags, value ? 1u : 0, 4, 5 );
        }

        public bool PhongShading
        {
            get => BitHelper.Unpack( ShaderFlags, 5, 6 ) != 0;
            set => ShaderFlags = BitHelper.Pack( ShaderFlags, value ? 1u : 0, 5, 6 );
        }

        public bool PerPixelShading
        {
            get => BitHelper.Unpack( ShaderFlags, 6, 7 ) != 0;
            set => ShaderFlags = BitHelper.Pack( ShaderFlags, value ? 1u : 0, 6, 7 );
        }

        public bool DoubleShading
        {
            get => BitHelper.Unpack( ShaderFlags, 7, 8 ) != 0;
            set => ShaderFlags = BitHelper.Pack( ShaderFlags, value ? 1u : 0, 7, 8 );
        }

        public BumpMapType BumpMapType
        {
            get => ( BumpMapType ) BitHelper.Unpack( ShaderFlags, 8, 10 );
            set => ShaderFlags = BitHelper.Pack( ShaderFlags, ( uint ) value, 8, 10 );
        }

        public uint Fresnel
        {
            get => BitHelper.Unpack( ShaderFlags, 10, 14 );
            set => ShaderFlags = BitHelper.Pack( ShaderFlags, value, 10, 14 );
        }

        public uint LineLight
        {
            get => BitHelper.Unpack( ShaderFlags, 14, 18 );
            set => ShaderFlags = BitHelper.Pack( ShaderFlags, value, 14, 18 );
        }

        public bool ReceiveShadow
        {
            get => BitHelper.Unpack( ShaderFlags, 18, 19 ) != 0;
            set => ShaderFlags = BitHelper.Pack( ShaderFlags, value ? 1u : 0, 18, 19 );
        }

        public bool CastShadow
        {
            get => BitHelper.Unpack( ShaderFlags, 19, 20 ) != 0;
            set => ShaderFlags = BitHelper.Pack( ShaderFlags, value ? 1u : 0, 19, 20 );
        }

        public SpecularQuality SpecularQuality
        {
            get => ( SpecularQuality ) BitHelper.Unpack( ShaderFlags, 20, 21 );
            set => ShaderFlags = BitHelper.Pack( ShaderFlags, ( uint ) value, 20, 21 );
        }

        public AnisoDirection AnisoDirection
        {
            get => ( AnisoDirection ) BitHelper.Unpack( ShaderFlags, 21, 24 );
            set => ShaderFlags = BitHelper.Pack( ShaderFlags, ( uint ) value, 21, 24 );
        }

        public bool AlphaTexture
        {
            get => BitHelper.Unpack( BlendFlags, 0, 1 ) != 0;
            set => BlendFlags = BitHelper.Pack( BlendFlags, value ? 1u : 0, 0, 1 );
        }

        public bool AlphaMaterial
        {
            get => BitHelper.Unpack( BlendFlags, 1, 2 ) != 0;
            set => BlendFlags = BitHelper.Pack( BlendFlags, value ? 1u : 0, 1, 2 );
        }

        public bool PunchThrough
        {
            get => BitHelper.Unpack( BlendFlags, 2, 3 ) != 0;
            set => BlendFlags = BitHelper.Pack( BlendFlags, value ? 1u : 0, 2, 3 );
        }

        public bool DoubleSided
        {
            get => BitHelper.Unpack( BlendFlags, 3, 4 ) != 0;
            set => BlendFlags = BitHelper.Pack( BlendFlags, value ? 1u : 0, 3, 4 );
        }

        public bool NormalDirectionLight
        {
            get => BitHelper.Unpack( BlendFlags, 4, 5 ) != 0;
            set => BlendFlags = BitHelper.Pack( BlendFlags, value ? 1u : 0, 4, 5 );
        }

        public BlendFactor SrcBlendFactor
        {
            get => ( BlendFactor ) BitHelper.Unpack( BlendFlags, 5, 9 );
            set => BlendFlags = BitHelper.Pack( BlendFlags, ( uint ) value, 5, 9 );
        }

        public BlendFactor DstBlendFactor
        {
            get => ( BlendFactor ) BitHelper.Unpack( BlendFlags, 9, 13 );
            set => BlendFlags = BitHelper.Pack( BlendFlags, ( uint ) value, 9, 13 );
        }

        public uint BlendOperation
        {
            get => BitHelper.Unpack( BlendFlags, 13, 16 );
            set => BlendFlags = BitHelper.Pack( BlendFlags, value, 13, 16 );
        }

        public uint ZBias
        {
            get => BitHelper.Unpack( BlendFlags, 16, 20 );
            set => BlendFlags = BitHelper.Pack( BlendFlags, value, 16, 20 );
        }

        public bool NoFog
        {
            get => BitHelper.Unpack( BlendFlags, 20, 21 ) != 0;
            set => BlendFlags = BitHelper.Pack( BlendFlags, value ? 1u : 0, 20, 21 );
        }

        public void ResetMaterialTextures()
        {
            for ( int i = 0; i < MaterialTextures.Length; i++ )
                MaterialTextures[ i ] = new MaterialTexture();
        }

        public void SortMaterialTextures()
        {
            var sortedMaterialTextures = new MaterialTexture[ 8 ];

            foreach ( var materialTexture in MaterialTextures )
            {
                if ( materialTexture == null || !MaterialTexture.PreferredIndices.TryGetValue( materialTexture.Type, out var indices ) )
                    continue;

                foreach ( int index in indices )
                {
                    if ( sortedMaterialTextures[ index ] != null )
                        continue;

                    sortedMaterialTextures[ index ] = materialTexture;
                    break;
                }
            }

            // Sequentially assign material textures that couldn't find a suitable place
            foreach ( var materialTexture in MaterialTextures.Except( sortedMaterialTextures ) )
            {
                for ( int i = 0; i < sortedMaterialTextures.Length; i++ )
                {
                    if ( sortedMaterialTextures[ i ] != null )
                        continue;

                    sortedMaterialTextures[ i ] = materialTexture;
                    break;
                }
            }

            // Fill null slots
            for ( int i = 0; i < sortedMaterialTextures.Length; i++ )
            {
                if ( sortedMaterialTextures[ i ] == null )
                    sortedMaterialTextures[ i ] = new MaterialTexture();
            }
            
            // Now copy 'em over
            Array.Copy( sortedMaterialTextures, 0, MaterialTextures, 0, MaterialTextures.Length );
        }

        internal void Read( EndianBinaryReader reader )
        {
            reader.SeekCurrent( 4 );
            Flags = ( MaterialFlags ) reader.ReadInt32();
            ShaderName = reader.ReadString( StringBinaryFormat.FixedLength, 8 );
            ShaderFlags = reader.ReadUInt32();

            for ( int i = 0; i < 8; i++ )
            {
                MaterialTextures[ i ] = new MaterialTexture();
                MaterialTextures[ i ].Read( reader );
            }

            BlendFlags = reader.ReadUInt32();
            Diffuse = reader.ReadColor();
            Ambient = reader.ReadColor();
            Specular = reader.ReadColor();
            Emission = reader.ReadColor();
            Shininess = reader.ReadSingle();
            Intensity = reader.ReadSingle();
            ReservedSphere = reader.ReadBoundingSphere();
            Name = reader.ReadString( StringBinaryFormat.FixedLength, 64 );
            BumpDepth = reader.ReadSingle();
            reader.SkipNulls( 15 * sizeof( float ) );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( MaterialTextures.Count( x => x.Type != MaterialTextureType.None ) );
            writer.Write( ( int ) Flags );
            writer.Write( ShaderName, StringBinaryFormat.FixedLength, 8 );
            writer.Write( ShaderFlags );

            for ( int i = 0; i < 8; i++ )
                MaterialTextures[ i ].Write( writer );

            writer.Write( BlendFlags );
            writer.Write( Diffuse );
            writer.Write( Ambient );
            writer.Write( Specular );
            writer.Write( Emission );
            writer.Write( Shininess );
            writer.Write( Intensity );
            writer.Write( ReservedSphere );
            writer.Write( Name, StringBinaryFormat.FixedLength, 64 );
            writer.Write( BumpDepth );
            writer.WriteNulls( 15 * sizeof( float ) );
        }

        public Material()
        {
            ShaderName = "BLINN";
            MaterialTextures = new MaterialTexture[ 8 ];
            ShaderFlags = 0x70;

            for ( int i = 0; i < MaterialTextures.Length; i++ )
                MaterialTextures[ i ] = new MaterialTexture();

            BlendFlags = 0xA80;
            Diffuse = new Color( 1, 1, 1, 1 );
            Ambient = new Color( 1, 1, 1, 1 );
            Specular = new Color( 0.5f, 0.5f, 0.5f, 1 );
            Emission = new Color( 0, 0, 0, 1 );
            Shininess = 20;
            BumpDepth = 1;
        }
    }
}