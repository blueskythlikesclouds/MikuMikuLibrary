using MikuMikuLibrary.IO.Sections.IO;
using MikuMikuLibrary.Stages;

namespace MikuMikuLibrary.IO.Sections.Stages;

[Section("STGC")]
public class StageDataSection : BinaryFileSection<StageData>
{
    public override SectionFlags Flags => SectionFlags.None;

    public StageDataSection(SectionMode mode, StageData data = null) : base(mode, data)
    {
    }
}