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

        private Texture ReplaceBitmap( string filePath, bool flipped = false )
        {
            using ( var bitmap = new Bitmap( filePath ) )
            {
                if ( flipped )
                    bitmap.RotateFlip( RotateFlipType.Rotate180FlipX );

                if ( Data.IsYCbCr )
                    return TextureEncoder.EncodeFromBitmap( bitmap, TextureFormat.RGBA8, false );

                return TextureEncoder.EncodeFromBitmap( bitmap, Format, Data.MipMapCount != 0 );
            }
        }

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

        protected override void Initialize()
        {
            AddExportHandler<Texture>( filePath => TextureDecoder.DecodeToFile( Data, filePath ) );
            AddReplaceHandler<Texture>( filePath =>
                TextureEncoder.EncodeFromFile( filePath, Data.IsYCbCr ? TextureFormat.RGBA8 : Data.Format, Data.MipMapCount != 1 ) );

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
            AddCustomHandler( "Replace flipped", () =>
            {
                string filePath = ModuleImportUtilities.SelectModuleImport<Bitmap>();

                if ( !string.IsNullOrEmpty( filePath ) )
                    Replace( ReplaceBitmap( filePath, true ) );
            } );
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