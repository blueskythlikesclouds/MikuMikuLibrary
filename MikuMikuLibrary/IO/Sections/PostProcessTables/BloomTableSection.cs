// Code by Thatrandomlurker
using System.Text;
using MikuMikuLibrary.PostProcessTables.BloomTable;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.PostProcessTables
{
    [Section("BLMT")]

    class BloomTableSection : BinaryFileSection<BloomTable>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public override Encoding Encoding { get; } = Encoding.GetEncoding( "utf-8" );

        public BloomTableSection( SectionMode mode, BloomTable data = null ) : base( mode, data )
        {
        }
    }
}
