using System.Text;
using MikuMikuLibrary.Aets;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.Aets
{
    [Section( "AETC" )]
    public class AetSetSection : BinaryFileSection<AetSet>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public override Encoding Encoding { get; } = Encoding.GetEncoding( "shift-jis" );

        public AetSetSection( SectionMode mode, AetSet data = null ) : base( mode, data )
        {
        }
    }
}