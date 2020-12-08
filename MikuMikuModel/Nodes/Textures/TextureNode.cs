using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.Processing;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Modules;
using MikuMikuModel.Nodes.TypeConverters;
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

        [Category( "General" )]
        [TypeConverter( typeof( IdTypeConverter ) )]
        public uint Id
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )] public int Width => GetProperty<int>();
        [Category( "General" )] public int Height => GetProperty<int>();
        [Category( "General" )] public TextureFormat Format => GetProperty<TextureFormat>();

        private ImageFormat GetImageFormat( string filePath )
        {
            string extension = Path.GetExtension( filePath ).Trim( '.' ).ToLowerInvariant();
            switch ( extension )
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
                    throw new ArgumentException( "Image format could not be detected", nameof( filePath ) );
            }
        }

        private void EncodeTexture( TextureFormat format, bool ycbcr, bool flipped )
        {
            string filePath = ModuleImportUtilities.SelectModuleImport<Bitmap>();

            if ( string.IsNullOrEmpty( filePath ) )
                return;

            using ( var bitmap = new Bitmap( filePath ) )
            {
                if ( flipped )
                    bitmap.RotateFlip( RotateFlipType.Rotate180FlipX );

                Replace( ycbcr
                    ? TextureEncoder.EncodeYCbCrFromBitmap( bitmap )
                    : TextureEncoder.EncodeFromBitmap( bitmap, format != TextureFormat.Unknown ? format : Format, Data.MipMapCount > 1 ) );
            }
        }

        protected override void Initialize()
        {
            AddExportHandler<Texture>( filePath => TextureDecoder.DecodeToFile( Data, filePath ) );
            AddReplaceHandler<Texture>( filePath =>
            {
                if ( Data.IsYCbCr )
                    return TextureEncoder.EncodeYCbCrFromFile( filePath );

                return TextureEncoder.EncodeFromFile( filePath, Data.Format, Data.MipMapCount > 1 );
            } );

            AddCustomHandlerSeparator();
            AddCustomHandler( "Export flipped", () =>
            {
                string filePath = ModuleExportUtilities.SelectModuleExport<Bitmap>( "Select a file to export to.", Name );

                if ( string.IsNullOrEmpty( filePath ) ) 
                    return;

                var imageFormat = GetImageFormat( filePath );

                using ( var bitmap = TextureDecoder.DecodeToBitmap( Data ) )
                {
                    bitmap.RotateFlip( RotateFlipType.Rotate180FlipX );
                    bitmap.Save( filePath, imageFormat );
                }
            } );

            AddCustomHandler( "Replace flipped", () => EncodeTexture( TextureFormat.Unknown, Data.IsYCbCr, true ) );

            AddCustomHandlerSeparator();

            AddCustomHandler( "Replace as..." );
            {
                AppendCustomHandler( "Uncompressed", () => EncodeTexture( TextureFormat.RGBA8, false, false ) );
                AppendCustomHandler( "DXT1/DXT5", () => EncodeTexture( TextureFormat.DXT5, false, false ) );
                AppendCustomHandler( "ATI1", () => EncodeTexture( TextureFormat.ATI1, false, false ) );
                AppendCustomHandler( "ATI2", () => EncodeTexture( TextureFormat.ATI2, false, false ) );
                AppendCustomHandler( "YCbCr", () => EncodeTexture( TextureFormat.Unknown, true, false ) );
            }

            AddCustomHandler( "Replace as... (flipped)" );
            {
                AppendCustomHandler( "Uncompressed", () => EncodeTexture( TextureFormat.RGBA8, false, true ) );
                AppendCustomHandler( "DXT1/DXT5", () => EncodeTexture( TextureFormat.DXT5, false, true ) );
                AppendCustomHandler( "ATI1", () => EncodeTexture( TextureFormat.ATI1, false, true ) );
                AppendCustomHandler( "ATI2", () => EncodeTexture( TextureFormat.ATI2, false, true ) );
                AppendCustomHandler( "YCbCr", () => EncodeTexture( TextureFormat.Unknown, true, true ) );
            }
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