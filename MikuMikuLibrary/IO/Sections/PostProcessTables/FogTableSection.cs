// Code by Thatrandomlurker
using System.Text;
using MikuMikuLibrary.PostProcessTables.FogTable;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.PostProcessTables
{
    [Section("FOGC")]

    class FogTableSection : BinaryFileSection<FogTable>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public override Encoding Encoding { get; } = Encoding.GetEncoding( "utf-8" );

        public FogTableSection( SectionMode mode, FogTable data = null ) : base( mode, data )
        {
        }
    }
}
