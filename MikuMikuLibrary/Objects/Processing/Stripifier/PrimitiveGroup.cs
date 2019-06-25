//========================================================//
// Taken from: https://github.com/TGEnigma/NvTriStrip.Net //
//========================================================//

namespace NvTriStripDotNet
{
    /// <summary>
    /// A primitive group contains optimized and/or stripped index data in a specified primitive format.
    /// </summary>
    public class PrimitiveGroup
    {
        /// <summary>
        /// Gets the primitive type of the group.
        /// </summary>
        public PrimitiveType Type { get; }

        /// <summary>
        /// Gets the number of indices in the index array.
        /// </summary>
        public int IndexCount => Indices.Length;

        /// <summary>
        /// Gets the indices of the group.
        /// </summary>
        public ushort[] Indices { get; internal set; }

        internal PrimitiveGroup( PrimitiveType type, ushort[] indices )
        {
            Type = type;
            Indices = indices;
        }
    }
}