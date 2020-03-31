using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MikuMikuLibrary.Sprites;
using MikuMikuModel.GUI.Controls;

namespace MikuMikuModel.Nodes.Sprites
{
    public class SpriteNode : Node<Sprite>
    {
        public override NodeFlags Flags => NodeFlags.Rename | NodeFlags.Export;

        [DisplayName( "Texture index" )]
        public int TextureIndex
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        [DisplayName( "Resolution mode" )]
        public ResolutionMode ResolutionMode
        {
            get => GetProperty<ResolutionMode>();
            set => SetProperty( value );
        }

        public float X
        {
            get => GetProperty<float>();
            set
            {
                SetProperty( value );
                CalculateNdcValues();
            }
        }

        public float Y
        {
            get => GetProperty<float>();
            set
            {
                SetProperty( value );
                CalculateNdcValues();
            }
        }

        public float Width
        {
            get => GetProperty<float>();
            set
            {
                SetProperty( value );
                CalculateNdcValues();
            }
        }

        public float Height
        {
            get => GetProperty<float>();
            set
            {
                SetProperty( value );
                CalculateNdcValues();
            }
        }

        private void CalculateNdcValues()
        {
            var spriteSetNode = FindParent<SpriteSetNode>();
            var spriteSet = spriteSetNode.Data;

            if ( Data.TextureIndex >= spriteSet.TextureSet.Textures.Count )
                return;

            var texture = spriteSet.TextureSet.Textures[ Data.TextureIndex ];

            Data.NdcX = Data.X / texture.Width;
            Data.NdcY = Data.Y / texture.Height;
            Data.NdcWidth = Data.Width / texture.Width;
            Data.NdcHeight = Data.Height / texture.Height;
        }

        protected override void Initialize()
        {
            RegisterExportHandler<Bitmap>(filePath =>
            {
                var imageFormat = ImageFormat.Png;

                if (!string.IsNullOrEmpty(filePath))
                {
                    string extension = Path.GetExtension(filePath).Trim('.').ToLowerInvariant();
                    switch (extension)
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
                            throw new ArgumentException("Image format could not be detected", nameof(filePath));
                    }
                }

                using (Bitmap obitmap = SpriteCropper.Crop(Data, FindParent<SpriteSetNode>().Data)) obitmap.Save(filePath, imageFormat);
            });
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
                Bitmap cropped = SpriteCropper.Crop(Data, spriteSet);

                SpriteViewControl.Instance.SetBitmap(cropped);
                return SpriteViewControl.Instance;
            }
        }
    }
}