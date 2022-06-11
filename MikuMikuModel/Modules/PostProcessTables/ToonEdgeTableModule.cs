using MikuMikuLibrary.IO;
using MikuMikuLibrary.PostProcessTables;

namespace MikuMikuModel.Modules.PostProcessTables;

public class ToonEdgeTableModule : FormatModule<ToonEdgeTable>
{
    public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
    {
        new FormatExtension("Toon Edge Table", "tet", FormatExtensionFlags.Import | FormatExtensionFlags.Export),
    };

    public override bool Match(byte[] buffer)
    {
        return buffer[0] == 'T' && buffer[1] == 'E' && buffer[2] == 'T' && buffer[3] == 'T';
    }

    public override ToonEdgeTable Import(string filePath)
    {
        return BinaryFile.Load<ToonEdgeTable>(filePath);
    }

    protected override ToonEdgeTable ImportCore(Stream source, string fileName)
    {
        return BinaryFile.Load<ToonEdgeTable>(source, true);
    }

    protected override void ExportCore(ToonEdgeTable model, Stream destination, string fileName)
    {
        model.Save(destination, true);
    }

}