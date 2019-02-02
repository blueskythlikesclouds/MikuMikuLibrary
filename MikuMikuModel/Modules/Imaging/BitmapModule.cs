using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MikuMikuModel.Modules.Imaging
{
    public class BitmapModule : FormatModule<Bitmap>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Bitmap";
        public override string[] Extensions => new[] { "png", "jpg", "jpeg", "gif", "bmp" };

        protected override Bitmap ImportCore( Stream source, string fileName ) => 
            ( Bitmap )Image.FromStream( source );

        protected override void ExportCore( Bitmap model, Stream destination, string fileName )
        {
            var imageFormat = ImageFormat.Png;

            if ( !string.IsNullOrEmpty( fileName ) )
            {
                string extension = Path.GetExtension( fileName ).Trim( '.' ).ToLowerInvariant();
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
                        throw new ArgumentException( "Image format could not be detected", nameof( fileName ) );
                }
            }

            model.Save( destination, imageFormat );
        }
    }
}