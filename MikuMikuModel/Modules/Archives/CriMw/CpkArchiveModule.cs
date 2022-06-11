using MikuMikuLibrary.Archives.CriMw;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.Modules.Archives.CriMw;

public class CpkArchiveModule : FormatModule<CpkArchive>
{
    public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
    {
        new FormatExtension("CPK Archive", "cpk", FormatExtensionFlags.Import)
    };

    public override bool Match(byte[] buffer)
    {
        string signature = Encoding.UTF8.GetString(buffer, 0, 4);
        return signature == "CPK ";
    }

    public override CpkArchive Import(string filePath)
    {
        return BinaryFile.Load<CpkArchive>(filePath);
    }

    protected override CpkArchive ImportCore(Stream source, string fileName)
    {
        return BinaryFile.Load<CpkArchive>(source, true);
    }

    protected override void ExportCore(CpkArchive model, Stream destination, string fileName)
    {
        throw new NotSupportedException();
    }
}