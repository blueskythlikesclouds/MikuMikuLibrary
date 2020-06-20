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
                case TextureFormat.RGB:
                    return DDSPixelFormatFourCC.R8G8B8;

                case TextureFormat.RGBA:
                    return DDSPixelFormatFourCC.A8R8G8B8;

                case TextureFormat.RGBA4:
                    return DDSPixelFormatFourCC.A4R4G4B4;

                case TextureFormat.DXT1:
                    return DDSPixelFormatFourCC.DXT1;

                case TextureFormat.DXT3:
                    return DDSPixelFormatFourCC.DXT3;

                case TextureFormat.DXT4:
                    return DDSPixelFormatFourCC.DXT4;

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

        public static TextureFormat GetTextureFormat( DDSPixelFormat pixelFormat )
        {
            switch ( pixelFormat.FourCC )
            {
                case DDSPixelFormatFourCC.R8G8B8:
                    return TextureFormat.RGB;

                case DDSPixelFormatFourCC.A8R8G8B8:
                    return TextureFormat.RGBA;

                case DDSPixelFormatFourCC.A4R4G4B4:
                    return TextureFormat.RGBA4;

                case DDSPixelFormatFourCC.DXT1:
                    return TextureFormat.DXT1;

                case DDSPixelFormatFourCC.DXT3:
                    return TextureFormat.DXT3;

                case DDSPixelFormatFourCC.DXT4:
                    return TextureFormat.DXT4;

                case DDSPixelFormatFourCC.DXT5:
                    return TextureFormat.DXT5;

                case DDSPixelFormatFourCC.ATI1:
                    return TextureFormat.ATI1;

                case DDSPixelFormatFourCC.ATI2N_3Dc:
                    return TextureFormat.ATI2;

                default:
                    throw new ArgumentException( nameof( pixelFormat ) );
            }
        }
    }
}