using MikuMikuLibrary.IO.Sections.IO;
using MikuMikuLibrary.Textures;

namespace MikuMikuLibrary.IO.Sections.Textures
{
    [Section( "MTXD" )]
    public class TextureSetSection : BinaryFileSection<TextureSet>
    {
        public override SectionFlags Flags => SectionFlags.None;
        public override AddressSpace AddressSpace => AddressSpace.Int32;

        public TextureSetSection( SectionMode mode, TextureSet data = null ) : base( mode, data )
        {
        }
    }
}