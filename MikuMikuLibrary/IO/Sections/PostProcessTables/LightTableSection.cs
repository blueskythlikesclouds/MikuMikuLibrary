// Code by Thatrandomlurker

using MikuMikuLibrary.IO.Sections.IO;
using MikuMikuLibrary.PostProcessTables;

namespace MikuMikuLibrary.IO.Sections.PostProcessTables;

[Section("LITC")]

class LightTableSection : BinaryFileSection<LightTable>
{
    public override SectionFlags Flags => SectionFlags.None;

    public override Encoding Encoding { get; } = Encoding.GetEncoding("utf-8");

    public LightTableSection(SectionMode mode, LightTable data = null) : base(mode, data)
    {
    }
}