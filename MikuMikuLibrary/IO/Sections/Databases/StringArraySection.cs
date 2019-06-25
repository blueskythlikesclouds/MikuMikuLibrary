﻿using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.Databases
{
    [Section( "STRA" )]
    public class StringArraySection : BinaryFileSection<StringArray>
    {
        public StringArraySection( SectionMode mode, StringArray data = null ) : base( mode, data )
        {
        }

        public override SectionFlags Flags => SectionFlags.HasRelocationTable;
    }
}
