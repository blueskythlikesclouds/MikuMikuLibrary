using MikuMikuLibrary.IO.Common;
using System.IO;

namespace MikuMikuLibrary.IO.Sections
{
    public abstract class BinaryFileSection<T> : Section<T> where T : BinaryFile
    {
        protected override void Read( EndianBinaryReader reader, long length )
        {
            Data.Endianness = reader.Endianness;
            Data.Read( reader, this );
        }

        protected override void Write( EndianBinaryWriter writer )
        {
            Data.Endianness = writer.Endianness;
            Data.Write( writer, this );
        }

        public BinaryFileSection( Stream source, T dataToRead = null ) : base( source, dataToRead )
        {
        }

        public BinaryFileSection( T dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
        }
    }
}
