//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System;
using System.Linq;

namespace MikuMikuLibrary.IO.Common
{
    public static class EndiannessHelper
    {
        public static Endianness SystemEndianness
        {
            get
            {
                if ( BitConverter.IsLittleEndian )
                    return Endianness.Little;
                return Endianness.Big;
            }
        }

        public static short Swap( short value )
        {
            return ( short ) ( ( value << 8 ) | ( ( value >> 8 ) & 0xFF ) );
        }

        public static void Swap( ref short value )
        {
            value = Swap( value );
        }

        public static ushort Swap( ushort value )
        {
            return ( ushort ) ( ( value << 8 ) | ( value >> 8 ) );
        }

        public static void Swap( ref ushort value )
        {
            value = Swap( value );
        }

        public static int Swap( int value )
        {
            value = ( int ) ( ( value << 8 ) & 0xFF00FF00 ) | ( ( value >> 8 ) & 0xFF00FF );
            return ( value << 16 ) | ( ( value >> 16 ) & 0xFFFF );
        }

        public static void Swap( ref int value )
        {
            value = Swap( value );
        }

        public static uint Swap( uint value )
        {
            value = ( ( value << 8 ) & 0xFF00FF00 ) | ( ( value >> 8 ) & 0xFF00FF );
            return ( value << 16 ) | ( value >> 16 );
        }

        public static void Swap( ref uint value )
        {
            value = Swap( value );
        }

        public static long Swap( long value )
        {
            value = ( long ) ( ( ( ulong ) ( value << 8 ) & 0xFF00FF00FF00FF00UL ) |
                               ( ( ulong ) ( value >> 8 ) & 0x00FF00FF00FF00FFUL ) );
            value = ( long ) ( ( ( ulong ) ( value << 16 ) & 0xFFFF0000FFFF0000UL ) |
                               ( ( ulong ) ( value >> 16 ) & 0x0000FFFF0000FFFFUL ) );
            return ( long ) ( ( ulong ) ( value << 32 ) | ( ( ulong ) ( value >> 32 ) & 0xFFFFFFFFUL ) );
        }

        public static void Swap( ref long value )
        {
            value = Swap( value );
        }

        public static ulong Swap( ulong value )
        {
            value = ( ( value << 8 ) & 0xFF00FF00FF00FF00UL ) | ( ( value >> 8 ) & 0x00FF00FF00FF00FFUL );
            value = ( ( value << 16 ) & 0xFFFF0000FFFF0000UL ) | ( ( value >> 16 ) & 0x0000FFFF0000FFFFUL );
            return ( value << 32 ) | ( value >> 32 );
        }

        public static void Swap( ref ulong value )
        {
            value = Swap( value );
        }

        public static unsafe float Swap( float value )
        {
            uint swapped = Swap( *( uint* ) &value );
            return *( float* ) &swapped;
        }

        public static void Swap( ref float value )
        {
            value = Swap( value );
        }

        public static unsafe double Swap( double value )
        {
            ulong swapped = Swap( *( ulong* ) &value );
            return *( double* ) &swapped;
        }

        public static void Swap( ref double value )
        {
            value = Swap( value );
        }

        public static unsafe decimal Swap( decimal value )
        {
            var pData = stackalloc ulong[ 2 ];

            *pData = Swap( *( ulong* ) &value );
            pData++;
            *pData = Swap( *( ( ulong* ) &value + 16 ) );

            return *( decimal* ) pData;
        }

        public static void Swap( ref decimal value )
        {
            value = Swap( value );
        }
    }
}