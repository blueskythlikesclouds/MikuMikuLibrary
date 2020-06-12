using MikuMikuLibrary.IO.Sections.IO;
using MikuMikuLibrary.Motions;

namespace MikuMikuLibrary.IO.Sections.Motions
{
    [Section( "MOTC" )]
    public class MotionSection : BinaryFileSection<Motion>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public MotionSection( SectionMode mode, Motion data = null ) : base( mode, data )
        {
        }
    }
}