using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MikuMikuLibrary.Sprites;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Mementos;

namespace MikuMikuModel.Nodes.Sprites
{
    public class SpriteNode : Node<Sprite>
    {
        public override NodeFlags Flags => NodeFlags.Rename | NodeFlags.Export;

        [Category( "General" )]
        [DisplayName( "Texture index" )]
        public uint TextureIndex
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Resolution mode" )]
        public ResolutionMode ResolutionMode
        {
            get => GetProperty<ResolutionMode>();
            set => SetProperty( value );
        }

        [Browsable( false )]
        public float NormalizedX
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Browsable( false )]
        public float NormalizedY
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }        
        
        [Browsable( false )]
        public float NormalizedWidth
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Browsable( false )]
        public float NormalizedHeight
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public float X
        {
            get => GetProperty<float>();
            set
            {
                MementoStack.BeginCompoundMemento();

                SetProperty( value );
                CalculateNormalizedValues();

                MementoStack.EndCompoundMemento();
            }
        }

        [Category( "General" )]
        public float Y
        {
            get => GetProperty<float>();
            set
            {
                MementoStack.BeginCompoundMemento();

                SetProperty( value );
                CalculateNormalizedValues();

                MementoStack.EndCompoundMemento();
            }
        }

        [Category( "General" )]
        public float Width
        {
            get => GetProperty<float>();
            set
            {
                MementoStack.BeginCompoundMemento();

                SetProperty( value );
                CalculateNormalizedValues();

                MementoStack.EndCompoundMemento();
            }
        }

        [Category( "General" )]
        public float Height
        {
            get => GetProperty<float>();
            set
            {
                MementoStack.BeginCompoundMemento();

                SetProperty( value );
                CalculateNormalizedValues();

                MementoStack.EndCompoundMemento();
            }
        }

        private void CalculateNormalizedValues()
        {
            var spriteSetNode = FindParent<SpriteSetNode>();
            var spriteSet = spriteSetNode.Data;

            if ( Data.TextureIndex >= spriteSet.TextureSet.Textures.Count )
                return;

            var texture = spriteSet.TextureSet.Textures[ ( int ) Data.TextureIndex ];

            NormalizedX = Data.X / texture.Width;
            NormalizedY = Data.Y / texture.Height;
            NormalizedWidth = Data.Width / texture.Width;
            NormalizedHeight = Data.Height / texture.Height;
        }

        protected override void Initialize()
        {
            AddExportHandler<Bitmap>( filePath =>
            {
                var imageFormat = ImageFormat.Png;

                if ( !string.IsNullOrEmpty( filePath ) )
                {
                    string extension = Path.GetExtension( filePath ).Trim( '.' ).ToLowerInvariant();

                    switch ( extension )
                    {
                        case "png":
                            imageFormat = ImageFormat.Png;
                            break;

                        case "jpg":
                        case "jpeg":
                            imageFormat = ImageFormat.Jpeg;
                            break;

                        case "gif":
                            imageFormat = ImageFormat.Gif;
                            break;

                        case "bmp":
                            imageFormat = ImageFormat.Bmp;
                            break;

                        default:
                            throw new ArgumentException( "Image format could not be detected", nameof( filePath ) );
                    }
                }

                using ( var bitmap = SpriteCropper.Crop( Data, FindParent<SpriteSetNode>().Data ) )
                    bitmap.Save( filePath, imageFormat );
            } );
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public SpriteNode( string name, Sprite data ) : base( name, data )
        {
        }

        public override Control Control
        {
            get
            {
                var spriteSetNode = FindParent<SpriteSetNode>();
                var spriteSet = spriteSetNode.Data;
                Bitmap cropped = SpriteCropper.Crop( Data, spriteSet );

                SpriteViewControl.Instance.SetBitmap( cropped );
                return SpriteViewControl.Instance;
            }
        }
    }
}