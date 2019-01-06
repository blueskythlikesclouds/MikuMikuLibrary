using MikuMikuLibrary.Sprites;
using MikuMikuLibrary.Textures;
using System.IO;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "SPRC", typeof( SpriteSet ) )]
    public class SpriteSetSection : BinaryFileSection<SpriteSet>
    {
        public override SectionFlags Flags => SectionFlags.RelocationTableSection;

        [SubSection( typeof( SpriteTextureSetSection ) )]
        public TextureSet TextureSet => Data.TextureSet;

        public SpriteSetSection( Stream source, SpriteSet dataToRead = null ) : base( source, dataToRead )
        {
        }

        public SpriteSetSection( SpriteSet dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
        }
    }

    [Section( "TXPC", typeof( TextureSet ) )]
    public class SpriteTextureSetSection : BinaryFileSection<TextureSet>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public SpriteTextureSetSection( Stream source, TextureSet dataToRead = null ) : base( source, dataToRead )
        {
        }

        public SpriteTextureSetSection( TextureSet dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
        }
    }
}
