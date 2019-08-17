//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System.IO;

namespace MikuMikuLibrary.IO.Common
{
    public static class StreamExtensions
    {
        public static StreamView CreateSubView( this Stream stream, long position, long length, bool leaveOpen = true )
        {
            return new StreamView( stream, position, length, leaveOpen );
        }
    }
}