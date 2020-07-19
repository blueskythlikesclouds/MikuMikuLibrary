using System;
using System.Collections.Generic;
using System.IO;

namespace MikuMikuModel.Resources
{
    public static class ValueCache
    {
        private static readonly string sValueCacheFilePath = ResourceStore.GetPath( "ValueCache.txt" );

        private static readonly Dictionary<string, object> sValueCache;

        public static T Get<T>( string key, T fallback = default )
        {
            if ( !sValueCache.TryGetValue( key, out var value ) || !( value is T cast ) )
                return fallback;

            return cast;
        }

        public static void Set<T>( string key, T value )
        {
            bool success = sValueCache.TryGetValue( key, out var previous );

            sValueCache[ key ] = value;

            if ( success && previous != null && previous.Equals( value ) )  
                return;

            using ( var writer = File.CreateText( sValueCacheFilePath ) )
            {
                foreach ( var kvp in sValueCache )
                    writer.WriteLine( "{0}, {1}, {2}", kvp.Key, kvp.Value?.GetType()?.FullName, kvp.Value );
            }
        }

        static ValueCache()
        {
            sValueCache = new Dictionary<string, object>( StringComparer.OrdinalIgnoreCase );

            if ( !File.Exists( sValueCacheFilePath ) ) 
                return;

            using ( var reader = File.OpenText( sValueCacheFilePath ) )
            {
                while ( true )
                {
                    string line = reader.ReadLine();
                    if ( line == null )
                        break;

                    var split = line.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
                    if ( split.Length != 3 )
                        continue;

                    var type = Type.GetType( split[ 1 ].Trim() );
                    if ( type == null )
                        continue;

                    sValueCache[ split[ 0 ].Trim() ] = Convert.ChangeType( split[ 2 ].Trim(), type );
                }
            }
        }
    }
}