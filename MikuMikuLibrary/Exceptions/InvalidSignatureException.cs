using System;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuLibrary.Exceptions
{
    public class InvalidSignatureException : Exception
    {
        private const string INT_SIGNATURE_FORMAT = "X00000000";

        public InvalidSignatureException( string readSignature, string expectedSignature )
            : base( string.Format( "Invalid signature {0}, expected {1}", readSignature, expectedSignature ) )
        {
        }

        public InvalidSignatureException( string readSignature, IEnumerable<string> expectedSignatures )
            : this( readSignature, string.Join( ", ", expectedSignatures ) )
        {
        }

        public InvalidSignatureException( int readSignature, int expectedSignature )
            : this( readSignature.ToString( INT_SIGNATURE_FORMAT ), expectedSignature.ToString( INT_SIGNATURE_FORMAT ) )
        {
        }

        public InvalidSignatureException( int readSignature, IEnumerable<int> expectedSignatures )
            : this( readSignature.ToString( INT_SIGNATURE_FORMAT ),
                expectedSignatures.Select( x => x.ToString( INT_SIGNATURE_FORMAT ) ) )
        {
        }
    }
}
