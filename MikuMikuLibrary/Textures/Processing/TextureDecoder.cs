using System.Drawing;

namespace MikuMikuLibrary.Textures.Processing
{
    public static class TextureDecoder
    {
        public static Bitmap DecodeToBitmap( SubTexture subTexture ) =>
            Native.TextureDecoder.DecodeToBitmap( subTexture );

        public static Bitmap DecodeToBitmap( Texture texture ) =>
            Native.TextureDecoder.DecodeToBitmap( texture );        
        
        public static Bitmap[ , ] DecodeToBitmaps( Texture texture ) =>
            Native.TextureDecoder.DecodeToBitmaps( texture );

        public static void DecodeToFile( SubTexture subTexture, string filePath ) =>
            Native.TextureDecoder.DecodeToFile( subTexture, filePath );

        public static void DecodeToFile( Texture texture, string filePath ) =>
            Native.TextureDecoder.DecodeToFile( texture, filePath );
    }
}