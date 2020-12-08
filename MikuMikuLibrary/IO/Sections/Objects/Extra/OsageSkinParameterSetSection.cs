using MikuMikuLibrary.IO.Sections.IO;
using MikuMikuLibrary.Objects.Extra;

namespace MikuMikuLibrary.IO.Sections.Objects.Extra
{
    [Section("SKPC")]
    public class OsageSkinParameterSetSection : BinaryFileSection<OsageSkinParameterSet>
    {
        public OsageSkinParameterSetSection( SectionMode mode, OsageSkinParameterSet data = default ) : base( mode, data )
        {
        }

        public override SectionFlags Flags => SectionFlags.None;
    }
}