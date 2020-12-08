using System;
using System.Collections.Generic;
using System.Globalization;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Parameters
{
    public class ParameterTree
    {
        private readonly Dictionary<string, object> mValues;
        private readonly Stack<ParameterTree> mScopeStack;

        public IEnumerable<string> Keys => mValues.Keys;

        public bool OpenScope<T>( T key )
        {
            var paramTree = mScopeStack.Count > 0 ? mScopeStack.Peek() : this;

            if ( !paramTree.mValues.TryGetValue( $"{key}", out var value ) || !( value is ParameterTree subParamTree ) )
                return false;

            mScopeStack.Push( subParamTree );
            return true;
        }

        public void CloseScope()
        {
            mScopeStack.Pop();
        }

        public T Get<T>( string key, T fallback = default )
        {
            var paramTree = mScopeStack.Count > 0 ? mScopeStack.Peek() : this;

            if ( paramTree.mValues.TryGetValue( key, out var value ) )
                return ( T ) Convert.ChangeType( value, typeof( T ), CultureInfo.InvariantCulture );

            return fallback;
        }

        public void Enumerate( string key, Action<int> handler )
        {
            if ( !OpenScope( key ) )
                return;

            int length = Get<int>( "length" );

            for ( int i = 0; i < length; i++ )
            {
                if ( !OpenScope( i ) )
                    continue;

                handler( i );
                CloseScope();
            }

            CloseScope();
        }

        public ParameterTree()
        {
            mValues = new Dictionary<string, object>( StringComparer.OrdinalIgnoreCase );
        }

        public ParameterTree( EndianBinaryReader reader )
        {
            mValues = new Dictionary<string, object>( StringComparer.OrdinalIgnoreCase );
            mScopeStack = new Stack<ParameterTree>();

            while ( reader.Position < reader.Length )
            {
                string line = reader.ReadLine();

                if ( string.IsNullOrEmpty( line ) || line.StartsWith( "#" ) )
                    continue;

                int equalIndex = line.IndexOf( '=' );

                if ( equalIndex == -1 )
                    continue;

                var paramTree = this;

                int periodIndex = -1;

                while ( true )
                {  
                    int nextPeriodIndex = line.IndexOf( '.', periodIndex + 1 );

                    string key;

                    if ( nextPeriodIndex == -1 || nextPeriodIndex > equalIndex )
                    {
                        key = line.Substring( periodIndex + 1, equalIndex - periodIndex - 1 ).Trim();

                        paramTree.mValues[ key ] = line.Substring( equalIndex + 1 ).Trim();
                        break;
                    }

                    key = line.Substring( periodIndex + 1, nextPeriodIndex - periodIndex - 1 ).Trim();

                    if ( !paramTree.mValues.TryGetValue( key, out var subParamTree ) )
                        paramTree.mValues[ key ] = subParamTree = new ParameterTree();

                    paramTree = ( ParameterTree ) subParamTree;

                    periodIndex = nextPeriodIndex;
                }
            }
        }
    }
}