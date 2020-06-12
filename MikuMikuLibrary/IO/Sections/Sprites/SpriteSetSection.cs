using MikuMikuLibrary.IO.Sections.IO;
using MikuMikuLibrary.Sprites;
using MikuMikuLibrary.Textures;

namespace MikuMikuLibrary.IO.Sections.Sprites
{
    [Section( "SPRC" )]
    public class SpriteSetSection : BinaryFileSection<SpriteSet>
    {
        public override SectionFlags Flags => SectionFlags.None;

        [SubSection( typeof( SpriteTextureSetSection ) )]
        public TextureSet TextureSet
        {
            get => Data.TextureSet;
            set => Data.TextureSet = value;
        }

        public SpriteSetSection( SectionMode mode, SpriteSet data = null ) : base( mode, data )
        {
        }
    }
}