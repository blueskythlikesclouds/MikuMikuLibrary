using MikuMikuLibrary.Textures;
using System.IO;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "MTXD", typeof( TextureSet ) )]
    public class TextureSetSection : BinaryFileSection<TextureSet>
    {
        public override SectionFlags Flags => SectionFlags.None;
        public override AddressSpace AddressSpace => AddressSpace.Int32;

        public TextureSetSection( Stream source, TextureSet dataToRead = null ) : base( source, dataToRead )
        {
        }

        public TextureSetSection( TextureSet dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, AddressSpace.Int32 )
        {
        }
    }
}
