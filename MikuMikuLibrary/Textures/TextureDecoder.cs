using MikuMikuLibrary.Textures.DDS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace MikuMikuLibrary.Textures
{
    public static class TextureDecoder
    {
        // Thanks to Brolijah for finding this out!!
        private static unsafe void ConvertYCbCrToRGBA( int* lumIntPtr, int* cbrIntPtr, int* outIntPtr, int width, int height )
        {
            for ( int y = 0; y < height; y++ )
            {
                for ( int x = 0; x < width; x++ )
                {
                    var positionLum = y * width + x;
                    var positionCbr = ( y >> 1 ) * ( width >> 1 ) + ( x >> 1 );

                    var lum = Color.FromArgb( *( lumIntPtr + positionLum ) );
                    var cbr = Color.FromArgb( *( cbrIntPtr + positionCbr ) );

                    float ypn = lum.R / 255.0f;
                    float cbn = cbr.R / 255.0f - 0.5f;
                    float crn = cbr.G / 255.0f - 0.5f;

                    float r = Math.Min( 1, Math.Max( 0, ypn + 1.5748f * crn ) );
                    float g = Math.Min( 1, Math.Max( 0, ypn + -0.1873f * cbn + -0.4681f * crn ) );
                    float b = Math.Min( 1, Math.Max( 0, ypn + 1.8556f * cbn ) );

                    *( outIntPtr + positionLum ) = Color.FromArgb( lum.G, ( int )( r * 255 ), ( int )( g * 255 ), ( int )( b * 255 ) ).ToArgb();
                }
            }
        }

        public unsafe static Bitmap Decode( SubTexture subTexture )
        {
            var bitmap = new Bitmap( subTexture.Width, subTexture.Height );
            var rect = new Rectangle( 0, 0, bitmap.Width, bitmap.Height );

            if ( subTexture.Format == TextureFormat.RGB )
            {
                var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );

                fixed ( byte* ptr = subTexture.Data )
                    RGBtoBGR( ptr, ( byte* )bitmapData.Scan0, subTexture.Data.Length );

                bitmap.UnlockBits( bitmapData );
            }
            else if ( subTexture.Format == TextureFormat.RGBA )
            {
                var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );

                fixed ( byte* ptr = subTexture.Data )
                    ByteRGBAToInt32( ptr, ( int* )bitmapData.Scan0, subTexture.Data.Length );

                bitmap.UnlockBits( bitmapData );
            }
            else if ( subTexture.Format == TextureFormat.RGBA5 )
            {
                var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );

                fixed ( byte* ptr = subTexture.Data )
                    RGBA5toRGBA( ptr, ( int* )bitmapData.Scan0, subTexture.Data.Length );

                bitmap.UnlockBits( bitmapData );
            }
            else
            {
                var buffer = DDSCodec.DecompressPixelDataToRGBA( subTexture.Data, subTexture.Width, subTexture.Height, Texture.GetDDSPixelFormat( subTexture.Format ) );
                var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
                Marshal.Copy( buffer, 0, bitmapData.Scan0, buffer.Length );
                bitmap.UnlockBits( bitmapData );
            }

            return bitmap;
        }

        private static IEnumerable<int> CubeMapToDDSCubeMap()
        {
            yield return 0;
            yield return 1;
            yield return 2;
            yield return 3;
            yield return 5;
            yield return 4;

            // Just for you Yukikami
            //yield return 0;
            //yield return 4;
            //yield return 1;
            //yield return 5;
            //yield return 3;
            //yield return 2;
        }

        public unsafe static Bitmap Decode( Texture texture )
        {
            if ( texture.IsYCbCr )
            {
                var bitmap = new Bitmap( texture[ 0 ].Width, texture[ 0 ].Height );
                var rect = new Rectangle( 0, 0, bitmap.Width, bitmap.Height );

                var lumBuffer = DDSCodec.DecompressPixelDataToRGBA(
                    texture[ 0 ].Data, texture[ 0 ].Width, texture[ 0 ].Height, Texture.GetDDSPixelFormat( texture.Format ) );

                var cbrBuffer = DDSCodec.DecompressPixelDataToRGBA(
                    texture[ 1 ].Data, texture[ 1 ].Width, texture[ 1 ].Height, Texture.GetDDSPixelFormat( texture.Format ) );

                var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
                fixed ( byte* lumPtr = lumBuffer )
                fixed ( byte* cbrPtr = cbrBuffer )
                {
                    int* lumIntPtr = ( int* )lumPtr;
                    int* cbrIntPtr = ( int* )cbrPtr;

                    ConvertYCbCrToRGBA( lumIntPtr, cbrIntPtr, ( int* )bitmapData.Scan0, bitmap.Width, bitmap.Height );
                }

                bitmap.UnlockBits( bitmapData );
                return bitmap;
            }

            else if ( texture.UsesDepth )
            {
                var bitmap = new Bitmap( texture.Width * texture.Depth, texture.Height );
                using ( var gfx = Graphics.FromImage( bitmap ) )
                {
                    int currentIndex = 0;
                    foreach ( var i in CubeMapToDDSCubeMap() )
                        gfx.DrawImageUnscaled( Decode( texture[ i, 0 ] ), ( currentIndex++ ) * texture.Width, 0 );
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
            var ddsHeader = new DDSHeader( texture.Width, texture.Height, Texture.GetDDSPixelFormat( texture.Format ) );

            if ( texture.UsesDepth )
            {
                ddsHeader.Depth = texture.Depth;
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

            if ( texture.UsesDepth )
            {
                foreach ( var i in CubeMapToDDSCubeMap() )
                {
                    foreach ( var mipMap in texture.EnumerateMipMaps( i ) )
                        destination.Write( mipMap.Data, 0, mipMap.Data.Length );
                }
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

        private unsafe static void ByteRGBAToInt32( byte* source, int* destination, int length )
        {
            byte* end = source + length;
            while ( source < end )
            {
                byte red = *source++;
                byte green = *source++;
                byte blue = *source++;
                byte alpha = *source++;

                *destination++ = Color.FromArgb( alpha, red, green, blue ).ToArgb();
            }
        }

        private unsafe static void RGBtoBGR( byte* source, byte* destination, int length )
        {
            byte* end = source + length;
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

        private unsafe static void RGBA5toRGBA( byte* source, int* destination, int length )
        {
            short* start = ( short* )source;
            short* end = ( short* )( source + length );

            while ( start < end )
            {
                int red = ( *start & 0x1F ) << 3;
                int green = ( ( *start >> 5 ) & 0x1F ) << 3;
                int blue = ( ( *start >> 10 ) & 0x1F ) << 3;
                int alpha = ( ( *start >> 15 ) & 1 ) * 255;
                start++;

                *destination++ = Color.FromArgb( alpha, red, green, blue ).ToArgb();
            }
        }
    }
}
