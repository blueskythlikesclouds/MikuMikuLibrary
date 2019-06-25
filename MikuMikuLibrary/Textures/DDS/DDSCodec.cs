//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MikuMikuLibrary.Textures.DDS
{
    /// <summary>
    /// DDS codec for decompressing and compressing bitmap data to DDS formats. Credit to aelan/KFreon for implementation.
    /// </summary>
    public static class DDSCodec
    {
        /// <summary>
        /// Decompress a DDS image file and output an RGBA bitmap.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static Bitmap DecompressImage( string filepath )
        {
            return DecompressImage( File.ReadAllBytes( filepath ) );
        }

        /// <summary>
        /// Decompress a DDS image stream and output an RGBA bitmap.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Bitmap DecompressImage( Stream stream )
        {
            // TODO: not make a copy
            var bytes = new byte[ stream.Length ];
            stream.Read( bytes, 0, ( int )stream.Length );
            return DecompressImage( bytes );
        }

        /// <summary>
        /// Decompress a DDS image and output an RGBA bitmap.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bitmap DecompressImage( byte[] image )
        {
            var header = new DDSHeader( image );
            var newData = DecompressImageData( image, header.Width, header.Height, header.PixelFormat.FourCC, true );
            return WriteRBGAToBitmap( header.Width, header.Height, newData );
        }

        /// <summary>
        /// Decompress a DDS image and save it to a file.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="outFilepath"></param>
        public static void DecompressImageToFile( string filepath, string outFilepath )
        {
            var bitmap = DecompressImage( filepath );
            bitmap.Save( outFilepath );
        }

        /// <summary>
        /// Decompress a DDS image stream and save it to a file.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="outFilepath"></param>
        public static void DecompressImageToFile( Stream stream, string outFilepath )
        {
            var bitmap = DecompressImage( stream );
            bitmap.Save( outFilepath );
        }

        /// <summary>
        /// Decompress a DDS image file and save it to a stream.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static Stream DecompressImageToStream( string filepath, ImageFormat format )
        {
            var bitmap = DecompressImage( filepath );
            var outStream = new MemoryStream();
            bitmap.Save( outStream, format );
            return outStream;
        }

        /// <summary>
        /// Decompress a DDS image stream and save it a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Stream DecompressImageToStream( Stream stream, ImageFormat format )
        {
            var bitmap = DecompressImage( stream );
            var outStream = new MemoryStream();
            bitmap.Save( outStream, format );
            return outStream;
        }

        /// <summary>
        /// Decompress a DDS image and output RGBA data.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] DecompressImageToRGBA( byte[] image )
        {
            var header = new DDSHeader( image );
            var newData = DecompressImageData( image, header.Width, header.Height, header.PixelFormat.FourCC, true );
            return newData;
        }

        /// <summary>
        /// Decompress raw DDS pixel data to a bitmap.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Bitmap DecompressPixelData( byte[] data, int width, int height, DDSPixelFormatFourCC format )
        {
            var newData = DecompressImageData( data, width, height, format, false );
            return WriteRBGAToBitmap( width, height, newData );
        }

        /// <summary>
        /// Decompress raw DDS pixel data to an array of bytes containing RGBA data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static byte[] DecompressPixelDataToRGBA( byte[] data, int width, int height, DDSPixelFormatFourCC format )
        {
            return DecompressImageData( data, width, height, format, false );
        }

        /// <summary>
        /// Compress the bitmap into a DDS image.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static byte[] CompressImage( Bitmap image, DDSPixelFormatFourCC format )
        {
            return CompressImage( image, format, true );
        }

        /// <summary>
        /// Compress the bitmap into a DDS image.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] CompressImage( Bitmap image )
        {
            return CompressImage( image, DetermineBestCompressedFormat( image ) );
        }

        /// <summary>
        /// Compress the bitmap into a DDS image and save it to a file.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <param name="filepath"></param>
        public static void CompressImageToFile( Bitmap image, DDSPixelFormatFourCC format, string filepath )
        {
            var bytes = CompressImage( image, format );
            File.WriteAllBytes( filepath, bytes );
        }


        /// <summary>
        /// Compress the bitmap into a DDS image and save it to a file.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <param name="filepath"></param>
        public static void CompressImageToFile( Bitmap image, string filepath )
        {
            var bytes = CompressImage( image, DetermineBestCompressedFormat( image ) );
            File.WriteAllBytes( filepath, bytes );
        }

        /// <summary>
        /// Compress the bitmap into a DDS image and save it to a stream.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Stream CompressImageToStream( Bitmap image, DDSPixelFormatFourCC format )
        {
            var stream = new MemoryStream();
            CompressImageToStream( image, format, stream );
            return stream;
        }

        /// <summary>
        /// Compress the bitmap into a DDS image and save it to a stream.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Stream CompressImageToStream( Bitmap image )
        {
            var stream = new MemoryStream();
            CompressImageToStream( image, DetermineBestCompressedFormat( image ), stream );
            return stream;
        }

        /// <summary>
        /// Compress the bitmap into a DDS image and save it to a stream.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <param name="outStream"></param>
        public static void CompressImageToStream( Bitmap image, DDSPixelFormatFourCC format, Stream outStream )
        {
            var bytes = CompressImage( image, format );
            outStream.Write( bytes, 0, bytes.Length );
        }

        /// <summary>
        /// Compress the bitmap into a DDS image and save it to a stream.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <param name="outStream"></param>
        public static void CompressImageToStream( Bitmap image, Stream outStream )
        {
            var bytes = CompressImage( image, DetermineBestCompressedFormat( image ) );
            outStream.Write( bytes, 0, bytes.Length );
        }

        /// <summary>
        /// Compress the bitmap into DDS pixel data (does not include header).
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static byte[] CompressPixelData( Bitmap image, DDSPixelFormatFourCC format )
        {
            return CompressImage( image, format, false );
        }

        public static Boolean HasTransparency( Bitmap bitmap )
        {
            // not an alpha-capable color format.
            if ( ( bitmap.Flags & ( Int32 )ImageFlags.HasAlpha ) == 0 )
                return false;
            // Indexed formats. Special case because one index on their palette is configured as THE transparent color.
            if ( bitmap.PixelFormat == PixelFormat.Format8bppIndexed || bitmap.PixelFormat == PixelFormat.Format4bppIndexed )
            {
                ColorPalette pal = bitmap.Palette;
                // Find the transparent index on the palette.
                Int32 transCol = -1;
                for ( int i = 0; i < pal.Entries.Length; i++ )
                {
                    Color col = pal.Entries[ i ];
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
                Int32 colDepth = Image.GetPixelFormatSize( bitmap.PixelFormat );
                BitmapData data = bitmap.LockBits( new Rectangle( 0, 0, bitmap.Width, bitmap.Height ), ImageLockMode.ReadOnly, bitmap.PixelFormat );
                Int32 stride = data.Stride;
                Byte[] bytes = new Byte[ bitmap.Height * stride ];
                Marshal.Copy( data.Scan0, bytes, 0, bytes.Length );
                bitmap.UnlockBits( data );
                if ( colDepth == 8 )
                {
                    // Last line index.
                    Int32 lineMax = bitmap.Width - 1;
                    for ( Int32 i = 0; i < bytes.Length; i++ )
                    {
                        // Last position to process.
                        Int32 linepos = i % stride;
                        // Passed last image byte of the line. Abort and go on with loop.
                        if ( linepos > lineMax )
                            continue;
                        Byte b = bytes[ i ];
                        if ( b == transCol )
                            return true;
                    }
                }
                else if ( colDepth == 4 )
                {
                    // line size in bytes. 1-indexed for the moment.
                    Int32 lineMax = bitmap.Width / 2;
                    // Check if end of line ends on half a byte.
                    Boolean halfByte = bitmap.Width % 2 != 0;
                    // If it ends on half a byte, one more needs to be processed.
                    // We subtract in the other case instead, to make it 0-indexed right away.
                    if ( !halfByte )
                        lineMax--;
                    for ( Int32 i = 0; i < bytes.Length; i++ )
                    {
                        // Last position to process.
                        Int32 linepos = i % stride;
                        // Passed last image byte of the line. Abort and go on with loop.
                        if ( linepos > lineMax )
                            continue;
                        Byte b = bytes[ i ];
                        if ( ( b & 0x0F ) == transCol )
                            return true;
                        if ( halfByte && linepos == lineMax ) // reached last byte of the line. If only half a byte to check on that, abort and go on with loop.
                            continue;
                        if ( ( b & 0xF0 ) >> 4 == transCol )
                            return true;
                    }
                }
                return false;
            }
            if ( bitmap.PixelFormat == PixelFormat.Format32bppArgb || bitmap.PixelFormat == PixelFormat.Format32bppPArgb )
            {
                BitmapData data = bitmap.LockBits( new Rectangle( 0, 0, bitmap.Width, bitmap.Height ), ImageLockMode.ReadOnly, bitmap.PixelFormat );
                Byte[] bytes = new Byte[ bitmap.Height * data.Stride ];
                Marshal.Copy( data.Scan0, bytes, 0, bytes.Length );
                bitmap.UnlockBits( data );
                for ( Int32 p = 3; p < bytes.Length; p += 4 )
                {
                    if ( bytes[ p ] != 255 )
                        return true;
                }
                return false;
            }
            // Final "screw it all" method. This is pretty slow, but it won't ever be used, unless you
            // encounter some really esoteric types not handled above, like 16bppArgb1555 and 64bppArgb.
            for ( Int32 i = 0; i < bitmap.Width; i++ )
            {
                for ( Int32 j = 0; j < bitmap.Height; j++ )
                {
                    if ( bitmap.GetPixel( i, j ).A != 255 )
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determine the best compressed DDS pixel format for a given bitmap.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static DDSPixelFormatFourCC DetermineBestCompressedFormat( Bitmap bitmap )
        {
            var ddsFormat = DDSPixelFormatFourCC.DXT1;
            if ( HasTransparency( bitmap ) )
            {
                ddsFormat = DDSPixelFormatFourCC.DXT5;
            }

            return ddsFormat;
        }

        private static Bitmap WriteRBGAToBitmap( int width, int height, byte[] data )
        {
            var bitmap = new Bitmap( width, height, PixelFormat.Format32bppArgb );
            var bitmapData = bitmap.LockBits( new Rectangle( 0, 0, bitmap.Width, bitmap.Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );
            Marshal.Copy( data, 0, bitmapData.Scan0, data.Length );
            bitmap.UnlockBits( bitmapData );

            return bitmap;
        }

        private static byte[] DecompressImageData( byte[] data, int width, int height, DDSPixelFormatFourCC format, bool hasHeader )
        {
            var newData = new byte[ width * height * 4 ];
            int blockSize = DDSFormatDetails.GetBlockSize( format );
            int compressedLineSize = blockSize * width / 4;
            int bitsPerScanline = 4 * width;
            int texelCount = height / 4;

            MemoryStream compressed;
            if ( hasHeader )
                compressed = new MemoryStream( data, 0x80, data.Length - 0x80 );
            else
                compressed = new MemoryStream( data );

            Func<Stream, List<byte[]>> decompressBlock = null;
            switch ( format )
            {
                case DDSPixelFormatFourCC.DXT1:
                    decompressBlock = DecompressBC1Block;
                    break;
                case DDSPixelFormatFourCC.DXT3:
                    decompressBlock = DecompressBC2Block;
                    break;
                case DDSPixelFormatFourCC.DXT5:
                    decompressBlock = DecompressBC3Block;
                    break;
                case DDSPixelFormatFourCC.ATI1:
                    decompressBlock = DecompressBC4;
                    break;
                case DDSPixelFormatFourCC.ATI2N_3Dc:
                    decompressBlock = DecompressBC5Block;
                    break;
                default:
                    throw new NotImplementedException();
            }

            for ( var row = 0; row < texelCount; row++ )
            {
                using ( var decompressedLine = ReadBCMipLine( compressed, height, width, bitsPerScanline, 0, compressedLineSize, row, decompressBlock ) )
                {
                    int index = row * bitsPerScanline * 4;
                    decompressedLine.Position = 0;
                    int length = decompressedLine.Length > newData.Length ? newData.Length : ( int )decompressedLine.Length;
                    if ( index + length <= newData.Length )
                        decompressedLine.Read( newData, index, length );
                }
            }

            return newData;
        }

        private static byte[] CompressImage( Bitmap image, DDSPixelFormatFourCC format, bool includeHeader )
        {
            var imageData = image.LockBits( new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );

            Func<byte[], byte[]> compressor = null;

            switch ( format )
            {
                case DDSPixelFormatFourCC.DXT1:
                    compressor = CompressBC1Block;
                    break;
                case DDSPixelFormatFourCC.DXT3:
                    compressor = CompressBC2Block;
                    break;
                case DDSPixelFormatFourCC.DXT5:
                    compressor = CompressBC3Block;
                    break;
                case DDSPixelFormatFourCC.ATI1:
                    compressor = CompressBC4Block;
                    break;
                case DDSPixelFormatFourCC.ATI2N_3Dc:
                    compressor = CompressBC5Block;
                    break;
                default:
                    throw new NotImplementedException( "Unhandled compression format" );
            }

            Action<Stream, Stream, int, int> pixelWriter = ( writer, pixels, width, height ) =>
            {
                var texel = GetTexel( pixels, width, height );
                var compressedBlock = compressor( texel );
                writer.Write( compressedBlock, 0, compressedBlock.Length );
            };

            var outStream = new MemoryStream();
            if ( includeHeader )
                new DDSHeader( image.Width, image.Height, format ).Save( outStream );

            unsafe
            {
                using ( var mipmap = new UnmanagedMemoryStream( ( byte* )imageData.Scan0, imageData.Stride * image.Height ) )
                using ( var compressed = WriteMipMap( outStream, mipmap, image.Width, image.Height, pixelWriter, true ) )
                {
                    image.UnlockBits( imageData );
                    return outStream.GetBuffer();
                }
            }
        }

        private static MemoryStream ReadBCMipLine( Stream compressed, int mipHeight, int mipWidth, int bitsPerScanLine, long mipOffset, int compressedLineSize, int rowIndex, Func<Stream, List<byte[]>> decompressBlock )
        {
            var bitsPerPixel = 4;

            var decompressedLine = new MemoryStream( bitsPerScanLine * 4 );

            // KFreon: Read compressed line into new stream for multithreading purposes
            using ( var compressedLine = new MemoryStream( compressedLineSize ) )
            {
                lock ( compressed )
                {
                    // KFreon: Seek to correct texel
                    compressed.Position = mipOffset + rowIndex * compressedLineSize;  // +128 = header size

                    // KFreon: since mip count is an estimate, check to see if there are any mips left to read.
                    if ( compressed.Position >= compressed.Length )
                        return null;

                    var buffer = new byte[ 4096 ];
                    int read;
                    do
                    {
                        read = compressed.Read( buffer, 0, ( int )Math.Min( 4096, compressedLineSize ) );
                        compressedLineSize -= read;
                        compressedLine.Write( buffer, 0, read );

                    } while ( compressedLineSize > 0 );
                }
                compressedLine.Position = 0;

                // KFreon: Read texels in row
                for ( var column = 0; column < mipWidth; column += 4 )
                {
                    // decompress 
                    var decompressed = decompressBlock( compressedLine );
                    var blue = decompressed[ 0 ];
                    var green = decompressed[ 1 ];
                    var red = decompressed[ 2 ];
                    var alpha = decompressed[ 3 ];

                    // Write texel
                    int topLeft = column * bitsPerPixel;// + rowIndex * 4 * bitsPerScanLine;  // Top left corner of texel IN BYTES (i.e. expanded pixels to 4 channels)
                    decompressedLine.Seek( topLeft, SeekOrigin.Begin );
                    var block = new byte[ 16 ];
                    for ( var i = 0; i < 16; i += 4 )
                    {
                        // BGRA
                        for ( var j = 0; j < 16; j += 4 )
                        {
                            block[ j ] = blue[ i + ( j >> 2 ) ];
                            block[ j + 1 ] = green[ i + ( j >> 2 ) ];
                            block[ j + 2 ] = red[ i + ( j >> 2 ) ];
                            block[ j + 3 ] = alpha[ i + ( j >> 2 ) ];
                        }
                        decompressedLine.Write( block, 0, 16 );

                        // Go one line of pixels down (bitsPerScanLine), then to the left side of the texel (4 pixels back from where it finished)
                        decompressedLine.Seek( bitsPerScanLine - bitsPerPixel * 4, SeekOrigin.Current );
                    }
                }
            }

            return decompressedLine;
        }

        private static MemoryStream WriteMipMap( MemoryStream outStream, Stream pixelData, int width, int height, Action<Stream, Stream, int, int> pixelWriter, bool isBCd )
        {
            var outStreamOrigin = outStream.Position;
            int bitsPerScanLine = isBCd ? 4 * width : width;  // KFreon: Bits per image line.

            // KFreon: Loop over rows and columns, doing extra moving if Block Compressed to accommodate texels.
            int texelCount = isBCd ? height / 4 : height;
            var compressedLineSize = 0;
            if ( texelCount == 0 )
            {
                // ignore for now...
                outStream.Write( new byte[ bitsPerScanLine ], 0, bitsPerScanLine ); // hopefully long enough to end it
                return outStream;
            }

            for ( var rowIndex = 0; rowIndex < texelCount; rowIndex++ )
            {
                using ( var compressedLine = WriteMipLine( pixelData, width, height, bitsPerScanLine, isBCd, rowIndex, pixelWriter ) )
                {
                    if ( compressedLine == null )
                        break;

                    lock ( outStream )
                    {
                        // KFreon: Detect size of a compressed line
                        if ( compressedLineSize == 0 )
                            compressedLineSize = ( int )compressedLine.Length;

                        outStream.Seek( outStreamOrigin + rowIndex * compressedLineSize, SeekOrigin.Begin );
                        compressedLine.WriteTo( outStream );
                    }
                }
            }

            return outStream;
        }

        private static MemoryStream WriteMipLine( Stream pixelData, int width, int height, int bitsPerScanLine, bool isBCd, int rowIndex, Action<Stream, Stream, int, int> pixelWriter )
        {
            var compressedLine = new MemoryStream( bitsPerScanLine ); // Not correct compressed size but it's close enough to not have it waste tonnes of time copying.
            using ( var uncompressedLine = new MemoryStream( 4 * bitsPerScanLine ) )
            {
                lock ( pixelData )
                {
                    // KFreon: Ensure we're in the right place
                    pixelData.Position = rowIndex * 4 * bitsPerScanLine;  // Uncompressed location

                    // KFreon: since mip count is an estimate, check to see if there are any mips left to read.
                    if ( pixelData.Position >= pixelData.Length )
                        return null;

                    // KFreon: Read compressed line
                    int length = 4 * bitsPerScanLine;

                    var buffer = new byte[ 4096 ];
                    int read;
                    do
                    {
                        read = pixelData.Read( buffer, 0, ( int )Math.Min( 4096, length ) );
                        length -= read;
                        uncompressedLine.Write( buffer, 0, read );

                    } while ( length > 0 );

                    uncompressedLine.Position = 0;
                }

                for ( var w = 0; w < width; w += isBCd ? 4 : 1 )
                {
                    pixelWriter( compressedLine, uncompressedLine, width, height );
                    if ( isBCd && w != width - 4 && width > 4 && height > 4 )  // KFreon: Only do this if dimensions are big enough
                        uncompressedLine.Seek( -( bitsPerScanLine * 4 ) + 4 * 4, SeekOrigin.Current );  // Not at an row end texel. Moves back up to read next texel in row.
                }
            }

            return compressedLine;
        }

        /// <summary>
        /// Decompresses an 8 bit channel.
        /// </summary>
        /// <param name="compressed">Compressed image data.</param>
        /// <param name="isSigned">true = use signed alpha range (-254 -- 255), false = 0 -- 255</param>
        /// <returns>Single channel decompressed (16 bits).</returns>
        internal static byte[] Decompress8BitBlock( Stream compressed, bool isSigned )
        {
            var decompressedBlock = new byte[ 16 ];

            // KFreon: Read min and max colours (not necessarily in that order)
            var block = new byte[ 8 ];
            compressed.Read( block, 0, 8 );

            byte min = block[ 0 ];
            byte max = block[ 1 ];

            var colours = Build8BitPalette( min, max, isSigned );

            // KFreon: Decompress pixels
            ulong bitmask = ( ulong )block[ 2 ] << 0 | ( ulong )block[ 3 ] << 8 | ( ulong )block[ 4 ] << 16 |   // KFreon: Read all 6 compressed bytes into single 
                ( ulong )block[ 5 ] << 24 | ( ulong )block[ 6 ] << 32 | ( ulong )block[ 7 ] << 40;


            // KFreon: Bitshift and mask compressed data to get 3 bit indicies, and retrieve indexed colour of pixel.
            for ( var i = 0; i < 16; i++ )
                decompressedBlock[ i ] = ( byte )colours[ bitmask >> ( i * 3 ) & 0x7 ];

            return decompressedBlock;
        }

        /// <summary>
        /// Reads a packed DXT colour into RGB
        /// </summary>
        /// <param name="colour">Colour to convert to RGB</param>
        /// <returns>RGB bytes</returns>
        private static byte[] ReadDXTColour( int color )
        {
            int temp = ( color >> 11 ) * 255 + 16;
            byte r = ( byte ) ( ( temp / 32 + temp ) / 32 );
            temp = ( ( color & 0x07E0 ) >> 5 ) * 255 + 32;
            byte g = ( byte ) ( ( temp / 64 + temp ) / 64 );
            temp = ( color & 0x001F ) * 255 + 16;
            byte b = ( byte ) ( ( temp / 32 + temp ) / 32 );
            return new byte[ 3 ] { r, g, b };
        }

        /// <summary>
        /// Creates a packed DXT colour from RGB.
        /// </summary>
        /// <param name="r">Red byte.</param>
        /// <param name="g">Green byte.</param>
        /// <param name="b">Blue byte.</param>
        /// <returns>DXT Colour</returns>
        private static int BuildDXTColour( byte r, byte g, byte b )
        {
            // Compress to 5:6:5
            var r1 = ( byte )( r >> 3 );
            var g1 = ( byte )( g >> 2 );
            var b1 = ( byte )( b >> 3 );

            return ( r1 << 11 ) | ( g1 << 5 ) | b1;
        }

        /// <summary>
        /// Builds an RGB palette from the min and max colours of a texel.
        /// </summary>
        /// <param name="colour0">First colour, usually the min.</param>
        /// <param name="colour1">Second colour, usually the max.</param>
        /// <param name="isDxt1">True = for DXT1 texels. Changes how the internals are calculated.</param>
        /// <returns>Texel palette.</returns>
        private static int[] BuildRGBPalette( int colour0, int colour1, bool isDxt1 )
        {
            var colours = new int[ 4 ];

            colours[ 0 ] = colour0;
            colours[ 1 ] = colour1;

            var colour0S = ReadDXTColour( colour0 );
            var colour1S = ReadDXTColour( colour1 );


            // Interpolate other 2 colours
            if ( colour0 > colour1 )
            {
                var r1 = ( byte )( 2f / 3f * colour0S[ 0 ] + 1f / 3f * colour1S[ 0 ] );
                var g1 = ( byte )( 2f / 3f * colour0S[ 1 ] + 1f / 3f * colour1S[ 1 ] );
                var b1 = ( byte )( 2f / 3f * colour0S[ 2 ] + 1f / 3f * colour1S[ 2 ] );

                var r2 = ( byte )( 1f / 3f * colour0S[ 0 ] + 2f / 3f * colour1S[ 0 ] );
                var g2 = ( byte )( 1f / 3f * colour0S[ 1 ] + 2f / 3f * colour1S[ 1 ] );
                var b2 = ( byte )( 1f / 3f * colour0S[ 2 ] + 2f / 3f * colour1S[ 2 ] );

                colours[ 2 ] = BuildDXTColour( r1, g1, b1 );
                colours[ 3 ] = BuildDXTColour( r2, g2, b2 );
            }
            else
            {
                // KFreon: Only for dxt1
                var r = ( byte )( 1 / 2f * colour0S[ 0 ] + 1 / 2f * colour1S[ 0 ] );
                var g = ( byte )( 1 / 2f * colour0S[ 1 ] + 1 / 2f * colour1S[ 1 ] );
                var b = ( byte )( 1 / 2f * colour0S[ 2 ] + 1 / 2f * colour1S[ 2 ] );

                colours[ 2 ] = BuildDXTColour( r, g, b );
                colours[ 3 ] = 0;
            }
            return colours;
        }

        /// <summary>
        /// Decompresses a 3 channel (RGB) block.
        /// </summary>
        /// <param name="compressed">Compressed image data.</param>
        /// <param name="isDxt1">True = DXT1, otherwise false.</param>
        /// <returns>16 pixel BGRA channels.</returns>
        private static List<byte[]> DecompressRGBBlock( Stream compressed, bool isDxt1 )
        {
            var decompressedBlock = new int[ 16 ];

            ushort colour0;
            ushort colour1;
            byte[] pixels = null;
            int[] colours = null;

            var decompressedChannels = new List<byte[]>( 4 );
            var red = new byte[ 16 ];
            var green = new byte[ 16 ];
            var blue = new byte[ 16 ];
            var alpha = new byte[ 16 ];
            decompressedChannels.Add( blue );
            decompressedChannels.Add( green );
            decompressedChannels.Add( red );
            decompressedChannels.Add( alpha );

            try
            {
                using ( var reader = new BinaryReader( compressed, Encoding.Default, true ) )
                {
                    // Read min max colours
                    colour0 = ( ushort )reader.ReadInt16();
                    colour1 = ( ushort )reader.ReadInt16();
                    colours = BuildRGBPalette( colour0, colour1, isDxt1 );

                    // Decompress pixels
                    pixels = reader.ReadBytes( 4 );
                    if ( pixels.Length != 4 )
                        return decompressedChannels;
                }
            }
            catch //(EndOfStreamException e)
            {
                return decompressedChannels;
            }



            for ( var i = 0; i < 16; i += 4 )
            {
                //byte bitmask = (byte)compressed.ReadByte();
                byte bitmask = pixels[ i / 4 ];
                for ( var j = 0; j < 4; j++ )
                    decompressedBlock[ i + j ] = colours[ bitmask >> ( 2 * j ) & 0x03 ];
            }

            // KFreon: Decode into BGRA
            for ( var i = 0; i < 16; i++ )
            {
                int colour = decompressedBlock[ i ];
                var rgb = ReadDXTColour( colour );
                red[ i ] = rgb[ 0 ];
                green[ i ] = rgb[ 1 ];
                blue[ i ] = rgb[ 2 ];
                alpha[ i ] = 0xFF;
            }
            return decompressedChannels;
        }

        /// <summary>
        /// Read an 8 byte BC1 compressed block from stream.
        /// </summary>
        /// <param name="compressed">BC1 compressed stream.</param>
        /// <returns>BGRA channels.</returns>
        private static List<byte[]> DecompressBC1Block( Stream compressed )
        {
            return DecompressRGBBlock( compressed, true );
        }

        /// <summary>
        /// Compress texel to 8 byte BC1 compressed block.
        /// </summary>
        /// <param name="texel">4x4 BGRA group of pixels.</param>
        /// <returns>8 byte BC1 compressed block.</returns>
        private static byte[] CompressBC1Block( byte[] texel )
        {
            return CompressRGBTexel( texel, true, 0.2f );
        }

        /// <summary>
        /// Reads a 16 byte BC2 compressed block from stream.
        /// </summary>
        /// <param name="compressed">BC2 compressed stream.</param>
        /// <returns>BGRA channels.</returns>
        private static List<byte[]> DecompressBC2Block( Stream compressed )
        {
            // KFreon: Read alpha into byte[] for maximum speed? Might be cos it's a MemoryStream...
            var compressedAlphas = new byte[ 8 ];
            compressed.Read( compressedAlphas, 0, 8 );
            var count = 0;

            // KFreon: Read alpha
            var alpha = new byte[ 16 ];
            for ( var i = 0; i < 16; i += 2 )
            {
                //byte twoAlphas = (byte)compressed.ReadByte();
                byte twoAlphas = compressedAlphas[ count++ ];
                for ( var j = 0; j < 2; j++ )
                    alpha[ i + j ] = ( byte )( twoAlphas << ( j * 4 ) );
            }


            // KFreon: Organise output by adding alpha channel (channel read in RGB block is empty)
            var decompressedBlock = DecompressRGBBlock( compressed, false );
            decompressedBlock[ 3 ] = alpha;
            return decompressedBlock;
        }

        /// <summary>
        /// Reads a 16 byte BC3 compressed block from stream.
        /// </summary>
        /// <param name="compressed">BC3 compressed image stream.</param>
        /// <returns>List of BGRA channels.</returns>
        private static List<byte[]> DecompressBC3Block( Stream compressed )
        {
            var alpha = Decompress8BitBlock( compressed, false );
            var decompressedBlock = DecompressRGBBlock( compressed, false );
            decompressedBlock[ 3 ] = alpha;
            return decompressedBlock;
        }

        /// <summary>
        /// Decompresses an ATI1 (BC4) block.
        /// </summary>
        /// <param name="compressed">Compressed data stream.</param>
        /// <returns>BGRA channels (16 bits each)</returns>
        private static List<byte[]> DecompressBC4( Stream compressed )
        {
            var channel = Decompress8BitBlock( compressed, false );
            var decompressedBlock = new List<byte[]>();

            // KFreon: All channels are the same to make grayscale.
            decompressedBlock.Add( channel );
            decompressedBlock.Add( channel );
            decompressedBlock.Add( channel );

            // KFreon: Alpha needs to be 255
            var alpha = new byte[ 16 ];
            for ( var i = 0; i < 16; i++ )
                alpha[ i ] = 0xFF;
            decompressedBlock.Add( alpha );
            return decompressedBlock;
        }

        /// <summary>
        /// Decompresses ATI2 (BC5) block.
        /// </summary>
        /// <param name="compressed">Compressed data stream.</param>
        /// <returns>16 pixel BGRA channels.</returns>
        private static List<byte[]> DecompressBC5Block( Stream compressed )
        {
            var red = Decompress8BitBlock( compressed, false );
            var green = Decompress8BitBlock( compressed, false );
            var decompressedBlock = new List<byte[]>();



            // KFreon: Alpha needs to be 255
            var alpha = new byte[ 16 ];
            var blue = new byte[ 16 ];
            for ( var i = 0; i < 16; i++ )
            {
                alpha[ i ] = 0xFF;
                /*double r = red[i] / 255.0;
                double g = green[i] / 255.0;
                double test = 1 - (r * g);
                double anbs = Math.Sqrt(test);
                double ans = anbs * 255.0;*/
                blue[ i ] = ( byte )0xFF;
            }

            decompressedBlock.Add( blue );
            decompressedBlock.Add( green );
            decompressedBlock.Add( red );
            decompressedBlock.Add( alpha );

            return decompressedBlock;
        }

        /// <summary>
        /// Compress texel to 16 byte BC2 compressed block.
        /// </summary>
        /// <param name="texel">4x4 BGRA set of pixels.</param>
        /// <returns>16 byte BC2 compressed block.</returns>
        private static byte[] CompressBC2Block( byte[] texel )
        {
            // Compress Alpha
            var alpha = new byte[ 8 ];
            for ( var i = 3; i < 64; i += 8 )  // Only read alphas
            {
                byte twoAlpha = 0;
                for ( var j = 0; j < 8; j += 4 )
                    twoAlpha |= ( byte )( texel[ i + j ] << j );
                alpha[ i / 8 ] = twoAlpha;
            }

            // Compress Colour
            var rgb = CompressRGBTexel( texel, false, 0f );

            //return Alpha.Concat(RGB).ToArray(Alpha.Length + RGB.Length);
            return alpha.Concat( rgb ).ToArray();
        }

        /// <summary>
        /// Compress texel to 16 byte BC3 compressed block.
        /// </summary>
        /// <param name="texel">4x4 BGRA set of pixels.</param>
        /// <returns>16 byte BC3 compressed block.</returns>
        private static byte[] CompressBC3Block( byte[] texel )
        {
            // Compress Alpha
            var alpha = Compress8BitBlock( texel, 3, false );

            // Compress Colour
            var rgb = CompressRGBTexel( texel, false, 0f );

            return alpha.Concat( rgb ).ToArray();
            //return Alpha.Concat(RGB).ToArray(Alpha.Length + RGB.Length);
        }

        /// <summary>
        /// Compress texel to 8 byte BC4 compressed block.
        /// </summary>
        /// <param name="texel">4x4 BGRA set of pixels.</param>
        /// <returns>8 byte BC4 compressed block.</returns>
        private static byte[] CompressBC4Block( byte[] texel )
        {
            return Compress8BitBlock( texel, 2, false );
        }

        /// <summary>
        /// Compresses texel to 16 byte BC5 block.
        /// </summary>
        /// <param name="texel">4x4 BGRA set of pixels.</param>
        /// <returns>16 byte BC5 block.</returns>
        private static byte[] CompressBC5Block( byte[] texel )
        {
            var red = Compress8BitBlock( texel, 2, false );
            var green = Compress8BitBlock( texel, 1, false );

            return red.Concat( green ).ToArray();
            //return red.Concat(green).ToArray(red.Length + green.Length);
        }

        private static int GetClosestValue( byte[] arr, byte c )
        {
            int min = int.MaxValue;
            var index = 0;
            var minIndex = 0;
            for ( var i = 0; i < arr.Length; i++ )
            {
                int check = arr[ i ] - c;
                check = ( check ^ ( check >> 7 ) ) - ( check >> 7 );
                if ( check < min )
                {
                    min = check;
                    minIndex = index;
                }

                index++;
            }
            return minIndex;
        }

        /// <summary>
        /// Gets 4x4 texel block from stream.
        /// </summary>
        /// <param name="pixelData">Image pixels.</param>
        /// <param name="width">Width of image.</param>
        /// <param name="height">Height of image.</param>
        /// <returns>4x4 texel.</returns>
        private static byte[] GetTexel( Stream pixelData, int width, int height )
        {
            var texel = new byte[ 16 * 4 ]; // 16 pixels, 4 bytes per pixel

            // KFreon: Edge case for when dimensions are too small for texel
            var count = 0;
            if ( width < 4 || height < 4 )
            {
                for ( var h = 0; h < height; h++ )
                    for ( var w = 0; w < width; w++ )
                        for ( var i = 0; i < 4; i++ )
                        {
                            if ( count >= 64 )
                                return texel;
                            else
                                texel[ count++ ] = ( byte )pixelData.ReadByte();
                        }

                return texel;
            }

            // KFreon: Normal operation. Read 4x4 texel row by row.
            int bitsPerScanLine = 4 * width;
            for ( var i = 0; i < 64; i += 16 )  // pixel rows
            {
                pixelData.Read( texel, i, 16 );
                /*for (int j = 0; j < 16; j += 4)  // pixels in row
                    for (int k = 0; k < 4; k++) // BGRA
                        texel[i + j + k] = (byte)pixelData.ReadByte();*/

                pixelData.Seek( bitsPerScanLine - 4 * 4, SeekOrigin.Current );  // Seek to next line of texel
            }


            return texel;
        }


        /// <summary>
        /// Builds palette for 8 bit channel.
        /// </summary>
        /// <param name="min">First main colour (often actually minimum)</param>
        /// <param name="max">Second main colour (often actually maximum)</param>
        /// <param name="isSigned">true = sets signed alpha range (-254 -- 255), false = 0 -- 255</param>
        /// <returns>8 byte colour palette.</returns>
        private static byte[] Build8BitPalette( byte min, byte max, bool isSigned )
        {
            var colours = new byte[ 8 ];
            colours[ 0 ] = min;
            colours[ 1 ] = max;

            // KFreon: Choose which type of interpolation is required
            if ( min > max )
            {
                // KFreon: Interpolate other colours
                for ( var i = 2; i < 8; i++ )
                {
                    double test = min + ( max - min ) * ( i - 1 ) / 7;
                    colours[ i ] = ( byte )test;
                }
            }
            else
            {
                // KFreon: Interpolate other colours and add Opacity or something...
                for ( var i = 2; i < 6; i++ )
                {
                    //double test = ((8 - i) * min + (i - 1) * max) / 5.0f;   // KFreon: "Linear interpolation". Serves me right for trusting a website without checking it...
                    double extratest = min + ( max - min ) * ( i - 1 ) / 5;
                    colours[ i ] = ( byte )extratest;
                }
                colours[ 6 ] = ( byte )( isSigned ? -254 : 0 );  // KFreon: snorm and unorm have different alpha ranges
                colours[ 7 ] = 255;
            }

            return colours;
        }


        /// <summary>
        /// Compresses single channel using Block Compression.
        /// </summary>
        /// <param name="texel">4 channel Texel to compress.</param>
        /// <param name="channel">0-3 (BGRA)</param>
        /// <param name="isSigned">true = uses alpha range -255 -- 255, else 0 -- 255</param>
        /// <returns>8 byte compressed texel.</returns>
        private static byte[] Compress8BitBlock( byte[] texel, int channel, bool isSigned )
        {
            // KFreon: Get min and max
            byte min = byte.MaxValue;
            byte max = byte.MinValue;
            int count = channel;
            for ( var i = 0; i < 16; i++ )
            {
                byte colour = texel[ count ];
                if ( colour > max )
                    max = colour;
                else if ( colour < min )
                    min = colour;

                count += 4; // skip to next entry in channel
            }

            // Build Palette
            var colours = Build8BitPalette( min, max, isSigned );

            // Compress Pixels
            ulong line = 0;
            count = channel;
            var indicies = new List<int>();
            for ( var i = 0; i < 16; i++ )
            {
                byte colour = texel[ count ];
                int index = GetClosestValue( colours, colour );
                indicies.Add( index );
                line |= ( ulong )index << ( i * 3 );
                count += 4;  // Only need 1 channel
            }

            var compressedBlock = new byte[ 8 ];
            var compressed = BitConverter.GetBytes( line );
            compressedBlock[ 0 ] = min;
            compressedBlock[ 1 ] = max;
            for ( var i = 2; i < 8; i++ )
                compressedBlock[ i ] = compressed[ i - 2 ];

            return compressedBlock;
        }

        private struct RGBColour
        {
            public float R, G, B, A;

            public RGBColour( float red, float green, float blue, float alpha )
            {
                R = red;
                G = green;
                B = blue;
                A = alpha;
            }
        }

        private static readonly float[] sPC3 = { 1f, 1f / 2f, 0f };
        private static readonly float[] sPD3 = { 0f, 1f / 2f, 1f };

        private static readonly float[] sPC4 = { 1f, 2f / 3f, 1f / 3f, 0f };
        private static readonly float[] sPD4 = { 0f, 1f / 3f, 2f / 3f, 1f };

        private static readonly uint[] sPsteps3 = { 0, 2, 1 };
        private static readonly uint[] sPsteps4 = { 0, 2, 3, 1 };

        private static RGBColour sLuminance = new RGBColour( 0.2125f / 0.7154f, 1f, 0.0721f / 0.7154f, 1f );
        private static RGBColour sLuminanceInv = new RGBColour( 0.7154f / 0.2125f, 1f, 0.7154f / 0.0721f, 1f );

        private static RGBColour Decode565( uint wColour )
        {
            var colour = new RGBColour();
            colour.R = ( ( wColour >> 11 ) & 31 ) * ( 1f / 31f );
            colour.G = ( ( wColour >> 5 ) & 63 ) * ( 1f / 63f );
            colour.B = ( ( wColour >> 0 ) & 31 ) * ( 1f / 31f );
            colour.A = 1f;

            return colour;
        }

        private static uint Encode565( RGBColour colour )
        {
            var temp = new RGBColour();
            temp.R = colour.R < 0f ? 0f : colour.R > 1f ? 1f : colour.R;
            temp.G = colour.G < 0f ? 0f : colour.G > 1f ? 1f : colour.G;
            temp.B = colour.B < 0f ? 0f : colour.B > 1f ? 1f : colour.B;

            return ( uint )( temp.R * 31f + 0.5f ) << 11 | ( uint )( temp.G * 63f + 0.5f ) << 5 | ( uint )( temp.B * 31f + 0.5f );
        }

        private static RGBColour ReadColourFromTexel( byte[] texel, int i )
        {
            // Pull out rgb from texel
            byte r = texel[ i + 2 ];
            byte g = texel[ i + 1 ];
            byte b = texel[ i ];
            byte a = texel[ i + 3 ];

            // Create current pixel colour
            var current = new RGBColour();
            current.R = r / 255f;
            current.G = g / 255f;
            current.B = b / 255f;
            current.A = a / 255f;

            return current;
        }

        private static RGBColour[] OptimiseRGB( RGBColour[] colour, int uSteps )
        {
            var pC = uSteps == 3 ? sPC3 : sPC4;
            var pD = uSteps == 3 ? sPD3 : sPD4;

            // Find min max
            var x = sLuminance;
            var y = new RGBColour();

            for ( var i = 0; i < colour.Length; i++ )
            {
                var current = colour[ i ];

                // X = min, Y = max
                if ( current.R < x.R )
                    x.R = current.R;

                if ( current.G < x.G )
                    x.G = current.G;

                if ( current.B < x.B )
                    x.B = current.B;


                if ( current.R > y.R )
                    y.R = current.R;

                if ( current.G > y.G )
                    y.G = current.G;

                if ( current.B > y.B )
                    y.B = current.B;
            }

            // Diagonal axis - starts with difference between min and max
            var diag = new RGBColour();
            diag.R = y.R - x.R;
            diag.G = y.G - x.G;
            diag.B = y.B - x.B;

            float fDiag = diag.R * diag.R + diag.G * diag.G + diag.B * diag.B;
            if ( fDiag < 1.175494351e-38F )
            {
                var min1 = new RGBColour();
                min1.R = x.R;
                min1.G = x.G;
                min1.B = x.B;

                var max1 = new RGBColour();
                max1.R = y.R;
                max1.G = y.G;
                max1.B = y.B;

                return new RGBColour[] { min1, max1 };
            }

            float fdiagInv = 1f / fDiag;

            var dir = new RGBColour();
            dir.R = diag.R * fdiagInv;
            dir.G = diag.G * fdiagInv;
            dir.B = diag.B * fdiagInv;

            var mid = new RGBColour();
            mid.R = ( x.R + y.R ) * .5f;
            mid.G = ( x.G + y.G ) * .5f;
            mid.B = ( x.B + y.B ) * .5f;

            var fDir = new float[ 4 ];

            for ( var i = 0; i < colour.Length; i++ )
            {
                var pt = new RGBColour();
                pt.R = dir.R * ( colour[ i ].R - mid.R );
                pt.G = dir.G * ( colour[ i ].G - mid.G );
                pt.B = dir.B * ( colour[ i ].B - mid.B );

                float f = 0;
                f = pt.R + pt.G + pt.B;
                fDir[ 0 ] += f * f;

                f = pt.R + pt.G - pt.B;
                fDir[ 1 ] += f * f;

                f = pt.R - pt.G + pt.B;
                fDir[ 2 ] += f * f;

                f = pt.R - pt.G - pt.B;
                fDir[ 3 ] += f * f;
            }

            float fDirMax = fDir[ 0 ];
            var iDirMax = 0;
            for ( var iDir = 1; iDir < 4; iDir++ )
            {
                if ( fDir[ iDir ] > fDirMax )
                {
                    fDirMax = fDir[ iDir ];
                    iDirMax = iDir;
                }
            }

            if ( ( iDirMax & 2 ) != 0 )
            {
                float f = x.G;
                x.G = y.G;
                y.G = f;
            }

            if ( ( iDirMax & 1 ) != 0 )
            {
                float f = x.B;
                x.B = y.B;
                y.B = f;
            }

            if ( fDiag < 1f / 4096f )
            {
                var min1 = new RGBColour();
                min1.R = x.R;
                min1.G = x.G;
                min1.B = x.B;

                var max1 = new RGBColour();
                max1.R = y.R;
                max1.G = y.G;
                max1.B = y.B;


                return new RGBColour[] { min1, max1 };
            }

            // newtons method for local min of sum of squares error.
            float fsteps = uSteps - 1;
            for ( var iteration = 0; iteration < 8; iteration++ )
            {
                var pSteps = new RGBColour[ 4 ];

                for ( var iStep = 0; iStep < uSteps; iStep++ )
                {
                    pSteps[ iStep ].R = x.R * pC[ iStep ] + y.R * pD[ iStep ];
                    pSteps[ iStep ].G = x.G * pC[ iStep ] + y.G * pD[ iStep ];
                    pSteps[ iStep ].B = x.B * pC[ iStep ] + y.B * pD[ iStep ];
                }


                // colour direction
                dir.R = y.R - x.R;
                dir.G = y.G - x.G;
                dir.B = y.B - x.B;

                float fLen = dir.R * dir.R + dir.G * dir.G + dir.B * dir.B;

                if ( fLen < 1f / 4096f )
                    break;

                float fScale = fsteps / fLen;
                dir.R *= fScale;
                dir.G *= fScale;
                dir.B *= fScale;

                // Evaluate function and derivatives
                float d2X = 0, d2Y = 0;
                RGBColour dX, dY;
                dX = new RGBColour();
                dY = new RGBColour();

                for ( var i = 0; i < colour.Length; i++ )
                {
                    var current = colour[ i ];

                    float fDot = ( current.R - x.R ) * dir.R + ( current.G - x.G ) * dir.G + ( current.B - x.B ) * dir.B;

                    var iStep = 0;
                    if ( fDot <= 0 )
                        iStep = 0;
                    else if ( fDot >= fsteps )
                        iStep = uSteps - 1;
                    else
                        iStep = ( int )( fDot + .5f );

                    var diff = new RGBColour();
                    diff.R = pSteps[ iStep ].R - current.R;
                    diff.G = pSteps[ iStep ].G - current.G;
                    diff.B = pSteps[ iStep ].B - current.B;

                    float fC = pC[ iStep ] * 1f / 8f;
                    float fD = pD[ iStep ] * 1f / 8f;

                    d2X += fC * pC[ iStep ];
                    dX.R += fC * diff.R;
                    dX.G += fC * diff.G;
                    dX.B += fC * diff.B;

                    d2Y += fD * pD[ iStep ];
                    dY.R += fD * diff.R;
                    dY.G += fD * diff.G;
                    dY.B += fD * diff.B;
                }

                // Move endpoints
                if ( d2X > 0f )
                {
                    float f = -1f / d2X;
                    x.R += dX.R * f;
                    x.G += dX.G * f;
                    x.B += dX.B * f;
                }

                if ( d2Y > 0f )
                {
                    float f = -1f / d2Y;
                    y.R += dY.R * f;
                    y.G += dY.G * f;
                    y.B += dY.B * f;
                }

                float fEpsilon = 0.25f / 64.0f * ( 0.25f / 64.0f );
                if ( dX.R * dX.R < fEpsilon && dX.G * dX.G < fEpsilon && dX.B * dX.B < fEpsilon &&
                    dY.R * dY.R < fEpsilon && dY.G * dY.G < fEpsilon && dY.B * dY.B < fEpsilon )
                {
                    break;
                }
            }

            var min = new RGBColour();
            min.R = x.R;
            min.G = x.G;
            min.B = x.B;

            var max = new RGBColour();
            max.R = y.R;
            max.G = y.G;
            max.B = y.B;


            return new RGBColour[] { min, max };
        }


        private static byte[] CompressRGBTexel( byte[] texel, bool isDxt1, float alphaRef )
        {
            var dither = true;
            var uSteps = 4;

            // Determine if texel is fully and entirely transparent. If so, can set to white and continue.
            if ( isDxt1 )
            {
                var uColourKey = 0;

                // Alpha stuff
                for ( var i = 0; i < texel.Length; i += 4 )
                {
                    var texelColour = ReadColourFromTexel( texel, i );
                    if ( texelColour.A < alphaRef )
                        uColourKey++;
                }

                if ( uColourKey == 16 )
                {
                    // Entire texel is transparent

                    var retval1 = new byte[ 8 ];
                    retval1[ 2 ] = byte.MaxValue;
                    retval1[ 3 ] = byte.MaxValue;

                    retval1[ 4 ] = byte.MaxValue;
                    retval1[ 5 ] = byte.MaxValue;
                    retval1[ 6 ] = byte.MaxValue;
                    retval1[ 7 ] = byte.MaxValue;

                    return retval1;
                }

                uSteps = uColourKey > 0 ? 3 : 4;
            }

            var colour = new RGBColour[ 16 ];
            var error = new RGBColour[ 16 ];

            var index = 0;
            for ( var i = 0; i < texel.Length; i += 4 )
            {
                index = i / 4;
                var current = ReadColourFromTexel( texel, i );

                if ( dither )
                {
                    // Adjust for accumulated error
                    // This works by figuring out the error between the current pixel colour and the adjusted colour? Dunno what the adjustment is. Looks like a 5:6:5 range adaptation
                    // Then, this error is distributed across the "next" few pixels and not the previous.
                    current.R += error[ index ].R;
                    current.G += error[ index ].G;
                    current.B += error[ index ].B;
                }


                // 5:6:5 range adaptation?
                colour[ index ].R = ( int )( current.R * 31f + .5f ) * ( 1f / 31f );
                colour[ index ].G = ( int )( current.G * 63f + .5f ) * ( 1f / 63f );
                colour[ index ].B = ( int )( current.B * 31f + .5f ) * ( 1f / 31f );

                if ( dither )
                {
                    // Calculate difference between current pixel colour and adapted pixel colour?
                    var diff = new RGBColour();
                    diff.R = current.A * ( byte )( current.R - colour[ index ].R );
                    diff.G = current.A * ( byte )( current.G - colour[ index ].G );
                    diff.B = current.A * ( byte )( current.B - colour[ index ].B );

                    // If current pixel is not at the end of a row
                    if ( ( index & 3 ) != 3 )
                    {
                        error[ index + 1 ].R += diff.R * ( 7f / 16f );
                        error[ index + 1 ].G += diff.G * ( 7f / 16f );
                        error[ index + 1 ].B += diff.B * ( 7f / 16f );
                    }

                    // If current pixel is not in bottom row
                    if ( index < 12 )
                    {
                        // If current pixel IS at end of row
                        if ( ( index & 3 ) != 0 )
                        {
                            error[ index + 3 ].R += diff.R * ( 3f / 16f );
                            error[ index + 3 ].G += diff.G * ( 3f / 16f );
                            error[ index + 3 ].B += diff.B * ( 3f / 16f );
                        }

                        error[ index + 4 ].R += diff.R * ( 5f / 16f );
                        error[ index + 4 ].G += diff.G * ( 5f / 16f );
                        error[ index + 4 ].B += diff.B * ( 5f / 16f );

                        // If current pixel is not at end of row
                        if ( ( index & 3 ) != 3 )
                        {
                            error[ index + 5 ].R += diff.R * ( 1f / 16f );
                            error[ index + 5 ].G += diff.G * ( 1f / 16f );
                            error[ index + 5 ].B += diff.B * ( 1f / 16f );
                        }
                    }
                }

                colour[ index ].R *= sLuminance.R;
                colour[ index ].G *= sLuminance.G;
                colour[ index ].B *= sLuminance.B;
            }

            // Palette colours
            RGBColour colourA, colourB, colourC, colourD;
            colourA = new RGBColour();
            colourB = new RGBColour();
            colourC = new RGBColour();
            colourD = new RGBColour();

            // OPTIMISER
            var minmax = OptimiseRGB( colour, uSteps );
            colourA = minmax[ 0 ];
            colourB = minmax[ 1 ];

            // Create interstitial colours?
            colourC.R = colourA.R * sLuminanceInv.R;
            colourC.G = colourA.G * sLuminanceInv.G;
            colourC.B = colourA.B * sLuminanceInv.B;

            colourD.R = colourB.R * sLuminanceInv.R;
            colourD.G = colourB.G * sLuminanceInv.G;
            colourD.B = colourB.B * sLuminanceInv.B;


            // Yeah...dunno
            uint wColourA = Encode565( colourC );
            uint wColourB = Encode565( colourD );

            if ( uSteps == 4 && wColourA == wColourB )
            {
                var bits = new byte[ 8 ];
                var c2 = BitConverter.GetBytes( wColourA );
                var c1 = BitConverter.GetBytes( wColourB );  //////////////////////////////////////////////////// MIN MAX
                bits[ 0 ] = c2[ 0 ];
                bits[ 1 ] = c2[ 1 ];

                bits[ 2 ] = c1[ 0 ];
                bits[ 3 ] = c1[ 1 ];
                return bits;
            }

            colourC = Decode565( wColourA );
            colourD = Decode565( wColourB );

            colourA.R = colourC.R * sLuminance.R;
            colourA.G = colourC.G * sLuminance.G;
            colourA.B = colourC.B * sLuminance.B;

            colourB.R = colourD.R * sLuminance.R;
            colourB.G = colourD.G * sLuminance.G;
            colourB.B = colourD.B * sLuminance.B;


            // Create palette colours
            var step = new RGBColour[ 4 ];
            uint min = 0;
            uint max = 0;

            if ( uSteps == 3 == wColourA <= wColourB )
            {
                min = wColourA;
                max = wColourB;
                step[ 0 ] = colourA;
                step[ 1 ] = colourB;
            }
            else
            {
                min = wColourB;
                max = wColourA;
                step[ 0 ] = colourB;
                step[ 1 ] = colourA;
            }

            uint[] psteps;

            if ( uSteps == 3 )
            {
                psteps = sPsteps3;

                step[ 2 ].R = step[ 0 ].R + 1f / 2f * ( step[ 1 ].R - step[ 0 ].R );
                step[ 2 ].G = step[ 0 ].G + 1f / 2f * ( step[ 1 ].G - step[ 0 ].G );
                step[ 2 ].B = step[ 0 ].B + 1f / 2f * ( step[ 1 ].B - step[ 0 ].B );
            }
            else
            {
                psteps = sPsteps4;

                // "step" appears to be the palette as this is the interpolation
                step[ 2 ].R = step[ 0 ].R + 1f / 3f * ( step[ 1 ].R - step[ 0 ].R );
                step[ 2 ].G = step[ 0 ].G + 1f / 3f * ( step[ 1 ].G - step[ 0 ].G );
                step[ 2 ].B = step[ 0 ].B + 1f / 3f * ( step[ 1 ].B - step[ 0 ].B );

                step[ 3 ].R = step[ 0 ].R + 2f / 3f * ( step[ 1 ].R - step[ 0 ].R );
                step[ 3 ].G = step[ 0 ].G + 2f / 3f * ( step[ 1 ].G - step[ 0 ].G );
                step[ 3 ].B = step[ 0 ].B + 2f / 3f * ( step[ 1 ].B - step[ 0 ].B );
            }



            // Calculating colour direction apparently
            var dir = new RGBColour();
            dir.R = step[ 1 ].R - step[ 0 ].R;
            dir.G = step[ 1 ].G - step[ 0 ].G;
            dir.B = step[ 1 ].B - step[ 0 ].B;

            int fsteps = uSteps - 1;
            float fscale = wColourA != wColourB ? fsteps / ( dir.R * dir.R + dir.G * dir.G + dir.B * dir.B ) : 0.0f;
            dir.R *= fscale;
            dir.G *= fscale;
            dir.B *= fscale;


            // Encoding colours apparently
            Array.Clear( error, 0, error.Length );  // Clear error for next bit
            uint dw = 0;
            index = 0;
            for ( var i = 0; i < texel.Length; i += 4 )
            {
                index = i / 4;
                var current = ReadColourFromTexel( texel, i );

                if ( uSteps == 3 && current.A < alphaRef )
                {
                    dw = ( uint )( ( 3 << 30 ) | ( dw >> 2 ) );
                    continue;
                }

                current.R *= sLuminance.R;
                current.G *= sLuminance.G;
                current.B *= sLuminance.B;


                if ( dither )
                {
                    // Error again
                    current.R += error[ index ].R;
                    current.G += error[ index ].G;
                    current.B += error[ index ].B;
                }


                float fdot = ( current.R - step[ 0 ].R ) * dir.R + ( current.G - step[ 0 ].G ) * dir.G + ( current.B - step[ 0 ].B ) * dir.B;

                uint iStep = 0;
                if ( fdot <= 0f )
                    iStep = 0;
                else if ( fdot >= fsteps )
                    iStep = 1;
                else
                    iStep = psteps[ ( int )( fdot + .5f ) ];

                dw = ( iStep << 30 ) | ( dw >> 2 );   // THIS  IS THE MAGIC here. This is the "list" of indicies. Somehow...


                // Dither again
                if ( dither )
                {
                    // Calculate difference between current pixel colour and adapted pixel colour?
                    var diff = new RGBColour();
                    diff.R = current.A * ( byte )( current.R - step[ iStep ].R );
                    diff.G = current.A * ( byte )( current.G - step[ iStep ].G );
                    diff.B = current.A * ( byte )( current.B - step[ iStep ].B );

                    // If current pixel is not at the end of a row
                    if ( ( index & 3 ) != 3 )
                    {
                        error[ index + 1 ].R += diff.R * ( 7f / 16f );
                        error[ index + 1 ].G += diff.G * ( 7f / 16f );
                        error[ index + 1 ].B += diff.B * ( 7f / 16f );
                    }

                    // If current pixel is not in bottom row
                    if ( index < 12 )
                    {
                        // If current pixel IS at end of row
                        if ( ( index & 3 ) != 0 )
                        {
                            error[ index + 3 ].R += diff.R * ( 3f / 16f );
                            error[ index + 3 ].G += diff.G * ( 3f / 16f );
                            error[ index + 3 ].B += diff.B * ( 3f / 16f );
                        }

                        error[ index + 4 ].R += diff.R * ( 5f / 16f );
                        error[ index + 4 ].G += diff.G * ( 5f / 16f );
                        error[ index + 4 ].B += diff.B * ( 5f / 16f );

                        // If current pixel is not at end of row
                        if ( ( index & 3 ) != 3 )
                        {
                            error[ index + 5 ].R += diff.R * ( 1f / 16f );
                            error[ index + 5 ].G += diff.G * ( 1f / 16f );
                            error[ index + 5 ].B += diff.B * ( 1f / 16f );
                        }
                    }
                }
            }

            var retval = new byte[ 8 ];
            var colour1 = BitConverter.GetBytes( min );
            var colour2 = BitConverter.GetBytes( max );
            retval[ 0 ] = colour1[ 0 ];
            retval[ 1 ] = colour1[ 1 ];

            retval[ 2 ] = colour2[ 0 ];
            retval[ 3 ] = colour2[ 1 ];

            var indicies = BitConverter.GetBytes( dw );
            retval[ 4 ] = indicies[ 0 ];
            retval[ 5 ] = indicies[ 1 ];
            retval[ 6 ] = indicies[ 2 ];
            retval[ 7 ] = indicies[ 3 ];

            return retval;
        }
    }
}
