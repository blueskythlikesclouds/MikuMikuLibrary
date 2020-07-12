using System.Drawing;

namespace MikuMikuLibrary.Textures.Processing.Interfaces
{
    public interface ITextureDecoder
    {
        Bitmap DecodeToBitmap( SubTexture subTexture );
        Bitmap DecodeToBitmap( Texture texture );
        Bitmap[ , ] DecodeToBitmaps( Texture texture );

        void DecodeToFile( SubTexture subTexture, string filePath );
        void DecodeToFile( Texture texture, string filePath );
    }
}