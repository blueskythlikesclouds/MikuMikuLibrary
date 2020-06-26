using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using System.Xml.Serialization;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Textures;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.GUI.Forms;
using MikuMikuModel.Mementos;
using MikuMikuModel.Nodes.Objects;
using MikuMikuModel.Nodes.TypeConverters;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.Materials
{
    public class MaterialTextureNode : Node<MaterialTexture>
    {
        private static readonly XmlSerializer sSerializer = new XmlSerializer( typeof( MaterialTexture ) );

        public override NodeFlags Flags => NodeFlags.Rename;
        public override Bitmap Image => ResourceStore.LoadBitmap( "Icons/MaterialTexture.png" );

        public override Control Control
        {
            get
            {
                var textureSetNode = FindParent<ObjectSetNode>()?.FindNode<INode>( "Texture Set", true );
                var texture = (( TextureSet ) textureSetNode?.Data)?.Textures.FirstOrDefault( x => x.Id == TextureId );

                if ( texture == null )
                    return null;

                TextureViewControl.Instance.SetTexture( texture );
                return TextureViewControl.Instance;
            }
        }

        [Category( "General" )]
        [DisplayName( "Sampler flags" )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint SamplerFlags
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Texture id" )]
        [TypeConverter( typeof( IdTypeConverter ) )]
        public uint TextureId
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Texture flags" )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint TextureFlags
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Extra shader name" )]
        public string ExtraShaderName
        {
            get => GetProperty<string>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public float Weight
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Texture coordinate matrix" )]
        public Matrix4x4 TextureCoordinateMatrix
        {
            get => GetProperty<Matrix4x4>();
            set => SetProperty( value );
        }

        [Category( "Sampler flags" )]
        [DisplayName( "Repeat U" )]
        public bool RepeatU
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Sampler flags" )]
        [DisplayName( "Repeat V" )]
        public bool RepeatV
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Sampler flags" )]
        [DisplayName( "Mirror U" )]
        public bool MirrorU
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Sampler flags" )]
        [DisplayName( "Mirror V" )]
        public bool MirrorV
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Sampler flags" )]
        [DisplayName( "Ignore alpha" )]
        public bool IgnoreAlpha
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Sampler flags" )]
        public uint Blend
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Sampler flags" )]
        [DisplayName( "Alpha blend" )]
        public uint AlphaBlend
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Sampler flags" )]
        public bool Border
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Sampler flags" )]
        [DisplayName( "Clamp to edge" )]
        public bool ClampToEdge
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        [Category( "Sampler flags" )]
        public uint Filter
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Sampler flags" )]
        [DisplayName( "Mipmap" )]
        public uint MipMap
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Sampler flags" )]
        [DisplayName( "Mipmap bias" )]
        public uint MipMapBias
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Sampler flags" )]
        [DisplayName( "Anisotropic filter" )]
        public uint AnisotropicFilter
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Texture flags" )]
        public MaterialTextureType Type
        {
            get => GetProperty<MaterialTextureType>();
            set
            {
                var previousType = Type;

                MementoStack.BeginCompoundMemento();
                
                SetProperty( value );

                if ( previousType == Type || Type == MaterialTextureType.None )
                {
                    MementoStack.EndCompoundMemento();
                    return;
                }

                Name = Enum.GetName( typeof( MaterialTextureType ), Type );
                Blend = Blend == 0 ? Type == MaterialTextureType.Specular ? 1u : 7 : Blend;
                Filter = Filter == 0 ? 2 : Filter;
                MipMap = MipMap == 0 ? 2 : MipMap;

                MementoStack.EndCompoundMemento();
            }
        }

        [Category( "Texture flags" )]
        [DisplayName( "Texture coordinate index" )]
        public uint TextureCoordinateIndex
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Texture flags" )]
        [DisplayName( "Texture coordinate translation type" )]
        public MaterialTextureCoordinateTranslationType TextureCoordinateTranslationType
        {
            get => GetProperty<MaterialTextureCoordinateTranslationType>();
            set => SetProperty( value );
        }

        public bool PromptTextureSelector( INode node )
        {
            var textureSetNode = node.FindParent<ObjectSetNode>().FindNode<INode>( "Texture Set", true );

            if ( textureSetNode == null )
            {
                MessageBox.Show( "Could not find a suitable texture set.", Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error );
                return false;
            }

            using ( var textureSelectForm = new TextureSelectForm( textureSetNode, Type != MaterialTextureType.None ? Type : MaterialTextureType.Color ) )
            {
                if ( textureSelectForm.ShowDialog() != DialogResult.OK )
                    return false;

                TextureId = textureSelectForm.SelectedTextureNode.Id;
                Type = textureSelectForm.MaterialTextureType;

                if ( textureSelectForm.SelectedTextureNode.Data.ArraySize == 6 )
                {
                    TextureCoordinateTranslationType = MaterialTextureCoordinateTranslationType.Cube;

                    if ( Type == MaterialTextureType.Reflection )
                        Type = MaterialTextureType.EnvironmentCube;
                }

                else if ( Type == MaterialTextureType.EnvironmentSphere )
                {
                    TextureCoordinateTranslationType = MaterialTextureCoordinateTranslationType.Sphere;

                    if ( Type == MaterialTextureType.Reflection )
                        Type = MaterialTextureType.EnvironmentSphere;
                }

                else
                    TextureCoordinateTranslationType = MaterialTextureCoordinateTranslationType.None;
            }

            return true;
        }

        protected override void Initialize()
        {
            AddCustomHandler( "Replace", () => PromptTextureSelector( this ) );
            AddCustomHandlerSeparator();
            AddCustomHandler( "Copy values", () =>
            {
                using ( var stringWriter = new StringWriter( CultureInfo.InvariantCulture ) )
                {
                    sSerializer.Serialize( stringWriter, Data );
                    Clipboard.SetText( stringWriter.ToString() );
                }
            }, Keys.Control | Keys.C );
            AddCustomHandler( "Paste values", () =>
            {
                try
                {
                    using ( var stringReader = new StringReader( Clipboard.GetText() ) )
                    {
                        var materialTexture = ( MaterialTexture ) sSerializer.Deserialize( stringReader );

                        SetProperty( materialTexture.SamplerFlags, nameof( MaterialTexture.SamplerFlags ) );
                        SetProperty( materialTexture.TextureFlags, nameof( MaterialTexture.TextureFlags ) );
                        SetProperty( materialTexture.ExtraShaderName, nameof( MaterialTexture.ExtraShaderName ) );
                        SetProperty( materialTexture.Weight, nameof( MaterialTexture.Weight ) );
                        SetProperty( materialTexture.TextureCoordinateMatrix, nameof( MaterialTexture.TextureCoordinateMatrix ) );
                    }
                }
                catch
                {
                    MessageBox.Show( "There is no valid data to paste.", Program.Name, MessageBoxButtons.OK,
                        MessageBoxIcon.Error );
                }
            }, Keys.Control | Keys.V );
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public MaterialTextureNode( string name, MaterialTexture data ) : base( name, data )
        {
        }
    }
}