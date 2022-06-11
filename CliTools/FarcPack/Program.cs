using MikuMikuLibrary.Archives;
using MikuMikuLibrary.Archives.CriMw;
using MikuMikuLibrary.IO;

namespace FarcPack;

internal class Program
{
    private static void Main(string[] args)
    {
        string sourceFileName = null;
        string destinationFileName = null;

        bool compress = false;
        int alignment = 16;

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];

            if (EqualsAny("-c", "--compress"))
                compress = true;

            else if (EqualsAny("-a", "--alignment"))
                alignment = int.Parse(args[++i]);

            else if (sourceFileName == null)
                sourceFileName = arg;

            else if (destinationFileName == null)
                destinationFileName = arg;

            bool EqualsAny(params string[] strings)
            {
                return strings.Contains(arg, StringComparer.OrdinalIgnoreCase);
            }
        }

        if (sourceFileName == null)
        {
            Console.WriteLine(@"~~~~~~~~~
FARC Pack
~~~~~~~~~
This tool allows you to unpack and pack FARC files. You can also unpack CPK files from MM+.

~~~~~
Usage
~~~~~
FarcPack [options] [source] [destination]

~~~~~~
Options
~~~~~~~
-c or --compress: Compress archive (disabled by default)
-a or --alignment: Alignment value (16 by default)

~~~~~~~
Example
~~~~~~~
FarcPack -c -a 16 mikitm mikitm.farc

Destination is optional, which makes it possible to do a drag and drop onto the executable.");
            Console.ReadLine();
            return;
        }

        if (destinationFileName == null)
            destinationFileName = sourceFileName;

        if (sourceFileName.EndsWith(".farc", StringComparison.OrdinalIgnoreCase))
        {
            destinationFileName = Path.ChangeExtension(destinationFileName, null);

            using (var stream = File.OpenRead(sourceFileName))
            {
                var farcArchive = BinaryFile.Load<FarcArchive>(stream);

                Directory.CreateDirectory(destinationFileName);

                foreach (string fileName in farcArchive)
                {
                    using (var destination = File.Create(Path.Combine(destinationFileName, fileName)))
                    using (var source = farcArchive.Open(fileName, EntryStreamMode.OriginalStream))
                        source.CopyTo(destination);
                }
            }
        }

        else if (sourceFileName.EndsWith(".cpk", StringComparison.OrdinalIgnoreCase))
        {
            destinationFileName = Path.ChangeExtension(destinationFileName, null);

            using (var cpkArchive = BinaryFile.Load<CpkArchive>(sourceFileName))
                cpkArchive.Extract(destinationFileName);
        }

        else
        {
            destinationFileName = Path.ChangeExtension(destinationFileName, "farc");

            var farcArchive = new FarcArchive { IsCompressed = compress, Alignment = alignment };

            if (File.GetAttributes(sourceFileName).HasFlag(FileAttributes.Directory))
            {
                foreach (string filePath in Directory.EnumerateFiles(sourceFileName))
                    farcArchive.Add(Path.GetFileName(filePath), filePath);
            }

            else
            {
                farcArchive.Add(Path.GetFileName(sourceFileName), sourceFileName);
            }

            farcArchive.Save(destinationFileName);
        }
    }
}