using System;
using NvTriStripDotNet;

namespace MikuMikuLibrary.Objects.Processing
{
    public static class Stripifier
    {
        private static readonly NvStripifier sStripifier = new NvStripifier
        {
            CacheSize = NvStripifier.CACHESIZE_RSX
        };

        public static uint[] Stripify( uint[] indices )
        {
            // TODO: Make NvTriStrip operate on unsigned integers.

            sStripifier.GenerateStrips( Array.ConvertAll( indices, x => ( ushort ) x ), out var primitiveGroups );

            if ( primitiveGroups.Length == 1 &&
                 primitiveGroups[ 0 ].Type == NvTriStripDotNet.PrimitiveType.TriangleStrip )
                return Array.ConvertAll( primitiveGroups[ 0 ].Indices, x => x == 0xFFFF ? 0xFFFFFFFF : ( uint ) x );

            return null;
        }
    }
}