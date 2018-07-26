using System;
using System.IO;

namespace MikuMikuLibrary.Archives.Farc
{
    internal class InternalEntry : IDisposable
    {
        public string Handle;
        public Stream Stream;
        public bool OwnsStream;

        public InternalEntry( string handle, Stream stream, bool ownsStream )
        {
            Handle = handle;
            Stream = stream;
            OwnsStream = ownsStream;
        }

        public void Dispose()
        {
            if ( OwnsStream && Stream != null )
                Stream.Dispose();
        }
    }

}
