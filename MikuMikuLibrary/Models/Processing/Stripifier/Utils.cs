//========================================================//
// Taken from: https://github.com/TGEnigma/NvTriStrip.Net //
//========================================================//

using System.Runtime.CompilerServices;

namespace NvTriStripDotNet
{
    /// <summary>
    /// Misc. utility class.
    /// </summary>
    internal static class Utils
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void Swap<T>( ref T first, ref T second )
        {
            var temp = first;
            first = second;
            second = temp;
        }
    }
}