using System.Drawing;

namespace MikuMikuLibrary.Textures.Processing.Interfaces
{
    public interface ITextureEncoder
    {
        SubTexture EncodeFromBitmap( Bitmap bitmap, TextureFormat formatHint );
        Texture EncodeFromBitmap( Bitmap bitmap, TextureFormat formatHint, bool generateMipMaps );

        SubTexture EncodeFromFile( string filePath, TextureFormat formatHint );
        Texture EncodeFromFile( string filePath, TextureFormat formatHint, bool generateMipMaps );
    }
}