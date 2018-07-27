using MikuMikuLibrary.Processing.Textures.DDS;
using MikuMikuLibrary.Textures;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace MikuMikuLibrary.Processing.Textures
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

        public unsafe static Bitmap Decode( Texture texture )
        {
            var subTexture = texture[ 0 ];
            var bitmap = new Bitmap( subTexture.Width, subTexture.Height );
            var rect = new Rectangle( 0, 0, bitmap.Width, bitmap.Height );

            if ( subTexture.Format == TextureFormat.RGB )
            {
                var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );
                Marshal.Copy( subTexture.Data, 0, bitmapData.Scan0, subTexture.Data.Length );
                bitmap.UnlockBits( bitmapData );
            }
            else if ( subTexture.Format == TextureFormat.RGBA )
            {
                var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
                Marshal.Copy( subTexture.Data, 0, bitmapData.Scan0, subTexture.Data.Length );
                bitmap.UnlockBits( bitmapData );
            }
            else if ( texture.IsYCbCr )
            {
                var lumBuffer = DDSCodec.DecompressPixelDataToRGBA(
                    texture[ 0 ].Data, texture[ 0 ].Width, texture[ 0 ].Height, GetDDSPixelFormat( texture.Format ) );

                var cbrBuffer = DDSCodec.DecompressPixelDataToRGBA(
                    texture[ 1 ].Data, texture[ 1 ].Width, texture[ 1 ].Height, GetDDSPixelFormat( texture.Format ) );

                var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
                fixed ( byte* lumPtr = lumBuffer )
                fixed ( byte* cbrPtr = cbrBuffer )
                {
                    int* lumIntPtr = ( int* )lumPtr;
                    int* cbrIntPtr = ( int* )cbrPtr;

                    ConvertYCbCrToRGBA( lumIntPtr, cbrIntPtr, ( int* )bitmapData.Scan0, bitmap.Width, bitmap.Height );
                }

                bitmap.UnlockBits( bitmapData );
            }

            else
            {
                var buffer = DDSCodec.DecompressPixelDataToRGBA( subTexture.Data, subTexture.Width, subTexture.Height, GetDDSPixelFormat( subTexture.Format ) );
                var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
                Marshal.Copy( buffer, 0, bitmapData.Scan0, buffer.Length );
                bitmap.UnlockBits( bitmapData );
            }

            return bitmap;
        }

        public static void DecodeToPNG( Texture texture, string fileName )
        {
            using ( var bitmap = Decode( texture ) )
                bitmap.Save( fileName, ImageFormat.Png );
        }

        public static void DecodeToDDS( Texture texture, Stream destination )
        {
            var ddsHeader = new DDSHeader( texture.Width, texture.Height, GetDDSPixelFormat( texture.Format ) );

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

            foreach ( var level in texture.EnumerateLevels() )
            {
                foreach ( var mipMap in level )
                {
                    destination.Write( mipMap.Data, 0, mipMap.Data.Length );
                }
            }
        }

        public static void DecodeToDDS( Texture texture, string destinationFileName )
        {
            using ( var destination = File.Create( destinationFileName ) )
                DecodeToDDS( texture, destination );
        }

        private static DDSPixelFormatFourCC GetDDSPixelFormat( TextureFormat textureFormat )
        {
            switch ( textureFormat )
            {
                case TextureFormat.RGB:
                    return DDSPixelFormatFourCC.R8G8B8;

                case TextureFormat.RGBA:
                    return DDSPixelFormatFourCC.A8R8G8B8;

                case TextureFormat.DXT1:
                    return DDSPixelFormatFourCC.DXT1;

                case TextureFormat.DXT3:
                    return DDSPixelFormatFourCC.DXT3;

                case TextureFormat.DXT5:
                    return DDSPixelFormatFourCC.DXT5;

                case TextureFormat.ATI1:
                    return DDSPixelFormatFourCC.ATI1;

                case TextureFormat.ATI2:
                    return DDSPixelFormatFourCC.ATI2N_3Dc;

                default:
                    throw new ArgumentException( nameof( textureFormat ) );
            }
        }
    }
}
