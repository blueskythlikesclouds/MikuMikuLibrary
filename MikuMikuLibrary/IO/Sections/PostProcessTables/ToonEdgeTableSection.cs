// Code by Thatrandomlurker

using MikuMikuLibrary.IO.Sections.IO;
using MikuMikuLibrary.PostProcessTables;

namespace MikuMikuLibrary.IO.Sections.PostProcessTables;

[Section("TETT")]

class ToonEdgeTableSection : BinaryFileSection<ToonEdgeTable>
{
    public override SectionFlags Flags => SectionFlags.None;

    public override Encoding Encoding { get; } = Encoding.GetEncoding("utf-8");

    public ToonEdgeTableSection(SectionMode mode, ToonEdgeTable data = null) : base(mode, data)
    {
    }
}