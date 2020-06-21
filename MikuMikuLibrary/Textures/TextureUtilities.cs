using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Textures.DDS;

namespace MikuMikuLibrary.Textures
{
    public class TextureUtilities
    {
        public static DDSPixelFormatFourCC GetDDSPixelFormat( TextureFormat textureFormat )
        {
            switch ( textureFormat )
            {
                case TextureFormat.A8:
                    return DDSPixelFormatFourCC.A8;

                case TextureFormat.RGB8:
                    return DDSPixelFormatFourCC.R8G8B8;

                case TextureFormat.RGBA8:
                    return DDSPixelFormatFourCC.A8R8G8B8;

                case TextureFormat.RGB5:
                    return DDSPixelFormatFourCC.X1R5G5B5;

                case TextureFormat.RGB5A1:
                    return DDSPixelFormatFourCC.A1R5G5B5;

                case TextureFormat.RGBA4:
                    return DDSPixelFormatFourCC.A4R4G4B4;

                case TextureFormat.DXT1:
                case TextureFormat.DXT1a:
                    return DDSPixelFormatFourCC.DXT1;

                case TextureFormat.DXT3:
                    return DDSPixelFormatFourCC.DXT3;

                case TextureFormat.DXT5:
                    return DDSPixelFormatFourCC.DXT5;

                case TextureFormat.ATI1:
                    return DDSPixelFormatFourCC.ATI1;

                case TextureFormat.ATI2:
                    return DDSPixelFormatFourCC.ATI2N_3Dc;

                case TextureFormat.L8:
                    return DDSPixelFormatFourCC.L8;

                case TextureFormat.L8A8:
                    return DDSPixelFormatFourCC.A8L8;

                default:
                    throw new ArgumentException( nameof( textureFormat ) );
            }
        }

        public static TextureFormat GetTextureFormat( DDSPixelFormat pixelFormat )
        {
            switch ( pixelFormat.FourCC )
            {
                case DDSPixelFormatFourCC.A8:
                    return TextureFormat.A8;

                case DDSPixelFormatFourCC.R8G8B8:
                    return TextureFormat.RGB8;

                case DDSPixelFormatFourCC.A8R8G8B8:
                    return TextureFormat.RGBA8;

                case DDSPixelFormatFourCC.X1R5G5B5:
                    return TextureFormat.RGB5;

                case DDSPixelFormatFourCC.A1R5G5B5:
                    return TextureFormat.RGB5A1;

                case DDSPixelFormatFourCC.A4R4G4B4:
                    return TextureFormat.RGBA4;

                case DDSPixelFormatFourCC.DXT1:
                    return TextureFormat.DXT1;

                case DDSPixelFormatFourCC.DXT3:
                    return TextureFormat.DXT3;

                case DDSPixelFormatFourCC.DXT5:
                    return TextureFormat.DXT5;

                case DDSPixelFormatFourCC.ATI1:
                    return TextureFormat.ATI1;

                case DDSPixelFormatFourCC.ATI2N_3Dc:
                    return TextureFormat.ATI2;

                case DDSPixelFormatFourCC.L8:
                    return TextureFormat.L8;

                case DDSPixelFormatFourCC.A8L8:
                    return TextureFormat.L8A8;

                default:
                    throw new ArgumentException( nameof( pixelFormat ) );
            }
        }
    }
}