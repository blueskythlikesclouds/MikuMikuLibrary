using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using MikuMikuLibrary.Textures.DDS;

namespace MikuMikuLibrary.Textures
{
    public static class TextureDecoder
    {
        private static readonly int[] sCubeMapToDds = { 0, 1, 2, 3, 5, 4 };

        internal static readonly Matrix4x4 sYCbCrToRGB = new Matrix4x4(
            1.0f, 1.0f, 1.0f, 0.0f,
            0.0f, -0.1873f, 1.8556f, 0.0f,
            1.5748f, -0.4681f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f );

        // Thanks to Brolijah for finding this out!!
        private static unsafe void ConvertYCbCrToRGBA( int* lumPtr, int* cbrPtr, int* outPtr, int width, int height )
        {
            for ( int y = 0; y < height; y++ )
            for ( int x = 0; x < width; x++ )
            {
                int offset = y * width + x;

                var lum = Color.FromArgb( *( lumPtr + offset ) );
                var cbr = Color.FromArgb( *( cbrPtr + offset ) );

                var rgb = Vector3.Transform( new Vector3( lum.R / 255.0f, cbr.R / 255.0f - 0.5f, cbr.G / 255.0f - 0.5f ), sYCbCrToRGB );
                rgb = Vector3.Multiply( Vector3.Max( Vector3.Zero, Vector3.Min( Vector3.One, rgb ) ), 255.0f );

                *( outPtr + offset ) = Color.FromArgb( lum.G, ( int ) rgb.X, ( int ) rgb.Y, ( int ) rgb.Z ).ToArgb();
            }
        }

        public static unsafe Bitmap Decode( SubTexture subTexture )
        {
            var bitmap = new Bitmap( subTexture.Width, subTexture.Height );
            var rect = new Rectangle( 0, 0, bitmap.Width, bitmap.Height );

            switch ( subTexture.Format )
            {
                case TextureFormat.RGB:
                {
                    var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );

                    fixed ( byte* ptr = subTexture.Data ) 
                        RGBtoBGR( ptr, ( byte* ) bitmapData.Scan0, subTexture.Data.Length );

                    bitmap.UnlockBits( bitmapData );
                    break;
                }

                case TextureFormat.RGBA:
                {
                    var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );

                    fixed ( byte* ptr = subTexture.Data ) 
                        ByteRGBAToInt32( ptr, ( int* ) bitmapData.Scan0, subTexture.Data.Length );

                    bitmap.UnlockBits( bitmapData );
                    break;
                }

                case TextureFormat.RGBA4:
                {
                    var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );

                    fixed ( byte* ptr = subTexture.Data ) 
                        RGBA4toRGBA( ptr, ( int* ) bitmapData.Scan0, subTexture.Data.Length );

                    bitmap.UnlockBits( bitmapData );
                    break;
                }

                default:
                {
                    var buffer = DDSCodec.DecompressPixelDataToRGBA( subTexture.Data, subTexture.Width, subTexture.Height, 
                        TextureUtilities.GetDDSPixelFormat( subTexture.Format ) );

                    var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
                    Marshal.Copy( buffer, 0, bitmapData.Scan0, buffer.Length );
                    bitmap.UnlockBits( bitmapData );

                    break;
                }
            }

            return bitmap;
        }

        public static unsafe Bitmap Decode( Texture texture )
        {
            if ( texture.IsYCbCr )
            {
                var bitmap = new Bitmap( texture[ 0 ].Width, texture[ 0 ].Height );
                var rect = new Rectangle( 0, 0, bitmap.Width, bitmap.Height );

                var lumBitmap = Decode( texture[ 0, 0 ] );
                var cbrBitmap = Decode( texture[ 0, 1 ] );

                var cbrBitmapScaled = new Bitmap( texture.Width, texture.Height );

                using ( var gfx = Graphics.FromImage( cbrBitmapScaled ) )
                {
                    gfx.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    gfx.DrawImage( cbrBitmap, rect );
                }

                var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
                var lumData = lumBitmap.LockBits( rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
                var cbrData = cbrBitmapScaled.LockBits( rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );

                ConvertYCbCrToRGBA( ( int* ) lumData.Scan0, ( int* ) cbrData.Scan0, ( int* ) bitmapData.Scan0, bitmap.Width, bitmap.Height );

                bitmap.UnlockBits( bitmapData );
                lumBitmap.UnlockBits( lumData );
                cbrBitmapScaled.UnlockBits( cbrData );

                lumBitmap.Dispose();
                cbrBitmap.Dispose();
                cbrBitmapScaled.Dispose();

                return bitmap;
            }

            if ( texture.UsesArraySize )
            {
                var bitmap = new Bitmap( texture.Width * texture.ArraySize, texture.Height );

                using ( var gfx = Graphics.FromImage( bitmap ) )
                {
                    int currentIndex = 0;
                    foreach ( int i in sCubeMapToDds )
                        gfx.DrawImageUnscaled( Decode( texture[ i, 0 ] ), currentIndex++ * texture.Width, 0 );
                }

                return bitmap;
            }

            return Decode( texture[ 0 ] );
        }

        public static void DecodeToPNG( Texture texture, string fileName )
        {
            using ( var bitmap = Decode( texture ) ) 
                bitmap.Save( fileName, ImageFormat.Png );
        }

        public static void DecodeToDDS( Texture texture, Stream destination )
        {
            var ddsHeader = new DDSHeader( texture.Width, texture.Height,
                TextureUtilities.GetDDSPixelFormat( texture.Format ) );

            if ( texture.UsesArraySize )
            {
                ddsHeader.Depth = texture.ArraySize;
                ddsHeader.Flags |= DDSHeaderFlags.Depth;
                ddsHeader.Caps |= DDSHeaderCaps.Complex;
                ddsHeader.Caps2 |= DDSHeaderCaps2.CubeMap |
                                   DDSHeaderCaps2.CubeMapPositiveX | DDSHeaderCaps2.CubeMapNegativeX |
                                   DDSHeaderCaps2.CubeMapPositiveY | DDSHeaderCaps2.CubeMapNegativeY |
                                   DDSHeaderCaps2.CubeMapPositiveZ | DDSHeaderCaps2.CubeMapNegativeZ;
            }

            if ( texture.UsesMipMaps )
            {
                ddsHeader.MipMapCount = texture.MipMapCount;
                ddsHeader.Flags |= DDSHeaderFlags.MipMapCount;
                ddsHeader.Caps |= DDSHeaderCaps.Complex;
            }

            ddsHeader.Save( destination );

            if ( texture.UsesArraySize )
            {
                foreach ( int i in sCubeMapToDds )
                foreach ( var mipMap in texture.EnumerateMipMaps( i ) )
                    destination.Write( mipMap.Data, 0, mipMap.Data.Length );
            }

            else
            {
                foreach ( var mipMap in texture.EnumerateMipMaps() )
                    destination.Write( mipMap.Data, 0, mipMap.Data.Length );
            }
        }

        public static void DecodeToDDS( Texture texture, string destinationFileName )
        {
            using ( var destination = File.Create( destinationFileName ) ) 
                DecodeToDDS( texture, destination );
        }

        private static unsafe void ByteRGBAToInt32( byte* source, int* destination, int length )
        {
            var end = source + length;

            while ( source < end )
            {
                byte red = *source++;
                byte green = *source++;
                byte blue = *source++;
                byte alpha = *source++;

                *destination++ = Color.FromArgb( alpha, red, green, blue ).ToArgb();
            }
        }

        private static unsafe void RGBtoBGR( byte* source, byte* destination, int length )
        {
            var end = source + length;

            while ( source < end )
            {
                byte r = *source++;
                byte g = *source++;
                byte b = *source++;

                *destination++ = b;
                *destination++ = g;
                *destination++ = r;
            }
        }

        private static unsafe void RGBA4toRGBA( byte* source, int* destination, int length )
        {
            var start = ( short* ) source;
            var end = ( short* ) ( source + length );

            while ( start < end )
            {
                int red = ( *start & 0xF ) * 255 / 15;
                int green = ( ( *start >> 4 ) & 0xF ) * 255 / 15;
                int blue = ( ( *start >> 8 ) & 0xF ) * 255 / 15;
                int alpha = ( ( *start >> 12 ) & 0xF ) * 255 / 15;
                start++;

                *destination++ = Color.FromArgb( alpha, red, green, blue ).ToArgb();
            }
        }
    }
}