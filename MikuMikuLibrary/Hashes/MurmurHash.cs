using System.Text;

namespace MikuMikuLibrary.Hashes
{
    public static unsafe class MurmurHash
    {
        public static uint Calculate( byte* data, int length )
        {
            unchecked
            {
                const uint m = 0x7FD652AD;
                const int r = 16;

                uint hash = 0xDEADBEEF;

                while ( length >= 4 )
                {
                    hash += *( uint* ) data;
                    hash *= m;
                    hash ^= hash >> r;

                    data += 4;
                    length -= 4;
                }

                switch ( length )
                {
                    case 3:
                        hash += ( uint ) ( data[ 2 ] << 16 );
                        goto case 2;
                    case 2:
                        hash += ( uint ) ( data[ 1 ] << 8 );
                        goto case 1;
                    case 1:
                        hash += data[ 0 ];
                        hash *= m;
                        hash ^= hash >> r;
                        break;
                }

                hash *= m;
                hash ^= hash >> 10;
                hash *= m;
                hash ^= hash >> 17;

                return hash;
            }
        }

        public static uint Calculate( string value, Encoding encoding )
        {
            // TODO: Allocations for each iteration is unnecessary. Find a convenient solution.
            // ArrayPool would be useful but we're not on .NET Core yet.

            var buffer = encoding.GetBytes( value.ToUpperInvariant() );
            fixed ( byte* bufferPtr = buffer )
                return Calculate( bufferPtr, buffer.Length );
        }

        public static uint Calculate( string value ) => 
            Calculate( value, Encoding.UTF8 );
    }
}
