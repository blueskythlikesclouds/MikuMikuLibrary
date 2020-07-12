using System.Drawing;

namespace MikuMikuLibrary.Textures.Processing
{
    public static class TextureEncoder
    {
        public static SubTexture EncodeFromBitmap( Bitmap bitmap, TextureFormat formatHint ) =>
            Native.TextureEncoder.EncodeFromBitmap( bitmap, formatHint );

        public static Texture EncodeFromBitmap( Bitmap bitmap, TextureFormat formatHint, bool generateMipMaps ) =>
            Native.TextureEncoder.EncodeFromBitmap( bitmap, formatHint, generateMipMaps );
        
        public static SubTexture EncodeFromFile( string filePath, TextureFormat formatHint ) =>
            Native.TextureEncoder.EncodeFromFile( filePath, formatHint );

        public static Texture EncodeFromFile( string filePath, TextureFormat formatHint, bool generateMipMaps ) =>
            Native.TextureEncoder.EncodeFromFile( filePath, formatHint, generateMipMaps );
    }
}