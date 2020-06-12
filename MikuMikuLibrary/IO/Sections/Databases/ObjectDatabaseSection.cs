using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO.Sections.IO;
using MikuMikuLibrary.Objects;

namespace MikuMikuLibrary.IO.Sections.Databases
{
    [Section( "MOSI" )]
    public class ObjectDatabaseSection : BinaryFileSection<ObjectDatabase>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public ObjectDatabaseSection( SectionMode mode, ObjectDatabase data = null ) : base( mode, data )
        {
        }
    }
}