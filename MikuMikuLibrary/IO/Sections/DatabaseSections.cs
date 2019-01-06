using MikuMikuLibrary.Databases;
using System.IO;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "AEDB", typeof( AetDatabase ) )]
    public class AetDatabaseSection : BinaryFileSection<AetDatabase>
    {
        public override SectionFlags Flags => SectionFlags.RelocationTableSection;

        public AetDatabaseSection( Stream source, AetDatabase dataToRead = null ) : base( source, dataToRead )
        {
        }

        public AetDatabaseSection( AetDatabase dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
        }
    }

    [Section( "BONE", typeof( BoneDatabase ) )]
    public class BoneDatabaseSection : BinaryFileSection<BoneDatabase>
    {
        public override SectionFlags Flags => SectionFlags.RelocationTableSection;

        public BoneDatabaseSection( Stream source, BoneDatabase dataToRead = null ) : base( source, dataToRead )
        {
        }

        public BoneDatabaseSection( BoneDatabase dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
        }
    }

    [Section( "MOSI", typeof( ObjectDatabase ) )]
    public class ObjectDatabaseSection : BinaryFileSection<ObjectDatabase>
    {
        public override SectionFlags Flags => SectionFlags.RelocationTableSection;

        public ObjectDatabaseSection( Stream source, ObjectDatabase dataToRead = null ) : base( source, dataToRead )
        {
        }

        public ObjectDatabaseSection( ObjectDatabase dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
        }
    }

    [Section( "SPDB", typeof( SpriteDatabase ) )]
    public class SpriteDatabaseSection : BinaryFileSection<SpriteDatabase>
    {
        public override SectionFlags Flags => SectionFlags.RelocationTableSection;

        public SpriteDatabaseSection( Stream source, SpriteDatabase dataToRead = null ) : base( source, dataToRead )
        {
        }

        public SpriteDatabaseSection( SpriteDatabase dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
        }
    }

    [Section( "MTXI", typeof( TextureDatabase ) )]
    public class TextureDatabaseSection : BinaryFileSection<TextureDatabase>
    {
        public override SectionFlags Flags => SectionFlags.RelocationTableSection;

        public TextureDatabaseSection( Stream source, TextureDatabase dataToRead = null ) : base( source, dataToRead )
        {
        }

        public TextureDatabaseSection( TextureDatabase dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
        }
    }
}
