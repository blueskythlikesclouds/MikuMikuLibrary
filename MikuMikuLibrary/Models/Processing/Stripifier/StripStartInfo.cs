//========================================================//
// Taken from: https://github.com/TGEnigma/NvTriStrip.Net //
//========================================================//

namespace NvTriStripDotNet
{
    /// <summary>
    /// This class is a quick summary of parameters used
    /// to begin a triangle strip.  Some operations may
    /// want to create lists of such items, so they were
    /// pulled out into a class
    /// </summary>
    internal class StripStartInfo
    {
        public FaceInfo StartFace;
        public EdgeInfo StartEdge;
        public bool ToV1;

        public StripStartInfo( FaceInfo startFace, EdgeInfo startEdge, bool toV1 )
        {
            StartFace = startFace;
            StartEdge = startEdge;
            ToV1 = toV1;
        }
    };
}