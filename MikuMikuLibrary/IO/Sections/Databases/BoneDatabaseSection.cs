using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.Databases
{
    [Section( "BONE" )]
    public class BoneDatabaseSection : BinaryFileSection<BoneDatabase>
    {
        public override SectionFlags Flags => SectionFlags.HasRelocationTable;

        public BoneDatabaseSection( SectionMode mode, BoneDatabase data = null ) : base( mode, data )
        {
        }
    }
}