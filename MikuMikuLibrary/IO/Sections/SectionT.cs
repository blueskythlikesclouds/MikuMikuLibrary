using System.IO;

namespace MikuMikuLibrary.IO.Sections
{
    public abstract class Section<T> : Section where T : class
    {
        public new T Data
        {
            get { return ( T )base.Data; }
        }

        public Section( Stream source, T dataToRead = null ) : base( source, dataToRead )
        {
        }

        public Section( T dataToWrite, Endianness endianness ) : base( dataToWrite, endianness )
        {
        }
    }
}
