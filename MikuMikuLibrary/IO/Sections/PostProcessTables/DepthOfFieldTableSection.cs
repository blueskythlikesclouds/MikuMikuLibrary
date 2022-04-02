// Code by Thatrandomlurker
using System.Text;
using MikuMikuLibrary.PostProcessTables.DepthOfFieldTable;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.PostProcessTables
{
    [Section("DOFT")]

    class DOFTableSection : BinaryFileSection<DOFTable>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public override Encoding Encoding { get; } = Encoding.GetEncoding( "utf-8" );

        public DOFTableSection( SectionMode mode, DOFTable data = null ) : base( mode, data )
        {
        }
    }
}
