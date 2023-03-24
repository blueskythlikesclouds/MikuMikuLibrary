using MikuMikuLibrary.Archives;
using MikuMikuLibrary.Archives.Extensions;
using MikuMikuLibrary.IO;

namespace MikuMikuLibrary.Objects.Processing;

public class BatchProcessor
{
    public static void ProcessObjectSetsInDirectory(
        string directoryPath, bool processRecursively, Func<ObjectSet, bool> processor)
    {
        var searchOption = processRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        Parallel.ForEach(Directory.EnumerateFiles(directoryPath, "*.farc", searchOption), filePath =>
        {
            using var farcArchive = BinaryFile.Load<FarcArchive>(filePath);
            bool shouldSave = false;

            foreach (var entryName in farcArchive)
            {
                if (entryName.EndsWith("_obj.bin", StringComparison.OrdinalIgnoreCase) ||
                    entryName.EndsWith(".osd", StringComparison.OrdinalIgnoreCase))
                {
                    var objSet = farcArchive.Open<ObjectSet>(entryName);

                    if (processor(objSet))
                    {
                        farcArchive.Add(entryName, objSet, ConflictPolicy.Replace);
                        shouldSave = true;
                    }
                }
            }

            if (shouldSave)
                farcArchive.Save(filePath);
        });

        var filePaths = Directory.EnumerateFiles(directoryPath, "*_obj.bin", searchOption).Concat(
            Directory.EnumerateFiles(directoryPath, "*.osd", searchOption));

        Parallel.ForEach(filePaths, filePath =>
        {
            var objSet = BinaryFile.Load<ObjectSet>(filePath);

            if (processor(objSet))
                objSet.Save(filePath);
        });
    }
}