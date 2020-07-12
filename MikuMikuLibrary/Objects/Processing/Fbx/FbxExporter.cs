namespace MikuMikuLibrary.Objects.Processing.Fbx
{
    public static class FbxExporter
    {
        public static void ExportToFile( ObjectSet objectSet, string destinationFilePath ) =>
            Native.FbxExporter.ExportToFile( objectSet, destinationFilePath );
    }
}