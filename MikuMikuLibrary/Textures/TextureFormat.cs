using System;

namespace MikuMikuLibrary.Textures
{
    public enum TextureFormat
    {
        Unknown = -1,
        A8 = 0,
        RGB8 = 1,
        RGBA8 = 2,
        RGB5 = 3,
        RGB5A1 = 4,
        RGBA4 = 5,
        DXT1 = 6,
        DXT1a = 7,
        DXT3 = 8,
        DXT5 = 9,
        ATI1 = 10,
        ATI2 = 11,
        L8 = 12,
        L8A8 = 13
    }

    public static class TextureFormatUtilities
    {
        public static bool IsBlockCompressed( TextureFormat format ) => 
            format >= TextureFormat.DXT1 && format <= TextureFormat.ATI2;

        public static bool HasAlpha( TextureFormat format )
        {
            return format == TextureFormat.A8 || format == TextureFormat.RGBA8 || format == TextureFormat.RGB5A1 || format == TextureFormat.RGBA4 ||
                   format == TextureFormat.DXT1a || format == TextureFormat.DXT3 || format == TextureFormat.DXT5;
        }

        public static int GetBlockSize( TextureFormat format )
        {
            switch ( format )
            {
                case TextureFormat.DXT1:
                case TextureFormat.DXT1a:
                case TextureFormat.ATI1:
                    return 8;

                case TextureFormat.DXT3:
                case TextureFormat.DXT5:
                case TextureFormat.ATI2:
                    return 16;
            }

            throw new ArgumentException( nameof( format ) );
        }

        public static int CalculateDataSize( int width, int height, TextureFormat format )
        {
            switch ( format )
            {
                case TextureFormat.A8:
                case TextureFormat.L8:
                    return width * height;

                case TextureFormat.RGB8:
                    return width * height * 3;

                case TextureFormat.RGBA8:
                    return width * height * 4;

                case TextureFormat.RGB5:
                case TextureFormat.RGB5A1:
                case TextureFormat.RGBA4:
                case TextureFormat.L8A8:
                    return width * height * 2;

                default:
                    return Math.Max( 1, ( width + 3 ) / 4 ) * Math.Max( 1, ( height + 3 ) / 4 ) * GetBlockSize( format );
            }
        }
    }
}