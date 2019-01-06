using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MikuMikuModel.FormatModules
{
    public class BitmapFormatModule : FormatModule<Bitmap>
    {
        public override FormatModuleFlags Flags =>
            FormatModuleFlags.Import | FormatModuleFlags.Export;

        public override string Name => "Bitmap";
        public override string[] Extensions => new[] { "png", "jpg", "jpeg", "gif", "bmp" };

        protected override bool CanImportCore( Stream source, string fileName )
        {
            return true;
        }

        protected override void ExportCore( Bitmap obj, Stream destination, string fileName )
        {
            if ( destination is FileStream fileStream )
                fileName = Path.GetFileName( fileStream.Name );

            ImageFormat imageFormat = ImageFormat.Png;

            if ( !string.IsNullOrEmpty( fileName ) )
            {
                var extension = Path.GetExtension( fileName ).Trim( '.' ).ToLowerInvariant();
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

            obj.Save( destination, imageFormat );
        }

        protected override Bitmap ImportCore( Stream source, string fileName )
        {
            return ( Bitmap )Image.FromStream( source );
        }
    }
}
