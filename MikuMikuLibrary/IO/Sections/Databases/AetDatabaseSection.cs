using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.Databases
{
    [Section( "AEDB" )]
    public class AetDatabaseSection : BinaryFileSection<AetDatabase>
    {
        public AetDatabaseSection( SectionMode mode, AetDatabase data = null ) : base( mode, data )
        {
        }

        public override SectionFlags Flags => SectionFlags.HasRelocationTable;
    }
}
