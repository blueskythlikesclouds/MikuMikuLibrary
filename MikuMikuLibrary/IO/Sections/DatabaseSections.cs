using MikuMikuLibrary.Databases;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "AEDB" )]
    public class AetDatabaseSection : BinaryFileSection<AetDatabase>
    {
        public AetDatabaseSection( SectionMode mode, AetDatabase dataObject = null ) : base( mode, dataObject )
        {
        }

        public override SectionFlags Flags => SectionFlags.HasRelocationTable;
    }

    [Section( "BONE" )]
    public class BoneDatabaseSection : BinaryFileSection<BoneDatabase>
    {
        public BoneDatabaseSection( SectionMode mode, BoneDatabase dataObject = null ) : base( mode, dataObject )
        {
        }

        public override SectionFlags Flags => SectionFlags.HasRelocationTable;
    }

    [Section( "MOSI" )]
    public class ObjectDatabaseSection : BinaryFileSection<ObjectDatabase>
    {
        public ObjectDatabaseSection( SectionMode mode, ObjectDatabase dataObject = null ) : base( mode, dataObject )
        {
        }

        public override SectionFlags Flags => SectionFlags.HasRelocationTable;
    }

    [Section( "SPDB" )]
    public class SpriteDatabaseSection : BinaryFileSection<SpriteDatabase>
    {
        public SpriteDatabaseSection( SectionMode mode, SpriteDatabase dataObject = null ) : base( mode, dataObject )
        {
        }

        public override SectionFlags Flags => SectionFlags.HasRelocationTable;
    }

    [Section( "MTXI" )]
    public class TextureDatabaseSection : BinaryFileSection<TextureDatabase>
    {
        public TextureDatabaseSection( SectionMode mode, TextureDatabase dataObject = null ) : base( mode, dataObject )
        {
        }

        public override SectionFlags Flags => SectionFlags.HasRelocationTable;
    }
}
