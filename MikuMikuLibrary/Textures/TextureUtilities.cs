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
                var info = textureDatabase.Textures.FirstOrDefault( x => x.Id == texture.Id );
                if ( info != null )
                    texture.Name = info.Name;
                else if ( empty )
                    texture.Name = string.Format( "Texture_{0}", textures.Textures.IndexOf( texture ) );
            }
        }

        public static void RenameTextures( TextureSet textures, TextureDatabase textureDatabase = null )
        {
            foreach ( var texture in textures.Textures )
                RenameTexture( texture, textures, textureDatabase );
        }

        public static string GetFileName( Texture texture )
        {
            if ( !TextureFormatUtilities.IsCompressed( texture.Format ) || texture.IsYCbCr )
                return texture.Name + ".png";
            return texture.Name + ".dds";
        }

        public static void SaveTextures( TextureSet textures, string outputDirectory )
        {
            Directory.CreateDirectory( outputDirectory );

            foreach ( var texture in textures.Textures )
                if ( !TextureFormatUtilities.IsCompressed( texture.Format ) || texture.IsYCbCr )
                    TextureDecoder.DecodeToPNG( texture, Path.Combine( outputDirectory, texture.Name + ".png" ) );
                else
                    TextureDecoder.DecodeToDDS( texture, Path.Combine( outputDirectory, texture.Name + ".dds" ) );
        }

        public static void ReAssignTextureIDs( ObjectSet model, List<int> newTextureIds )
        {
            var dictionary = new Dictionary<int, int>( model.TextureIds.Count );
            for ( int i = 0; i < model.TextureIds.Count; i++ )
            {
                dictionary.Add( model.TextureIds[ i ], newTextureIds[ i ] );
                model.TextureIds[ i ] = newTextureIds[ i ];
                model.TextureSet.Textures[ i ].Id = newTextureIds[ i ];
            }

            foreach ( var materialTexture in model.Objects.SelectMany( x => x.Materials )
                .SelectMany( x => x.MaterialTextures ) )
                if ( dictionary.TryGetValue( materialTexture.TextureId, out int id ) )
                    materialTexture.TextureId = id;
                else
                    materialTexture.TextureId = -1;
        }

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