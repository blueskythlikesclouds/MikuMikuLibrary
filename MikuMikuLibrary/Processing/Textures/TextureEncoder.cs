using MikuMikuLibrary.Processing.Textures.DDS;
using MikuMikuLibrary.Textures;
using System;
using System.IO;

namespace MikuMikuLibrary.Processing.Textures
{
    public static class TextureEncoder
    {
        public static Texture Encode( Stream source )
        {
            var ddsHeader = new DDSHeader( source );

            int mipMapCount = 1;
            if ( ddsHeader.Flags.HasFlag( DDSHeaderFlags.MipMapCount ) &&
                 ddsHeader.Caps.HasFlag( DDSHeaderCaps.MipMap ) )
            {
                mipMapCount = ddsHeader.MipMapCount;
            }

            var format = GetTextureFormat( ddsHeader.PixelFormat );

            var texture = new Texture();
            for ( int i = 0; i < mipMapCount; i++ )
            {
                var mipMap = new TextureMipMap();
                mipMap.ID = i;
                mipMap.Format = format;
                mipMap.Width = ddsHeader.Width >> i;
                mipMap.Height = ddsHeader.Height >> i;
                mipMap.Data = new byte[ ddsHeader.PitchOrLinearSize >> i >> i ];
                source.Read( mipMap.Data, 0, mipMap.Data.Length );
                texture.MipMaps.Add( mipMap );
            }

            return texture;
        }

        public static Texture Encode( string sourceFileName )
        {
            using ( var source = File.OpenRead( sourceFileName ) )
                return Encode( source );
        }

        private static TextureFormat GetTextureFormat( DDSPixelFormat pixelFormat )
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
