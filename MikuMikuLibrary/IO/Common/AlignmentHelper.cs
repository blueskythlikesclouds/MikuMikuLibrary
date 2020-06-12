//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System.Runtime.CompilerServices;

namespace MikuMikuLibrary.IO.Common
{
    public static class AlignmentHelper
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long Align( long value, int alignment )
        {
            return ( value + ( alignment - 1 ) ) & ~( alignment - 1 );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int Align( int value, int alignment )
        {
            return ( value + ( alignment - 1 ) ) & ~( alignment - 1 );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int GetAlignedDifference( long value, int alignment )
        {
            return ( int ) ( ( ( value + ( alignment - 1 ) ) & ~( alignment - 1 ) ) - value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int GetAlignedDifference( int value, int alignment )
        {
            return ( ( value + ( alignment - 1 ) ) & ~( alignment - 1 ) ) - value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int AlignToNextPowerOfTwo( int value )
        {
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;

            return value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint AlignToNextPowerOfTwo( uint value )
        {
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;

            return value;
        }
    }
}