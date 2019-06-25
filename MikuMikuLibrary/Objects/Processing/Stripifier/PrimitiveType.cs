//========================================================//
// Taken from: https://github.com/TGEnigma/NvTriStrip.Net //
//========================================================//

namespace NvTriStripDotNet
{
    /// <summary>
    /// Enumeration for the types of primitives used in primitive groups.
    /// </summary>
    public enum PrimitiveType
    {
        /// <summary>
        /// List of triangles consisting out of 3 indices per triangle.
        /// </summary>
        TriangleList,

        /// <summary>
        /// Triangle strip consisting out of 2 base indices, with each subsequent index forming a new triangle.
        /// </summary>
        TriangleStrip,

        /// <summary>
        /// Triangle fan consisting out of a center vertex index, with each subsequent pair of indices forming a new triangle.
        /// </summary>
        TriangleFan,
    }
}