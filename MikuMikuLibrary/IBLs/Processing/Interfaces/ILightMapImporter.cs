namespace MikuMikuLibrary.IBLs.Processing.Interfaces
{
    public interface ILightMapImporter
    {
        LightMap ImportFromFile( string filePath, int width, int height );  
    }
}