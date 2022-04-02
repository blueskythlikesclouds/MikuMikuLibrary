// Code by Thatrandomlurker
using System.Text;
using MikuMikuLibrary.PostProcessTables.ColorCorrectTable;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.PostProcessTables
{
    [Section("CCRT")]

    class ColorCorrectTableSection : BinaryFileSection<ColorCorrectTable>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public override Encoding Encoding { get; } = Encoding.GetEncoding( "utf-8" );

        public ColorCorrectTableSection( SectionMode mode, ColorCorrectTable data = null ) : base( mode, data )
        {
        }
    }
}
