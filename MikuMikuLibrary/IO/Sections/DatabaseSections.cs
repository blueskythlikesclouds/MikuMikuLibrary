using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO.Common;
using System.IO;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "AEDB", typeof( AetDatabase ) )]
    public class AetDatabaseSection : BinaryFileSection<AetDatabase>
    {
        public AetDatabaseSection( Stream source, AetDatabase dataToRead = null ) : base( source, dataToRead )
        {
        }

        public AetDatabaseSection( AetDatabase dataToWrite, Endianness endianness ) : base( dataToWrite, endianness )
        {
        }
    }

    [Section( "BONE", typeof( BoneDatabase ) )]
    public class BoneDatabaseSection : BinaryFileSection<BoneDatabase>
    {
        public BoneDatabaseSection( Stream source, BoneDatabase dataToRead = null ) : base( source, dataToRead )
        {
        }

        public BoneDatabaseSection( BoneDatabase dataToWrite, Endianness endianness ) : base( dataToWrite, endianness )
        {
        }
    }

    [Section( "MOSI", typeof( ObjectDatabase ) )]
    public class ObjectDatabaseSection : BinaryFileSection<ObjectDatabase>
    {
        public ObjectDatabaseSection( Stream source, ObjectDatabase dataToRead = null ) : base( source, dataToRead )
        {
        }

        public ObjectDatabaseSection( ObjectDatabase dataToWrite, Endianness endianness ) : base( dataToWrite, endianness )
        {
        }
    }

    [Section( "MTXI", typeof( TextureDatabase ) )]
    public class TextureDatabaseSection : BinaryFileSection<TextureDatabase>
    {
        public TextureDatabaseSection( Stream source, TextureDatabase dataToRead = null ) : base( source, dataToRead )
        {
        }

        public TextureDatabaseSection( TextureDatabase dataToWrite, Endianness endianness ) : base( dataToWrite, endianness )
        {
        }
    }
}
