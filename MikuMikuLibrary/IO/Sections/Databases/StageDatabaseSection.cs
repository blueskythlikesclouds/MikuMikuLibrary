using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.Databases
{
    [Section( "STGC" )]
    public class StageDatabaseSection : BinaryFileSection<StageDatabase>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public StageDatabaseSection(SectionMode mode, StageDatabase data = null) : base( mode, data )
        {
        }
    }
}
