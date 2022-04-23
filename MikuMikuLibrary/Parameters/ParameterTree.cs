using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Parameters
{
    public class ParameterTree
    {
        private readonly Dictionary<string, object> mValues;
        private readonly Stack<ParameterTree> mScopeStack;

        public ParameterTree Current => mScopeStack.Count > 0 ? mScopeStack.Peek() : this;
        public IEnumerable<string> Keys => Current.mValues.Keys;

        // Read operations

        public bool Contains( string key )
        {
            return Current.mValues.ContainsKey( key );
        }

        public bool OpenScope<T>( T key )
        {
            if ( !Current.mValues.TryGetValue( $"{key}", out var value ) || !( value is ParameterTree subParamTree ) )
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
            if ( Current.mValues.TryGetValue( key, out var value ) )
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

        // Write operations

        public void CreateScope<T>( T key )
        {
            var current = Current;
            string keyStr = $"{key}";
            
            if ( !current.mValues.TryGetValue( keyStr, out var value ) || !( value is ParameterTree subParamTree ) )
                current.mValues[ keyStr ] = ( subParamTree = new ParameterTree() );

            mScopeStack.Push( subParamTree );
        }

        public void Set<T>( string key, T value )
        {
            Current.mValues[ key ] = value;
        }

        public void Write( ParameterTreeWriter writer )
        {
            foreach ( var pair in mValues )
            {
                if ( pair.Value is ParameterTree parameterTree )
                {
                    writer.PushScope( pair.Key );
                    parameterTree.Write( writer );
                    writer.PopScope();
                }

                else
                    writer.Write( pair.Key, pair.Value );
            }
        }

        public static ParameterTree Load( Stream stream, bool leaveOpen = false )
        {
            using ( var reader = new EndianBinaryReader( stream, Encoding.UTF8, Endianness.Little, leaveOpen ) )
                return new ParameterTree( reader );
        }

        public static ParameterTree Load( string filePath )
        {
            using ( var stream = File.OpenRead( filePath ) )
                return Load( stream );
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