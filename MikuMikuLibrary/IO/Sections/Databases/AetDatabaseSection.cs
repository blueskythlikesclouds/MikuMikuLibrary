using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.Databases
{
    [Section( "AEDB" )]
    public class AetDatabaseSection : BinaryFileSection<AetDatabase>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public AetDatabaseSection( SectionMode mode, AetDatabase data = null ) : base( mode, data )
        {
        }
    }
}