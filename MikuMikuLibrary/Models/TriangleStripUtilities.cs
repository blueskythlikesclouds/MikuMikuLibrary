using NvTriStripDotNet;

namespace MikuMikuLibrary.Models
{
    public static class TriangleStripUtilities
    {
        private static readonly NvStripifier sStripifier = new NvStripifier
        {
            CacheSize = NvStripifier.CACHESIZE_RSX,
        };

        public static ushort[] GenerateStrips( ushort[] indices )
        {
            sStripifier.GenerateStrips( indices, out PrimitiveGroup[] primitiveGroups );

            if ( primitiveGroups.Length == 1 && primitiveGroups[ 0 ].Type == PrimitiveType.TriangleStrip )
                return primitiveGroups[ 0 ].Indices;

            return null;
        }
    }
}
