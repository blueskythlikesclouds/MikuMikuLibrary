using System.Collections.Generic;

namespace MikuMikuLibrary.Extensions
{
    public static class ListEx
    {
        public static int IndexOf<T>( this IReadOnlyList<T> list, T item )
        {
            for ( int i = 0; i < list.Count; i++ )
            {
                if ( list[ i ].Equals( item ) )
                    return i;
            }

            return -1;
        }
    }
}