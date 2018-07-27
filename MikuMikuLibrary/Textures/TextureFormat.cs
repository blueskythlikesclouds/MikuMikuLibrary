using System;

namespace MikuMikuLibrary.Textures
{
    public enum TextureFormat
    {
        RGB = 1,
        RGBA = 2,
        DXT1 = 6,
        DXT3 = 7,
        DXT5 = 9,
        ATI1 = 10,
        ATI2 = 11,
    }

    public static class TextureFormatUtilities
    {
        public static int GetBlockSize( TextureFormat format )
        {
            switch ( format )
            {
                case TextureFormat.DXT1:
                case TextureFormat.ATI1:
                    return 8;

                case TextureFormat.DXT3:
                case TextureFormat.DXT5:
                case TextureFormat.ATI2:
                    return 16;
            }

            return 0;
        }

        public static int CalculateDataSize( int width, int height, TextureFormat format )
        {
            switch ( format )
            {
                case TextureFormat.RGB:
                    return width * height * 3;

                case TextureFormat.RGBA:
                    return width * height * 4;

                default:
                    return Math.Max( 1, ( width + 3 ) / 4 ) * Math.Max( 1, ( height + 3 ) / 4 ) * GetBlockSize( format );
            }
        }
    }
}
