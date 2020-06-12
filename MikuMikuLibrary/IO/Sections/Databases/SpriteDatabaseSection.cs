using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO.Sections.IO;
using MikuMikuLibrary.Sprites;

namespace MikuMikuLibrary.IO.Sections.Databases
{
    [Section( "SPDB" )]
    public class SpriteDatabaseSection : BinaryFileSection<SpriteDatabase>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public SpriteDatabaseSection( SectionMode mode, SpriteDatabase data = null ) : base( mode, data )
        {
        }
    }
}