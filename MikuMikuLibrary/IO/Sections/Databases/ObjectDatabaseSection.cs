using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.Databases
{
    [Section( "MOSI" )]
    public class ObjectDatabaseSection : BinaryFileSection<ObjectDatabase>
    {
        public ObjectDatabaseSection( SectionMode mode, ObjectDatabase data = null ) : base( mode, data )
        {
        }

        public override SectionFlags Flags => SectionFlags.HasRelocationTable;
    }
}
