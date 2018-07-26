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
            var m = texture.MipMaps[ 0 ];
            var bitmap = new Bitmap( m.Width, m.Height );
            var rect = new Rectangle( 0, 0, bitmap.Width, bitmap.Height );

            if ( m.Format == TextureFormat.RGB )
            {
                var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );
                Marshal.Copy( m.Data, 0, bitmapData.Scan0, m.Data.Length );
                bitmap.UnlockBits( bitmapData );
            }
            else if ( m.Format == TextureFormat.RGBA )
            {
                var bitmapData = bitmap.LockBits( rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
                Marshal.Copy( m.Data, 0, bitmapData.Scan0, m.Data.Length );
                bitmap.UnlockBits( bitmapData );
            }
            else if ( texture.IsYCbCr )
            {
                var lumBuffer = DDSCodec.DecompressPixelDataToRGBA(
                    texture.MipMaps[ 0 ].Data, texture.MipMaps[ 0 ].Width, texture.MipMaps[ 0 ].Height, GetDDSPixelFormat( texture.MipMaps[ 0 ].Format ) );

                var cbrBuffer = DDSCodec.DecompressPixelDataToRGBA(
                    texture.MipMaps[ 1 ].Data, texture.MipMaps[ 1 ].Width, texture.MipMaps[ 1 ].Height, GetDDSPixelFormat( texture.MipMaps[ 1 ].Format ) );

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
                var buffer = DDSCodec.DecompressPixelDataToRGBA( m.Data, m.Width, m.Height, GetDDSPixelFormat( m.Format ) );
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
            var topMipMap = texture.MipMaps[ 0 ];

            var ddsHeader = new DDSHeader(
                topMipMap.Width, topMipMap.Height, GetDDSPixelFormat( topMipMap.Format ) );

            if ( texture.MipMaps.Count > 1 )
            {
                ddsHeader.MipMapCount = texture.MipMaps.Count;
                ddsHeader.Flags |= DDSHeaderFlags.MipMapCount;
                ddsHeader.Caps |= DDSHeaderCaps.MipMap;
            }

            ddsHeader.Save( destination );

            foreach ( var mipMap in texture.MipMaps )
                destination.Write( mipMap.Data, 0, mipMap.Data.Length );
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
