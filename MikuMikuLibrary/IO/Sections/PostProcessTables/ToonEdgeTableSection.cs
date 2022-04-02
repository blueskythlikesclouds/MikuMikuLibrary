// Code by Thatrandomlurker
using System.Text;
using MikuMikuLibrary.PostProcessTables.ToonEdgeTable;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.PostProcessTables
{
    [Section("TETT")]

    class ToonEdgeTableSection : BinaryFileSection<ToonEdgeTable>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public override Encoding Encoding { get; } = Encoding.GetEncoding( "utf-8" );

        public ToonEdgeTableSection( SectionMode mode, ToonEdgeTable data = null ) : base( mode, data )
        {
        }
    }
}
