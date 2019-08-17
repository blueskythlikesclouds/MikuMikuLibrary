using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "ENRS" )]
    public class EndianReverseTableSection : Section<object>
    {
        public override SectionFlags Flags => SectionFlags.None;

        protected override void Read( object data, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( object data, EndianBinaryWriter writer )
        {
        }

        public EndianReverseTableSection( SectionMode mode, object data = null ) : base( mode, data )
        {
        }
    }
}