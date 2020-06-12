using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "EOFC" )]
    public class EndOfFileSection : Section<object>
    {
        public override SectionFlags Flags => SectionFlags.HasNoRelocationTable;
        public override Endianness Endianness => Endianness.Little;
        public override AddressSpace AddressSpace => AddressSpace.Int32;

        protected override void Read( object data, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( object data, EndianBinaryWriter writer )
        {
        }

        public EndOfFileSection( SectionMode mode, object data = null ) : base( mode, data )
        {
        }
    }
}