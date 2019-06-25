using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.Databases
{
    [Section( "MTXI" )]
    public class TextureDatabaseSection : BinaryFileSection<TextureDatabase>
    {
        public TextureDatabaseSection( SectionMode mode, TextureDatabase data = null ) : base( mode, data )
        {
        }

        public override SectionFlags Flags => SectionFlags.HasRelocationTable;
    }
}
