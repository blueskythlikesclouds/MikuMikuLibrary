using MikuMikuLibrary.IO.Sections.IO;
using MikuMikuLibrary.Textures;

namespace MikuMikuLibrary.IO.Sections.Sprites
{
    [Section( "TXPC" )]
    public class SpriteTextureSetSection : BinaryFileSection<TextureSet>
    {
        public override SectionFlags Flags => SectionFlags.None;
        public override AddressSpace AddressSpace => AddressSpace.Int32;

        public SpriteTextureSetSection( SectionMode mode, TextureSet data = null ) : base( mode, data )
        {
        }
    }
}