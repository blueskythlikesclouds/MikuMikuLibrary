namespace MikuMikuLibrary.Objects.Processing.Fbx.Interfaces
{
    public interface IFbxExporter
    {
        void ExportToFile( ObjectSet objectSet, string destinationFilePath );
    }
}