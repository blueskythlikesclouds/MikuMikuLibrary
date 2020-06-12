using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Textures.DDS;

namespace MikuMikuLibrary.Textures
{
    public static class TextureEncoder
    {
        private static readonly Matrix4x4 sRGBtoYCbCr;

        public static Texture Encode( Stream source )
        {
            var ddsHeader = new DDSHeader( source );

            int depth = 1;
            if ( ddsHeader.Flags.HasFlag( DDSHeaderFlags.Depth ) )
                depth = ddsHeader.Depth;

            int mipMapCount = 1;
            if ( ddsHeader.Flags.HasFlag( DDSHeaderFlags.MipMapCount ) )
                mipMapCount = ddsHeader.MipMapCount;

            var format = TextureUtilities.GetTextureFormat( ddsHeader.PixelFormat );

            var texture = new Texture( ddsHeader.Width, ddsHeader.Height, format, depth, mipMapCount );
            foreach ( var level in texture.EnumerateLevels() )
            foreach ( var mipMap in level )
                source.Read( mipMap.Data, 0, mipMap.Data.Length );

            return texture;
        }

        public static Texture Encode( Bitmap bitmap, TextureFormat format, bool generateMipMaps )
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            if ( TextureFormatUtilities.IsCompressed( format ) )
            {
                width = AlignmentHelper.AlignToNextPowerOfTwo( bitmap.Width );
                height = AlignmentHelper.AlignToNextPowerOfTwo( bitmap.Height );
            }

            Texture texture;

            if ( generateMipMaps && TextureFormatUtilities.IsCompressed( format ) )
                texture = new Texture( width, height, format, 1,
                    ( int ) Math.Log( Math.Max( width, height ), 2 ) + 1 );

            else
                texture = new Texture( width, height, format );

            for ( int i = 0; i < texture.MipMapCount; i++ )
                Encode( texture[ i ], bitmap );

            return texture;
        }

        public static unsafe Texture EncodeYCbCr( Bitmap bitmap )
        {
            int width = AlignmentHelper.Align( bitmap.Width, 4 );
            int height = AlignmentHelper.Align( bitmap.Height, 4 );

            var texture = new Texture( width, height, TextureFormat.ATI2, 1, 2 );

            var bitmapData = bitmap.LockBits( new Rectangle( 0, 0, bitmap.Width, bitmap.Height ),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );

            using ( var lumBitmap = new Bitmap( bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb ) )
            using ( var cbrBitmap = new Bitmap( bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb ) )
            {
                var lumData = lumBitmap.LockBits( new Rectangle( 0, 0, lumBitmap.Width, lumBitmap.Height ),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );

                var cbrData = cbrBitmap.LockBits( new Rectangle( 0, 0, cbrBitmap.Width, cbrBitmap.Height ),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );

                for ( int x = 0; x < lumBitmap.Width; x++ )
                for ( int y = 0; y < lumBitmap.Height; y++ )
                {
                    var color = Color.FromArgb( *( ( int* ) bitmapData.Scan0 + y * bitmap.Width + x ) );
                    var ycbcrVector =
                        Vector3.Transform( Vector3.Divide( new Vector3( color.R, color.G, color.B ), 255 ),
                            sRGBtoYCbCr );

                    var lumColor = Color.FromArgb( ( int ) ( ycbcrVector.X * 255 ), color.A, 0 );

                    var cbrVector =
                        Vector3.Multiply(
                            Vector3.Min( Vector3.One, new Vector3( ycbcrVector.Y + 0.5f, ycbcrVector.Z + 0.5f, 0 ) ),
                            255 );

                    var cbrColor = Color.FromArgb( ( int ) cbrVector.X, ( int ) cbrVector.Y, 0 );

                    *( ( int* ) lumData.Scan0 + y * lumBitmap.Width + x ) = lumColor.ToArgb();
                    *( ( int* ) cbrData.Scan0 + y * cbrBitmap.Width + x ) = cbrColor.ToArgb();
                }

                lumBitmap.UnlockBits( lumData );
                cbrBitmap.UnlockBits( cbrData );

                Encode( texture[ 0 ], lumBitmap );
                Encode( texture[ 1 ], cbrBitmap );
            }

            bitmap.UnlockBits( bitmapData );

            return texture;
        }

        private static unsafe void Encode( SubTexture subTexture, Bitmap bitmap )
        {
            bool ownsBitmap = false;

            if ( subTexture.Width != bitmap.Width || subTexture.Height != bitmap.Height )
            {
                ownsBitmap = true;
                bitmap = new Bitmap( bitmap, ( int ) subTexture.Width, ( int ) subTexture.Height );
            }

            var rect = new Rectangle( 0, 0, bitmap.Width, bitmap.Height );

            if ( subTexture.Format == TextureFormat.RGB )
            {
                var bitmapData = bitmap.LockBits( rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

                fixed ( byte* ptr = subTexture.Data )
                {
                    BGRtoRGB( ( byte* ) bitmapData.Scan0, ptr, subTexture.Data.Length );
                }

                bitmap.UnlockBits( bitmapData );
            }
            else if ( subTexture.Format == TextureFormat.RGBA )
            {
                var bitmapData = bitmap.LockBits( rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );

                fixed ( byte* ptr = subTexture.Data )
                {
                    Int32RGBAToByte( ( int* ) bitmapData.Scan0, ptr, subTexture.Data.Length );
                }

                bitmap.UnlockBits( bitmapData );
            }
            else
            {
                var compressedPixels =
                    DDSCodec.CompressPixelData( bitmap, TextureUtilities.GetDDSPixelFormat( subTexture.Format ) );
                Array.Copy( compressedPixels, subTexture.Data, subTexture.Data.Length );
            }

            if ( ownsBitmap )
                bitmap.Dispose();
        }

        public static Texture Encode( string sourceFileName )
        {
            if ( sourceFileName.EndsWith( ".dds", StringComparison.OrdinalIgnoreCase ) )
                using ( var source = File.OpenRead( sourceFileName ) )
                {
                    return Encode( source );
                }

            using ( var bitmap = new Bitmap( sourceFileName ) )
            {
                return Encode( bitmap, DDSCodec.HasTransparency( bitmap ) ? TextureFormat.DXT5 : TextureFormat.DXT1,
                    true );
            }
        }

        private static unsafe void Int32RGBAToByte( int* source, byte* destination, int length )
        {
            var end = destination + length;

            while ( destination < end )
            {
                var color = Color.FromArgb( *source++ );
                *destination++ = color.R;
                *destination++ = color.G;
                *destination++ = color.B;
                *destination++ = color.A;
            }
        }

        private static unsafe void BGRtoRGB( byte* source, byte* destination, int length )
        {
            var end = source + length;
            while ( source < end )
            {
                byte b = *source++;
                byte g = *source++;
                byte r = *source++;

                *destination++ = r;
                *destination++ = g;
                *destination++ = b;
            }
        }

        static TextureEncoder()
        {
            Matrix4x4.Invert( TextureDecoder.sYCbCrToRGB, out sRGBtoYCbCr );
        }
    }
}