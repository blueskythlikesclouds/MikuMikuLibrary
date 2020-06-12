using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.Databases
{
    [Section( "MTXI" )]
    public class TextureDatabaseSection : BinaryFileSection<TextureDatabase>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public TextureDatabaseSection( SectionMode mode, TextureDatabase data = null ) : base( mode, data )
        {
        }
    }
}