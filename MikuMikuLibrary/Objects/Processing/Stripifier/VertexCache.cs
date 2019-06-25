//========================================================//
// Taken from: https://github.com/TGEnigma/NvTriStrip.Net //
//========================================================//

using System.Runtime.CompilerServices;

namespace NvTriStripDotNet
{
    internal class VertexCache
    {
        private readonly int[] mEntries;

        public VertexCache( int size )
        {
            mEntries = new int[ size ];

            for ( var i = 0; i < mEntries.Length; i++ )
                mEntries[ i ] = -1;
        }

        public VertexCache() : this( 16 ) { }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool InCache( int entry )
        {
            for ( int i = 0; i < mEntries.Length; i++ )
                if ( mEntries[ i ] == entry )
                    return true;

            return false;
        }

        public int AddEntry( int entry )
        {
            int removed = mEntries[ mEntries.Length - 1 ];

            //push everything right one
            for ( int i = mEntries.Length - 2; i >= 0; i-- )
            {
                mEntries[ i + 1 ] = mEntries[ i ];
            }

            mEntries[ 0 ] = entry;

            return removed;
        }

        public void Clear()
        {
            for ( var i = 0; i < mEntries.Length; ++i )
                mEntries[ i ] = -1;
        }

        public void Copy( VertexCache inVcache )
        {
            for ( var i = 0; i < mEntries.Length; i++ )
            {
                inVcache.Set( i, mEntries[ i ] );
            }
        }

        public int At( int index ) { return mEntries[ index ]; }

        public void Set( int index, int value ) { mEntries[ index ] = value; }
    }
}