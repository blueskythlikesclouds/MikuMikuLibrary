using System.IO;

namespace MikuMikuLibrary.IO.Sections
{
    public abstract class Section<T> : Section where T : class
    {
        public new T Data => ( T )base.Data;

        public Section( Stream source, T dataToRead = null ) : base( source, dataToRead )
        {
        }

        public Section( T dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
        }
    }
}
