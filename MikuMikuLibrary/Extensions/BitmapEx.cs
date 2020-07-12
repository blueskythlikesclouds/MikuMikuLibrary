using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MikuMikuLibrary.Extensions
{
    public static class BitmapEx
    {
        //
        // Implementation taken from aelan/KFreon's DDS codec class.
        //

        public static bool HasTransparency( this Bitmap bitmap )
        {
            // not an alpha-capable color format.
            if ( ( bitmap.Flags & ( int ) ImageFlags.HasAlpha ) == 0 )
                return false;
            // Indexed formats. Special case because one index on their palette is configured as THE transparent color.
            if ( bitmap.PixelFormat == PixelFormat.Format8bppIndexed ||
                 bitmap.PixelFormat == PixelFormat.Format4bppIndexed )
            {
                var pal = bitmap.Palette;
                // Find the transparent index on the palette.
                int transCol = -1;
                for ( int i = 0; i < pal.Entries.Length; i++ )
                {
                    var col = pal.Entries[ i ];
                    if ( col.A != 255 )
                    {
                        // Color palettes should only have one index acting as transparency. Not sure if there's a better way of getting it...
                        transCol = i;
                        break;
                    }
                }

                // none of the entries in the palette have transparency information.
                if ( transCol == -1 )
                    return false;
                // Check pixels for existence of the transparent index.
                int colDepth = Image.GetPixelFormatSize( bitmap.PixelFormat );
                var data = bitmap.LockBits( new Rectangle( 0, 0, bitmap.Width, bitmap.Height ), ImageLockMode.ReadOnly,
                    bitmap.PixelFormat );
                int stride = data.Stride;
                var bytes = new byte[ bitmap.Height * stride ];
                Marshal.Copy( data.Scan0, bytes, 0, bytes.Length );
                bitmap.UnlockBits( data );
                if ( colDepth == 8 )
                {
                    // Last line index.
                    int lineMax = bitmap.Width - 1;
                    for ( int i = 0; i < bytes.Length; i++ )
                    {
                        // Last position to process.
                        int linepos = i % stride;
                        // Passed last image byte of the line. Abort and go on with loop.
                        if ( linepos > lineMax )
                            continue;
                        byte b = bytes[ i ];
                        if ( b == transCol )
                            return true;
                    }
                }
                else if ( colDepth == 4 )
                {
                    // line size in bytes. 1-indexed for the moment.
                    int lineMax = bitmap.Width / 2;
                    // Check if end of line ends on half a byte.
                    bool halfByte = bitmap.Width % 2 != 0;
                    // If it ends on half a byte, one more needs to be processed.
                    // We subtract in the other case instead, to make it 0-indexed right away.
                    if ( !halfByte )
                        lineMax--;
                    for ( int i = 0; i < bytes.Length; i++ )
                    {
                        // Last position to process.
                        int linepos = i % stride;
                        // Passed last image byte of the line. Abort and go on with loop.
                        if ( linepos > lineMax )
                            continue;
                        byte b = bytes[ i ];
                        if ( ( b & 0x0F ) == transCol )
                            return true;
                        if ( halfByte && linepos == lineMax
                        ) // reached last byte of the line. If only half a byte to check on that, abort and go on with loop.
                            continue;
                        if ( ( b & 0xF0 ) >> 4 == transCol )
                            return true;
                    }
                }

                return false;
            }

            if ( bitmap.PixelFormat == PixelFormat.Format32bppArgb ||
                 bitmap.PixelFormat == PixelFormat.Format32bppPArgb )
            {
                var data = bitmap.LockBits( new Rectangle( 0, 0, bitmap.Width, bitmap.Height ), ImageLockMode.ReadOnly,
                    bitmap.PixelFormat );
                var bytes = new byte[ bitmap.Height * data.Stride ];
                Marshal.Copy( data.Scan0, bytes, 0, bytes.Length );
                bitmap.UnlockBits( data );
                for ( int p = 3; p < bytes.Length; p += 4 )
                    if ( bytes[ p ] != 255 )
                        return true;
                return false;
            }

            // Final "screw it all" method. This is pretty slow, but it won't ever be used, unless you
            // encounter some really esoteric types not handled above, like 16bppArgb1555 and 64bppArgb.
            for ( int i = 0; i < bitmap.Width; i++ )
            for ( int j = 0; j < bitmap.Height; j++ )
                if ( bitmap.GetPixel( i, j ).A != 255 )
                    return true;
            return false;
        }
    }
}