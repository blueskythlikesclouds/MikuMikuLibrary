//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System;

namespace MikuMikuLibrary.Textures.DDS
{
    public static class DDSFormatDetails
    {
        public static bool IsBlockCompressed( DDSPixelFormatFourCC format )
        {
            switch ( format )
            {
                case DDSPixelFormatFourCC.DXT1:
                case DDSPixelFormatFourCC.DXT2:
                case DDSPixelFormatFourCC.DXT3:
                case DDSPixelFormatFourCC.DXT4:
                case DDSPixelFormatFourCC.DXT5:
                case DDSPixelFormatFourCC.ATI1:
                case DDSPixelFormatFourCC.ATI2N_3Dc:
                    return true;
                default:
                    return false;
            }
        }

        public static int GetBlockSize( DDSPixelFormatFourCC format )
        {
            switch ( format )
            {
                case DDSPixelFormatFourCC.DXT1:
                case DDSPixelFormatFourCC.ATI1:
                    return 8;

                case DDSPixelFormatFourCC.DXT2:
                case DDSPixelFormatFourCC.DXT3:
                case DDSPixelFormatFourCC.DXT4:
                case DDSPixelFormatFourCC.DXT5:
                case DDSPixelFormatFourCC.ATI2N_3Dc:
                    return 16;
            }

            return 0;
        }

        public static int GetBitsPerPixel( DDSPixelFormatFourCC format )
        {
            switch ( format )
            {
                case DDSPixelFormatFourCC.DXT1:
                case DDSPixelFormatFourCC.ATI1:
                    return 4;

                case DDSPixelFormatFourCC.DXT2:
                case DDSPixelFormatFourCC.DXT3:
                case DDSPixelFormatFourCC.DXT4:
                case DDSPixelFormatFourCC.DXT5:
                case DDSPixelFormatFourCC.ATI2N_3Dc:
                    return 8;

                case DDSPixelFormatFourCC.R8G8B8:
                    return 24;

                case DDSPixelFormatFourCC.A8R8G8B8:
                case DDSPixelFormatFourCC.X8R8G8B8:
                    return 32;

                case DDSPixelFormatFourCC.R5G6B5:
                case DDSPixelFormatFourCC.X1R5G5B5:
                case DDSPixelFormatFourCC.A1R5G5B5:
                case DDSPixelFormatFourCC.A4R4G4B4:
                    return 16;

                case DDSPixelFormatFourCC.R3G3B2:
                case DDSPixelFormatFourCC.A8:
                    return 8;

                case DDSPixelFormatFourCC.A8R3G3B2:
                case DDSPixelFormatFourCC.X4R4G4B4:
                    return 16;

                case DDSPixelFormatFourCC.A2B10G10R10:
                case DDSPixelFormatFourCC.A8B8G8R8:
                case DDSPixelFormatFourCC.X8B8G8R8:
                case DDSPixelFormatFourCC.G16R16:
                case DDSPixelFormatFourCC.A2R10G10B10:
                    return 32;

                case DDSPixelFormatFourCC.A16B16G16R16:
                    return 64;

                case DDSPixelFormatFourCC.A8P8:
                    return 16;

                case DDSPixelFormatFourCC.P8:
                    return 8;

                case DDSPixelFormatFourCC.L8:
                    return 8;

                case DDSPixelFormatFourCC.A8L8:
                    return 16;
                case DDSPixelFormatFourCC.A4L4:
                    return 8;
                case DDSPixelFormatFourCC.V8U8:
                case DDSPixelFormatFourCC.L6V5U5:
                    return 16;
                case DDSPixelFormatFourCC.X8L8V8U8:
                case DDSPixelFormatFourCC.Q8W8V8U8:
                case DDSPixelFormatFourCC.V16U16:
                case DDSPixelFormatFourCC.A2W10V10U10:
                    return 32;
                case DDSPixelFormatFourCC.UYVY:
                    return 16;
                case DDSPixelFormatFourCC.R8G8_B8G8:
                    return 32;
                case DDSPixelFormatFourCC.YUY2:
                    return 8;
                case DDSPixelFormatFourCC.G8R8_G8B8:
                    return 32;
                case DDSPixelFormatFourCC.D16_LOCKABLE:
                    return 16;
                case DDSPixelFormatFourCC.D32:
                    return 32;
                case DDSPixelFormatFourCC.D15S1:
                    return 16;
                case DDSPixelFormatFourCC.D24S8:
                    return 32;
                case DDSPixelFormatFourCC.D24X8:
                    return 32;
                case DDSPixelFormatFourCC.D24X4S4:
                    return 32;
                case DDSPixelFormatFourCC.D16:
                    return 16;
                case DDSPixelFormatFourCC.D32F_LOCKABLE:
                    return 32;
                case DDSPixelFormatFourCC.D24FS8:
                    return 32;
                case DDSPixelFormatFourCC.L16:
                    return 16;
                case DDSPixelFormatFourCC.Q16Q16V16U16:
                    return 64;
                case DDSPixelFormatFourCC.R16F:
                    return 16;
                case DDSPixelFormatFourCC.G16R16F:
                    return 32;
                case DDSPixelFormatFourCC.A16B16G16R16F:
                    return 64;
                case DDSPixelFormatFourCC.R32F:
                    return 32;
                case DDSPixelFormatFourCC.G32R32F:
                    return 64;
                case DDSPixelFormatFourCC.A32B32G32R32F:
                    return 128;
                case DDSPixelFormatFourCC.CxV8U8:
                    return 16;
            }

            return 0;
        }

        public static int CalculatePitchOrLinearSize( int width, int height, DDSPixelFormatFourCC format, out DDSHeaderFlags additionalFlags )
        {
            if ( IsBlockCompressed( format ) )
            {
                additionalFlags = DDSHeaderFlags.LinearSize;
                return CalculateLinearSize( width, height, format );
            }
            else
            {
                additionalFlags = DDSHeaderFlags.Pitch;
                return CalculatePitch( width, format );
            }
        }

        public static int CalculatePitch( int width, DDSPixelFormatFourCC format )
        {
            if ( IsBlockCompressed( format ) )
            {
                int blockSize = GetBlockSize( format );
                return Math.Max( 1, ( width + 3 ) / 4 ) * blockSize;
            }

            if ( format == DDSPixelFormatFourCC.R8G8_B8G8 || format == DDSPixelFormatFourCC.G8R8_G8B8 ||
                 format == DDSPixelFormatFourCC.UYVY || format == DDSPixelFormatFourCC.YUY2 )
            {
                return ( ( width + 1 ) >> 1 ) * 4;
            }

            int bitsPerPixel = GetBitsPerPixel( format );
            return ( width * bitsPerPixel + 7 ) / 8;
        }

        public static int CalculateLinearSize( int width, int height, DDSPixelFormatFourCC format )
        {
            return width * height * GetBitsPerPixel( format ) / 8;
        }
    }
}