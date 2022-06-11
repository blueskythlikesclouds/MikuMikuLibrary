// Code by Thatrandomlurker

using MikuMikuLibrary.IO;
using MikuMikuLibrary.PostProcessTables;

namespace MikuMikuModel.Modules.PostProcessTables;

public class ColorCorrectTableModule : FormatModule<ColorCorrectTable>
{
    public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
    {
        new FormatExtension("Color Correct Table", "cct", FormatExtensionFlags.Import | FormatExtensionFlags.Export),
    };

    public override bool Match(byte[] buffer)
    {
        return buffer[0] == 'C' && buffer[1] == 'C' && buffer[2] == 'R' && buffer[3] == 'T';
    }

    public override ColorCorrectTable Import(string filePath)
    {
        return BinaryFile.Load<ColorCorrectTable>(filePath);
    }

    protected override ColorCorrectTable ImportCore(Stream source, string fileName)
    {
        return BinaryFile.Load<ColorCorrectTable>(source, true);
    }

    protected override void ExportCore(ColorCorrectTable model, Stream destination, string fileName)
    {
        model.Save(destination, true);
    }

}