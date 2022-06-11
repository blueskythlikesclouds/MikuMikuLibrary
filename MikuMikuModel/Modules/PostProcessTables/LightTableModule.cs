// Code by Thatrandomlurker

using MikuMikuLibrary.IO;
using MikuMikuLibrary.PostProcessTables;

namespace MikuMikuModel.Modules.PostProcessTables;

public class LightTableModule : FormatModule<LightTable>
{
    public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
    {
        new FormatExtension("Light Table", "lit", FormatExtensionFlags.Import | FormatExtensionFlags.Export),
    };

    public override bool Match(byte[] buffer)
    {
        return buffer[0] == 'L' && buffer[1] == 'I' && buffer[2] == 'T' && buffer[3] == 'C';
    }

    public override LightTable Import(string filePath)
    {
        return BinaryFile.Load<LightTable>(filePath);
    }

    protected override LightTable ImportCore(Stream source, string fileName)
    {
        return BinaryFile.Load<LightTable>(source, true);
    }

    protected override void ExportCore(LightTable model, Stream destination, string fileName)
    {
        model.Save(destination, true);
    }

}