namespace MikuMikuLibrary.IBLs.Processing
{
    public class LightMapImporter
    {
        public static LightMap ImportFromFile( string filePath, int width = -1, int height = -1 ) =>
            Native.LightMapImporter.ImportFromFile( filePath, width, height );
    }
}