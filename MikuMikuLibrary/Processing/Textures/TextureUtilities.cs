using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Textures;
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
    }
}
