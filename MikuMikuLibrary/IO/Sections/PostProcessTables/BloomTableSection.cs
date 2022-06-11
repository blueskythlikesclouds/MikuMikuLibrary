// Code by Thatrandomlurker

using MikuMikuLibrary.IO.Sections.IO;
using MikuMikuLibrary.PostProcessTables;

namespace MikuMikuLibrary.IO.Sections.PostProcessTables;

[Section("BLMT")]

class BloomTableSection : BinaryFileSection<BloomTable>
{
    public override SectionFlags Flags => SectionFlags.None;

    public override Encoding Encoding { get; } = Encoding.GetEncoding("utf-8");

    public BloomTableSection(SectionMode mode, BloomTable data = null) : base(mode, data)
    {
    }
}