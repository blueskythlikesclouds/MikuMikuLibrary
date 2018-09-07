using MikuMikuLibrary.IO.Common;
using System.IO;

namespace MikuMikuLibrary.IO.Sections
{
    // Work in progress.
    [Section( "ENRS", typeof( object ) )]
    public class EnrsSection : Section
    {
        public override Endianness Endianness
        {
            get { return Endianness.LittleEndian; }
        }

        public override SectionFlags Flags
        {
            get { return SectionFlags.None; }
        }

        protected override void Read( EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( EndianBinaryWriter writer )
        {
        }

        public EnrsSection( Stream source, object dataToRead = null ) : base( source, dataToRead )
        {
        }

        public EnrsSection( object dataToWrite ) : base( dataToWrite, Endianness.LittleEndian )
        {
        }
    }
}
