using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuLibrary.Sprites;
using MikuMikuLibrary.Textures;

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

        public SpriteSetSection( SpriteSet dataToWrite, Endianness endianness ) : base( dataToWrite, endianness )
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

        public SpriteTextureSetSection( TextureSet dataToWrite, Endianness endianness ) : base( dataToWrite, endianness )
        {
        }
    }
}
