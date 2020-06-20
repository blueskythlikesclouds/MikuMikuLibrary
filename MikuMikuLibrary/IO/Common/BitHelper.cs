using System.Runtime.CompilerServices;

namespace MikuMikuLibrary.IO.Common
{
    public static class BitHelper
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static byte Unpack( byte value, int from, int to )
        {
            return ( byte ) ( ( value >> from ) & ( byte.MaxValue >> ( sizeof( byte ) * 8 - ( to - from ) ) ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ushort Unpack( ushort value, int from, int to )
        {
            return ( ushort ) ( ( value >> from ) & ( ushort.MaxValue >> ( sizeof( ushort ) * 8 - ( to - from ) ) ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint Unpack( uint value, int from, int to )
        {
            return ( value >> from ) & ( uint.MaxValue >> ( sizeof( uint ) * 8 - ( to - from ) ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong Unpack( ulong value, int from, int to )
        {
            return ( value >> from ) & ( ulong.MaxValue >> ( sizeof( ulong ) * 8 - ( to - from ) ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void Pack( ref byte destination, byte value, int from, int to )
        {
            int mask = byte.MaxValue >> ( sizeof( byte ) * 8 - ( to - from ) );
            destination = ( byte ) ( ( destination & ~( mask << from ) ) | ( ( value & mask ) << from ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void Pack( ref ushort destination, ushort value, int from, int to )
        {
            int mask = ushort.MaxValue >> ( sizeof( ushort ) * 8 - ( to - from ) );
            destination = ( ushort ) ( ( destination & ~( mask << from ) ) | ( ( value & mask ) << from ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void Pack( ref uint destination, uint value, int from, int to )
        {
            uint mask = uint.MaxValue >> ( sizeof( uint ) * 8 - ( to - from ) );
            destination = ( destination & ~( mask << from ) ) | ( ( value & mask ) << from );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void Pack( ref ulong destination, ulong value, int from, int to )
        {
            ulong mask = ulong.MaxValue >> ( sizeof( ulong ) * 8 - ( to - from ) );
            destination = ( destination & ~( mask << from ) ) | ( ( value & mask ) << from );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static byte Pack( byte destination, byte value, int from, int to )
        {
            int mask = byte.MaxValue >> ( sizeof( byte ) * 8 - ( to - from ) );
            return ( byte ) ( ( destination & ~( mask << from ) ) | ( ( value & mask ) << from ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ushort Pack( ushort destination, ushort value, int from, int to )
        {
            int mask = ushort.MaxValue >> ( sizeof( ushort ) * 8 - ( to - from ) );
            return ( ushort ) ( ( destination & ~( mask << from ) ) | ( ( value & mask ) << from ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint Pack( uint destination, uint value, int from, int to )
        {
            uint mask = uint.MaxValue >> ( sizeof( uint ) * 8 - ( to - from ) );
            return ( destination & ~( mask << @from ) ) | ( ( value & mask ) << @from );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong Pack( ulong destination, ulong value, int from, int to )
        {
            ulong mask = ulong.MaxValue >> ( sizeof( ulong ) * 8 - ( to - from ) );
            return ( destination & ~( mask << from ) ) | ( ( value & mask ) << from );
        }
    }
}