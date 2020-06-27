using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Materials;
using MikuMikuModel.Nodes.TypeConverters;
using MikuMikuModel.Resources;
using Color = MikuMikuLibrary.Misc.Color;

namespace MikuMikuModel.Nodes.Materials
{
    public class MaterialNode : Node<Material>
    {
        public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Rename | NodeFlags.Remove;
        public override Bitmap Image => ResourceStore.LoadBitmap( "Icons/Material.png" );

        [Category( "General" )]
        [DisplayName( "Flags" )]
        public MaterialFlags MaterialFlags
        {
            get => GetProperty<MaterialFlags>( nameof( Material.Flags ) );
            set => SetProperty( value, nameof( Material.Flags ) );
        }

        [Category( "General" )]
        [DisplayName( "Shader name" )]
        public string ShaderName
        {
            get => GetProperty<string>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Shader flags" )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint ShaderFlags
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Blend flags" )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint BlendFlags
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [TypeConverter( typeof( ColorTypeConverter ) )]
        public Color Diffuse
        {
            get => GetProperty<Color>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [TypeConverter( typeof( ColorTypeConverter ) )]
        public Color Ambient
        {
            get => GetProperty<Color>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [TypeConverter( typeof( ColorTypeConverter ) )]
        public Color Specular
        {
            get => GetProperty<Color>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [TypeConverter( typeof( ColorTypeConverter ) )]
        public Color Emission
        {
            get => GetProperty<Color>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public float Shininess
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public float Intensity
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Reserved sphere" )]
        public BoundingSphere ReservedSphere
        {
            get => GetProperty<BoundingSphere>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Bump depth" )]
        public float BumpDepth
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "Shader flags" )]
        [DisplayName( "Vertex translation type" )]
        public VertexTranslationType VertexTranslationType
        {
            get => GetProperty<VertexTranslationType>();
            set => SetProperty( value );
        }

        [Category( "Shader flags" )]
        [DisplayName( "Color source type" )]
        public ColorSourceType ColorSourceType
        {
            get => GetProperty<ColorSourceType>();
            set => SetProperty( value );
        }

        [Category( "Shader flags" )]
        [DisplayName( "Lambert shading" )]
        public bool LambertShading
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Shader flags" )]
        [DisplayName( "Phong shading" )]
        public bool PhongShading
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Shader flags" )]
        [DisplayName( "Per pixel shading" )]
        public bool PerPixelShading
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Shader flags" )]
        [DisplayName( "Double shading" )]
        public bool DoubleShading
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Shader flags" )]
        [DisplayName( "Bump map type" )]
        public BumpMapType BumpMapType
        {
            get => GetProperty<BumpMapType>();
            set => SetProperty( value );
        }

        [Category( "Shader flags" )]
        public uint Fresnel
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Shader flags" )]
        [DisplayName( "Line light" )]
        public uint LineLight
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Shader flags" )]
        [DisplayName( "Receive shadow" )]
        public bool ReceiveShadow
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Shader flags" )]
        [DisplayName( "Cast shadow" )]
        public bool CastShadow
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Shader flags" )]
        [DisplayName( "Specular quality" )]
        public SpecularQuality SpecularQuality
        {
            get => GetProperty<SpecularQuality>();
            set => SetProperty( value );
        }

        [Category( "Shader flags" )]
        [DisplayName( "Anisotropic direction" )]
        public AnisoDirection AnisoDirection
        {
            get => GetProperty<AnisoDirection>();
            set => SetProperty( value );
        }

        [Category( "Blend flags" )]
        [DisplayName( "Alpha texture" )]
        public bool AlphaTexture
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Blend flags" )]
        [DisplayName( "Alpha material" )]
        public bool AlphaMaterial
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Blend flags" )]
        [DisplayName( "Punch through" )]
        public bool PunchThrough
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Blend flags" )]
        [DisplayName( "Double sided" )]
        public bool DoubleSided
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Blend flags" )]
        [DisplayName( "Normal direction light" )]
        public bool NormalDirectionLight
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Blend flags" )]
        [DisplayName( "Source blend factor" )]
        public BlendFactor SrcBlendFactor
        {
            get => GetProperty<BlendFactor>();
            set => SetProperty( value );
        }

        [Category( "Blend flags" )]
        [DisplayName( "Destination blend factor" )]
        public BlendFactor DstBlendFactor
        {
            get => GetProperty<BlendFactor>();
            set => SetProperty( value );
        }

        [Category( "Blend flags" )]
        [DisplayName( "Blend operation" )]
        public uint BlendOperation
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Blend flags" )]
        [DisplayName( "Z bias" )]
        public uint ZBias
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Blend flags" )]
        [DisplayName( "No fog" )]
        public bool NoFog
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
            AddCustomHandler( "Add material texture", () =>
            {
                var materialTextureNode = new MaterialTextureNode( "None", new MaterialTexture { RepeatU = true, RepeatV = true } );

                if ( !materialTextureNode.PromptTextureSelector( this ) )
                    return;

                if ( !IsPopulated )
                    Populate();

                Nodes.Add( materialTextureNode );
            } );
            AddCustomHandlerSeparator();
            AddCustomHandler( "Enable transparency", () =>
            {
                MaterialFlags |= MaterialFlags.ColorAlpha;
                AlphaTexture = true;
            } );
            AddCustomHandler( "Enable punch through transparency", () =>
            {
                MaterialFlags |= MaterialFlags.ColorAlpha;
                AlphaTexture = true;
                PunchThrough = true;
            } );
        }

        protected override void PopulateCore()
        {
            foreach ( var materialTexture in Data.MaterialTextures )
            {
                if ( materialTexture.Type == MaterialTextureType.None )
                    continue;

                Nodes.Add( new MaterialTextureNode( Enum.GetName( typeof( MaterialTextureType ), materialTexture.Type ), materialTexture ) );
            }
        }

        protected override void SynchronizeCore()
        {
            if ( Data.MaterialTextures.Any( x => Nodes.All( y => y.Data != x ) ) )
            {
                int i = 0;

                for ( int j = 0; i < Data.MaterialTextures.Length && j < Nodes.Count; j++ )
                {
                    var materialTextureNode = ( MaterialTextureNode ) Nodes[ j ];

                    if ( materialTextureNode.Type == MaterialTextureType.None )
                        continue;

                    Data.MaterialTextures[ i++ ] = materialTextureNode.Data;
                }

                for ( ; i < Data.MaterialTextures.Length; i++ )
                    Data.MaterialTextures[ i ] = null;

                Data.SortMaterialTextures();
            }

            const MaterialFlags environmentFlags =
                MaterialFlags.Environment | MaterialFlags.ColorL1Alpha | MaterialFlags.ColorL2Alpha | MaterialFlags.OverrideIBL;

            ApplyFlags( MaterialTextureType.Color, MaterialFlags.Color );
            ApplyFlags( MaterialTextureType.Normal, MaterialFlags.Normal );
            ApplyFlags( MaterialTextureType.Specular, MaterialFlags.Specular );
            ApplyFlags( MaterialTextureType.Reflection, environmentFlags );
            ApplyFlags( MaterialTextureType.Translucency, MaterialFlags.Translucency );
            ApplyFlags( MaterialTextureType.Transparency, MaterialFlags.Transparency );
            ApplyFlags( MaterialTextureType.EnvironmentSphere, environmentFlags );
            ApplyFlags( MaterialTextureType.EnvironmentCube, environmentFlags );

            void ApplyFlags( MaterialTextureType type, MaterialFlags flags )
            {
                if ( Data.MaterialTextures.Any( x => x.Type == type ) )
                    Data.Flags |= flags;

                else
                    Data.Flags &= ~( flags );
            }
        }

        public MaterialNode( string name, Material data ) : base( name, data )
        {
        }
    }
}