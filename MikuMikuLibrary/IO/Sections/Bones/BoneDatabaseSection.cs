using MikuMikuLibrary.Bones;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.Bones;

[Section("BONE")]
public class BoneDataSection : BinaryFileSection<BoneData>
{
    public override SectionFlags Flags => SectionFlags.None;

    public BoneDataSection(SectionMode mode, BoneData data = null) : base(mode, data)
    {
    }
}