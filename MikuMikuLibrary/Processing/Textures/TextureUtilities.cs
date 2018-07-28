using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Processing.Textures.DDS;
using MikuMikuLibrary.Textures;
using System;
using System.IO;
using System.Linq;

namespace MikuMikuLibrary.Processing.Textures
{
    public static class TextureUtilities
    {
        public static void RenameTexture( Texture texture, TextureSet textures, TextureDatabase textureDatabase = null )
        {
            bool empty = string.IsNullOrEmpty( texture.Name );

            if ( textureDatabase == null )
            {
                if ( empty )
                    texture.Name = string.Format( "Texture_{0}", textures.Textures.IndexOf( texture ) );
            }
            else
            {
                var entry = textureDatabase.Textures.FirstOrDefault( x => x.ID == texture.ID );
                if ( entry != null )
                    texture.Name = entry.Name;
                else if ( empty )
                    texture.Name = string.Format( "Texture_{0}", textures.Textures.IndexOf( texture ) );
            }
        }

        public static void RenameTextures( TextureSet textures, TextureDatabase textureDatabase = null )
        {
            foreach ( var texture in textures.Textures )
                RenameTexture( texture, textures, textureDatabase );
        }

        public static void SaveTextures( TextureSet textures, string outputDirectory )
        {
            Directory.CreateDirectory( outputDirectory );

            foreach ( var texture in textures.Textures )
            {
                using ( var destination = File.Create( Path.Combine( outputDirectory, texture.Name + ".dds" ) ) )
                    TextureDecoder.DecodeToDDS( texture, destination );
            }
        }

        public static DDSPixelFormatFourCC GetDDSPixelFormat( TextureFormat textureFormat )
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

        public static TextureFormat GetTextureFormat( DDSPixelFormat pixelFormat )
        {
            switch ( pixelFormat.FourCC )
            {
                case DDSPixelFormatFourCC.R8G8B8:
                    return TextureFormat.RGB;

                case DDSPixelFormatFourCC.A8R8G8B8:
                    return TextureFormat.RGBA;

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

                default:
                    throw new ArgumentException( nameof( pixelFormat ) );
            }
        }
    }
}
