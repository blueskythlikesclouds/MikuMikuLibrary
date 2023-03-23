using MikuMikuLibrary.IO;
using MikuMikuLibrary.Text;

namespace MikuMikuModel.Modules.Text;

public class TextFileModule : FormatModule<TextFile>
{
    public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        { new FormatExtension("Text File", "txt", FormatExtensionFlags.Import | FormatExtensionFlags.Export) };

    protected override TextFile ImportCore(Stream source, string fileName)
    {
        return BinaryFile.Load<TextFile>(source, true);
    }

    protected override void ExportCore(TextFile model, Stream destination, string fileName)
    {
        model.Save(destination);
    }
}