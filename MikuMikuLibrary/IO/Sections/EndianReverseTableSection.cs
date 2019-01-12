using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "ENRS" )]
    public class EndianReverseTableSection : Section<object>
    {
        public override SectionFlags Flags => SectionFlags.None;

        protected override void Read( object dataObject, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( object dataObject, EndianBinaryWriter writer )
        {
        }

        public EndianReverseTableSection( SectionMode mode, object dataObject = null ) : base( mode, dataObject )
        {
        }
    }
}
