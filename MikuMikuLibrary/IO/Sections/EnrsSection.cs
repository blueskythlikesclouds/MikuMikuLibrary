using System.Collections.Generic;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections.Enrs;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "ENRS" )]
    public class EnrsSection : Section<List<ScopeDescriptor>>
    {
        public override SectionFlags Flags => SectionFlags.HasNoRelocationTable;
        public override Endianness Endianness => Endianness.Little;

        protected override void Read( List<ScopeDescriptor> data, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( List<ScopeDescriptor> data, EndianBinaryWriter writer )
        {
            // Thanks to korenkonder for figuring the structure out.

            writer.WriteNulls( sizeof( uint ) );
            writer.Write( data.Count );
            writer.WriteNulls( 2 * sizeof( uint ) );

            ScopeDescriptor.WriteDescriptors( writer, data );
        }

        public EnrsSection( SectionMode mode, List<ScopeDescriptor> data = default ) : base( mode, data )
        {
        }
    }
}