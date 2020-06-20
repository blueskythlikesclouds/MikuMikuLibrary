using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Objects;

namespace MikuMikuLibrary.IO.Sections.Objects
{
    [Section( "OSKN" )]
    public class SkinSection : Section<Skin>
    {
        public override SectionFlags Flags => SectionFlags.None;

        protected override void Read( Skin data, EndianBinaryReader reader, long length ) => 
            data.Read( reader );

        protected override void Write( Skin data, EndianBinaryWriter writer ) => 
            data.Write( writer );

        public SkinSection( SectionMode mode, Skin data = null ) : base( mode, data )
        {
        }
    }
}