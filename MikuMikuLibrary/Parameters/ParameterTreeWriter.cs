using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MikuMikuLibrary.Parameters
{
    public class ParameterTreeWriter
    {
        private readonly StringBuilder mStringBuilder;
        private readonly List<string> mLines;
        private readonly List<object> mScopes;

        private void BeginWrite( string key )
        {
            mStringBuilder.Clear();

            foreach ( var scope in mScopes )
            {
                mStringBuilder.AppendFormat( CultureInfo.InvariantCulture, "{0}", scope );
                mStringBuilder.Append( '.' );
            }

            mStringBuilder.Append( key );
            mStringBuilder.Append( '=' );
        }

        private void EndWrite()
        {
            mStringBuilder.Append( '\n' );
            mLines.Add( mStringBuilder.ToString() );
        }

        public void Write<T>( string key, T value )
        {
            BeginWrite( key );
            {
                mStringBuilder.AppendFormat( CultureInfo.InvariantCulture, "{0}", value );
            }
            EndWrite();
        }

        public void Write<T>( string key, IReadOnlyList<T> list, Action<T> writer )
        {
            PushScope( key );
            {
                for ( int i = 0; i < list.Count; i++ )
                {
                    PushScope( i );
                    {
                        writer( list[ i ] );
                    }
                    PopScope();
                }

                Write( "length", list.Count );
            }
            PopScope();
        }

        public void PushScope<T>( T scope )
        {
            mScopes.Add( scope );
        }

        public void PopScope()
        {
            mScopes.RemoveAt( mScopes.Count - 1 );
        }

        public void Flush( Stream stream )
        {
            var buffer = new byte[ 4096 ];

            foreach ( string line in mLines.OrderBy( x => x, StringComparer.Ordinal ) )
            {
                int count = Encoding.UTF8.GetBytes( line, 0, line.Length, buffer, 0 );
                stream.Write( buffer, 0, count );
            }
        }

        public ParameterTreeWriter()
        {
            mStringBuilder = new StringBuilder();
            mLines = new List<string>();
            mScopes = new List<object>();
        }
    }
}