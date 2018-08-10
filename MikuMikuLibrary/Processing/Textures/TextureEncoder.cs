using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Processing.Textures.DDS;
using MikuMikuLibrary.Textures;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace MikuMikuLibrary.Processing.Textures
{
    public static class TextureEncoder
    {
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
            {
                foreach ( var mipMap in level )
                {
                    source.Read( mipMap.Data, 0, mipMap.Data.Length );
                }
            }

            return texture;
        }

        public static Texture Encode( Bitmap bitmap, TextureFormat format, bool generateMipMaps )
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            if ( TextureFormatUtilities.IsCompressed( format ) )
            {
                width = AlignmentUtilities.AlignToNextPowerOfTwo( bitmap.Width );
                height = AlignmentUtilities.AlignToNextPowerOfTwo( bitmap.Height );
            }

            Texture texture;

            if ( generateMipMaps && TextureFormatUtilities.IsCompressed( format ) )
                texture = new Texture( width, height, format, 1, ( int )Math.Log( Math.Max( width, height ), 2 ) + 1 );

            else
                texture = new Texture( width, height, format );

            for ( int i = 0; i < texture.MipMapCount; i++ )
                Encode( texture[ i ], bitmap );

            return texture;
        }

        private unsafe static void Encode( SubTexture subTexture, Bitmap bitmap )
        {
            bool ownsBitmap = false;

            if ( subTexture.Width != bitmap.Width || subTexture.Height != bitmap.Height )
            {
                ownsBitmap = true;
                bitmap = new Bitmap( bitmap, subTexture.Width, subTexture.Height );
            }

            var rect = new Rectangle( 0, 0, bitmap.Width, bitmap.Height );

            if ( subTexture.Format == TextureFormat.RGB )
            {
                var bitmapData = bitmap.LockBits( rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );
                Marshal.Copy( bitmapData.Scan0, subTexture.Data, 0, subTexture.Data.Length );
                bitmap.UnlockBits( bitmapData );
            }
            else if ( subTexture.Format == TextureFormat.RGBA )
            {
                var bitmapData = bitmap.LockBits( rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );

                fixed ( byte* ptr = subTexture.Data )
                    Int32RGBAToByte( ( int* )bitmapData.Scan0, ptr, subTexture.Data.Length );

                bitmap.UnlockBits( bitmapData );
            }
            else
            {
                var compressedPixels = DDSCodec.CompressPixelData( bitmap, TextureUtilities.GetDDSPixelFormat( subTexture.Format ) );
                Array.Copy( compressedPixels, subTexture.Data, subTexture.Data.Length );
            }

            if ( ownsBitmap )
                bitmap.Dispose();
        }

        public static Texture Encode( string sourceFileName )
        {
            if ( sourceFileName.EndsWith( ".dds", StringComparison.OrdinalIgnoreCase ) )
            {
                using ( var source = File.OpenRead( sourceFileName ) )
                    return Encode( source );
            }

            using ( var bitmap = new Bitmap( sourceFileName ) )
                return Encode( bitmap, DDSCodec.HasTransparency( bitmap ) ? TextureFormat.DXT5 : TextureFormat.DXT1, true );
        }

        private unsafe static void Int32RGBAToByte( int *source, byte *destination, int length )
        {
            byte* end = destination + length;

            while ( destination < end )
            {
                var color = Color.FromArgb( *source++ );
                *destination++ = color.R;
                *destination++ = color.G;
                *destination++ = color.B;
                *destination++ = color.A;
            }
        }
    }
}
