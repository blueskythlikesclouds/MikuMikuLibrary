//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System;

namespace MikuMikuLibrary.IO.Common
{
    public static unsafe class UnsafeUtilities
    {
        public static TDest ReinterpretCast<TSource, TDest>( TSource source )
        {
            var sourceRef = __makeref(source);
            var dest = default( TDest );
            var destRef = __makeref(dest);
            *( IntPtr* )&destRef = *( ( IntPtr* )&sourceRef );
            return __refvalue(destRef, TDest);
        }

        public static void ReinterpretCast<TSource, TDest>( TSource source, out TDest destination )
        {
            var sourceRef = __makeref(source);
            var dest = default( TDest );
            var destRef = __makeref(dest);
            *( IntPtr* )&destRef = *( ( IntPtr* )&sourceRef );
            destination = __refvalue(destRef, TDest);
        }
    }
}
