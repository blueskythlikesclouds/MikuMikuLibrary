namespace MikuMikuLibrary.Parameters.Extensions;

public static class ParameterTreeWriterEx
{
    public static void Save(this ParameterTreeWriter paramTreeWriter, string filePath)
    {
        using (var stream = File.Create(filePath))
            paramTreeWriter.Flush(stream);
    }
}