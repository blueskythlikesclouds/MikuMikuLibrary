//========================================================//
// Taken from: https://github.com/TGEnigma/NvTriStrip.Net //
//========================================================//

namespace NvTriStripDotNet
{
    /// <summary>
    /// Nice and dumb edge class that points knows its
    /// indices, the two faces, and the next edge using
    /// the lesser of the indices.
    /// </summary>
    internal class EdgeInfo
    {
        public FaceInfo Face0;
        public FaceInfo Face1;
        public int V0;
        public int V1;
        public EdgeInfo NextV0;
        public EdgeInfo NextV1;

        public EdgeInfo( int v0, int v1 )
        {
            V0 = v0;
            V1 = v1;
            Face0 = null;
            Face1 = null;
            NextV0 = null;
            NextV1 = null;
        }
    }
}