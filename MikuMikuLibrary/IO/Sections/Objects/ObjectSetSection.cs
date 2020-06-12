using System.Collections.Generic;
using MikuMikuLibrary.IO.Sections.IO;
using MikuMikuLibrary.Objects;

namespace MikuMikuLibrary.IO.Sections.Objects
{
    [Section( "MOSD" )]
    public class ObjectSetSection : BinaryFileSection<ObjectSet>
    {
        public override SectionFlags Flags => SectionFlags.None;

        [SubSection( typeof( ObjectSection ) )]
        public List<Object> Objects => Data.Objects;

        public ObjectSetSection( SectionMode mode, ObjectSet data = null ) : base( mode, data )
        {
        }
    }
}