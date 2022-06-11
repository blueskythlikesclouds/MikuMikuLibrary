using MikuMikuLibrary.Bones;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.Modules.Bones;

public class BoneDataModule : FormatModule<BoneData>
{
    public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
    {
        new FormatExtension("Bone Data (Classic)", "bin", FormatExtensionFlags.Import | FormatExtensionFlags.Export),
        new FormatExtension("Bone Data (Modern)", "bon", FormatExtensionFlags.Import | FormatExtensionFlags.Export)
    };

    public override bool Match(string fileName)
    {
        return base.Match(fileName) && Path.GetFileNameWithoutExtension(fileName)
            .Equals("bone_data", StringComparison.OrdinalIgnoreCase);
    }

    protected override BoneData ImportCore(Stream source, string fileName)
    {
        return BinaryFile.Load<BoneData>(source, true);
    }

    protected override void ExportCore(BoneData model, Stream destination, string fileName)
    {
        model.Save(destination, true);
    }
}