using MikuMikuLibrary.Sprites;
using MikuMikuLibrary.Textures;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "SPRC" )]
    public class SpriteSetSection : BinaryFileSection<SpriteSet>
    {
        public override SectionFlags Flags => SectionFlags.HasRelocationTable;

        [SubSection( typeof( SpriteTextureSetSection ) )]
        public TextureSet TextureSet
        {
            get => DataObject.TextureSet;
            set => DataObject.TextureSet = value;
        }

        public SpriteSetSection( SectionMode mode, SpriteSet dataObject = null ) : base( mode, dataObject )
        {
        }
    }

    [Section( "TXPC" )]
    public class SpriteTextureSetSection : BinaryFileSection<TextureSet>
    {
        public override SectionFlags Flags => SectionFlags.None;
        public override AddressSpace AddressSpace => AddressSpace.Int32;

        public SpriteTextureSetSection( SectionMode mode, TextureSet dataObject = null ) : base( mode, dataObject )
        {
        }
    }
}
