using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.DDS;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Modules;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.Textures
{
    public class TextureNode : Node<Texture>
    {
        public override NodeFlags Flags => NodeFlags.Rename | NodeFlags.Export | NodeFlags.Replace;
        public override Bitmap Image => ResourceStore.LoadBitmap( "Icons/Texture.png" );

        public override Control Control
        {
            get
            {
                TextureViewControl.Instance.SetTexture( Data );
                return TextureViewControl.Instance;
            }
        }

        public int Id
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        public int Width => GetProperty<int>();
        public int Height => GetProperty<int>();
        public TextureFormat Format => GetProperty<TextureFormat>();

        private Texture ReplaceBitmap(string filePath, bool flipped=false)
        {
            using (var bitmap = new Bitmap(filePath))
            {
                if (flipped)
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

                bool hasTransparency = DDSCodec.HasTransparency(bitmap);

                if (Data.IsYCbCr)
                    return TextureEncoder.Encode(bitmap,
                        hasTransparency ? TextureFormat.RGBA : TextureFormat.RGB, false);

                var format =
                    Data.Format == TextureFormat.DXT1 ||
                    Data.Format == TextureFormat.DXT3 ||
                    Data.Format == TextureFormat.DXT5 ? hasTransparency
                        ? TextureFormat.DXT5
                        : TextureFormat.DXT1 :
                    Data.Format == TextureFormat.RGB ||
                    Data.Format == TextureFormat.RGBA ? hasTransparency
                        ? TextureFormat.RGBA
                        : TextureFormat.RGB : Data.Format;



                return TextureEncoder.Encode(bitmap, format, Data.MipMapCount != 0);
            }
        }

        private ImageFormat GetImageFormat(string filePath)
        {
            string extension = Path.GetExtension(filePath).Trim('.').ToLowerInvariant();
            switch (extension)
            {
                case "png":
                    return ImageFormat.Png;

                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;

                case "gif":
                    return ImageFormat.Gif;

                case "bmp":
                    return ImageFormat.Bmp;

                default:
                    throw new ArgumentException("Image format could not be detected", nameof(filePath));
            }
        }

        protected override void Initialize()
        {
            RegisterExportHandler<Bitmap>( filePath => TextureDecoder.DecodeToPNG( Data, filePath ) );
            RegisterExportHandler<Texture>( filePath => TextureDecoder.DecodeToDDS( Data, filePath ) );
            RegisterReplaceHandler<Texture>( TextureEncoder.Encode );
            RegisterReplaceHandler<Bitmap>( filePath => ReplaceBitmap(filePath) );
            RegisterCustomHandler("Export flipped", () =>
            {
                string filePath = ModuleExportUtilities.SelectModuleExport<Bitmap>();
                if (!string.IsNullOrEmpty(filePath))
                {
                    var imageFormat = GetImageFormat(filePath);

                    using (Bitmap oBitmap = TextureDecoder.Decode(Data))
                    {
                        oBitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                        oBitmap.Save(filePath, imageFormat);
                    }
                }
            });
            RegisterCustomHandler("Replace flipped", () =>
            {
                string filePath = ModuleImportUtilities.SelectModuleImport<Bitmap>();
                 if (!string.IsNullOrEmpty(filePath))
                     Replace(ReplaceBitmap(filePath, true));
            });
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        protected override void OnReplace( Texture previousData )
        {
            Data.Name = previousData.Name;
            Data.Id = previousData.Id;

            base.OnReplace( previousData );
        }

        public TextureNode( string name, Texture data ) : base( name, data )
        {
        }
    }
}