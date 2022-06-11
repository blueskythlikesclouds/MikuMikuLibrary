// Code by Thatrandomlurker

using MikuMikuLibrary.IO.Sections.IO;
using MikuMikuLibrary.PostProcessTables;

namespace MikuMikuLibrary.IO.Sections.PostProcessTables;

[Section("CCRT")]

class ColorCorrectTableSection : BinaryFileSection<ColorCorrectTable>
{
    public override SectionFlags Flags => SectionFlags.None;

    public override Encoding Encoding { get; } = Encoding.GetEncoding( "utf-8" );

    public ColorCorrectTableSection( SectionMode mode, ColorCorrectTable data = null ) : base( mode, data )
    {
    }
}