using MikuMikuLibrary.IO;

namespace MikuMikuLibrary.Archives;

public static class ArchiveUtility
{
    public static T Open<T>(IArchive archive, string fileName, Func<Stream, T> factory = null)
        where T : IBinaryFile, new()
    {
        using (var stream = archive.Open(fileName, EntryStreamMode.MemoryStream))
            return factory != null ? factory(stream) : BinaryFile.Load<T>(stream);
    }

    public static T Open<TArchive, T>(Stream archiveStream, string fileName, Func<Stream, T> factory = null)
        where TArchive : IArchive, IBinaryFile, new()
        where T : IBinaryFile, new()
    {
        using (var archive = BinaryFile.Load<TArchive>(archiveStream, true))
            return Open(archive, fileName, factory);
    }

    public static T Open<TArchive, T>(string archiveFilePath, string fileName, Func<Stream, T> factory = null)
        where TArchive : IArchive, IBinaryFile, new()
        where T : IBinaryFile, new()
    {
        using (var stream = File.OpenRead(archiveFilePath))
            return Open<TArchive, T>(stream, fileName, factory);
    }

    public static void Add(IArchive archive, string fileName, IBinaryFile binaryFile,
        ConflictPolicy conflictPolicy = ConflictPolicy.RaiseError, Action<Stream> saver = null)
    {
        var memoryStream = new MemoryStream();

        if (saver != null)
            saver(memoryStream);

        else
            binaryFile.Save(memoryStream, true);

        archive.Add(fileName, memoryStream, true, conflictPolicy);
    }
}