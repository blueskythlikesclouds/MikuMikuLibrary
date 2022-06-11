using System.Security.Cryptography;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Archives.CriMw;

// This is made SPECIFICALLY for MM+ CPKs.
// Do not expect it to work for anything else.

public class CpkArchive : BinaryFile, IArchive
{
    private readonly Dictionary<string, Entry> mEntries = new(StringComparer.OrdinalIgnoreCase);

    public override BinaryFileFlags Flags => BinaryFileFlags.Load | BinaryFileFlags.UsesSourceStream;
    public override Endianness Endianness => Endianness.Little;

    public bool CanAdd => false;
    public bool CanRemove => false;
    public IEnumerable<string> FileNames => mEntries.Keys;
    public bool Contains(string fileName) => mEntries.ContainsKey(fileName);
    public void Add(string fileName, Stream source, bool leaveOpen, ConflictPolicy conflictPolicy) => throw new NotSupportedException();
    public void Add(string fileName, string sourceFilePath, ConflictPolicy conflictPolicy) => throw new NotSupportedException();
    public void Remove(string fileName) => throw new NotSupportedException();
    public void Clear() => throw new NotSupportedException();

    public EntryStream Open(string fileName, EntryStreamMode mode)
    {
        var entry = mEntries[fileName];
        Stream stream = entry.Open(mStream);

        if (mode == EntryStreamMode.MemoryStream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            stream.Close();
            stream = memoryStream;
            stream.Seek(0, SeekOrigin.Begin);
        }

        return new EntryStream(entry.Name, stream);
    }

    // Very fragile, will break if you give it a path with more than one separator.
    public void Extract(string dstDirectoryPath, string dirName = null)
    {
        if (!string.IsNullOrEmpty(dirName))
        {
            dirName = dirName.Replace('\\', '/');
            if (!dirName.EndsWith("/"))
                dirName += "/";
        }

        foreach (var entry in mEntries.Values)
        {
            string path = entry.Name;

            if (!string.IsNullOrEmpty(dirName))
            {
                if (!entry.Name.StartsWith(dirName, StringComparison.OrdinalIgnoreCase))
                    continue;

                path = path.Substring(dirName.Length);
            }

            string filePath = Path.Combine(dstDirectoryPath, path);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var dstStream = File.Create(filePath))
            using (var entryStream = entry.Open(mStream))
                entryStream.CopyTo(dstStream);
        }
    }

    public override void Read(EndianBinaryReader reader, ISection section = null)
    {
        var cpk = UtfTable.ReadFromChunk(reader, "CPK ");

        long tocOffset = cpk[0].Get<long>("TocOffset");
        long contentOffset = cpk[0].Get<long>("ContentOffset");
        int alignment = cpk[0].Get<int>("Align");

        reader.ReadAtOffset(tocOffset, () =>
        {
            var toc = UtfTable.ReadFromChunk(reader, "TOC ");

            foreach (var row in toc)
            {
                var entry = new Entry
                {
                    Name = Path.Combine(row.Get<string>("DirName"), row.Get<string>("FileName")).Replace('\\', '/'),
                    Position = row.Get<long>("FileOffset"),
                    Length = row.Get<long>("FileSize")
                };

                if (contentOffset < tocOffset)
                    entry.Position += contentOffset;
                else
                    entry.Position += tocOffset;

                entry.Position = AlignmentHelper.Align(entry.Position, alignment);

                mEntries[entry.Name] = entry;
            }
        });
    }

    public override void Write(EndianBinaryWriter writer, ISection section = null)
    {
        throw new NotSupportedException();
    }

    public IEnumerator<string> GetEnumerator() => mEntries.Keys.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private class Entry
    {
        public string Name;
        public long Position;
        public long Length;

        public Stream Open(Stream stream)
        {
            var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Key = new byte[]
            {
                0xCF, 0x53, 0xBF, 0x9C, 0x37, 0x67, 0xAF, 0xB0, 0x35, 0x54, 0x4E, 0xB9, 0x96, 0xAA, 0x24, 0x39,
                0x26, 0x5D, 0x40, 0x89, 0x7E, 0xD0, 0x1C, 0x3A, 0x6B, 0xA6, 0x5D, 0xD5, 0xFD, 0x6C, 0x19, 0xA3
            };
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.IV = new byte[]
            {
                0xC2, 0x55, 0xFD, 0x73, 0xD8, 0x30, 0xFA, 0xEF, 0xD5, 0x32, 0x08, 0x54, 0xA2, 0x26, 0x44, 0x14
            };

            return new CryptoStream(new StreamView(stream, Position, Length, true),
                aes.CreateDecryptor(), CryptoStreamMode.Read);
        }
    }
}