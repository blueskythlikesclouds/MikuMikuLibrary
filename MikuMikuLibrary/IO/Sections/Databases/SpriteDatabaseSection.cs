using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.Databases
{
    [Section( "SPDB" )]
    public class SpriteDatabaseSection : BinaryFileSection<SpriteDatabase>
    {
        public override SectionFlags Flags => SectionFlags.HasRelocationTable;

        public SpriteDatabaseSection( SectionMode mode, SpriteDatabase data = null ) : base( mode, data )
        {
        }
    }
}