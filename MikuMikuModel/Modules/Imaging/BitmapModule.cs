using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MikuMikuModel.Modules.Imaging
{
    public class BitmapModule : FormatModule<Bitmap>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Portable Network Graphics", "png", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Joint Photographic Group", "jpg", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Joint Photographic Group", "jpeg", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Graphic Interchange Format", "gif", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Bitmap", "bmp", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
        };

        protected override Bitmap ImportCore( Stream source, string fileName )
        {
            return ( Bitmap ) Image.FromStream( source );
        }

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