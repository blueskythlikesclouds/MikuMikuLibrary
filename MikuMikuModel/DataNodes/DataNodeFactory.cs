using MikuMikuModel.FormatModules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MikuMikuModel.DataNodes
{
    public static class DataNodeFactory
    {
        private static readonly Dictionary<Type, Type> sDataNodeTypes;

        public static IReadOnlyDictionary<Type, Type> DataNodeTypes => sDataNodeTypes;

        public static void Register( Type dataType, Type nodeType )
        {
            if ( !typeof( DataNode ).IsAssignableFrom( nodeType ) )
                throw new ArgumentException( "Node type doesn't inherit IDataNode", nameof( nodeType ) );

            if ( sDataNodeTypes.ContainsKey( dataType ) )
                throw new ArgumentException( "Library already contains the data type", nameof( dataType ) );

            sDataNodeTypes.Add( dataType, nodeType );
        }

        public static DataNode Create( Type type, string name, object data )
        {
            if ( !DataNodeTypes.TryGetValue( type, out Type nodeType ) )
                return null;

            object[] args = new[] { name, data };
            return Activator.CreateInstance( nodeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, args, null ) as DataNode;
        }

        public static DataNode Create<T>( string name, object data )
        {
            return Create( typeof( T ), name, data );
        }

        public static DataNode Create( Stream source, IEnumerable<Type> typesToCheck, string fileName = null )
        {
            var module = FormatModuleUtilities.GetModuleForImport( source, typesToCheck, fileName );
            if ( module != null && DataNodeTypes.ContainsKey( module.ModelType ) )
                return Create( module.ModelType, fileName, module.Import( source, fileName ) );

            return new StreamNode( fileName, source );
        }

        public static DataNode Create( string filePath, IEnumerable<Type> typesToCheck )
        {
            IFormatModule module;

            using ( var stream = File.OpenRead( filePath ) )
                module = FormatModuleUtilities.GetModuleForImport( stream, typesToCheck, Path.GetFileName( filePath ) );

            if ( module != null && DataNodeTypes.ContainsKey( module.ModelType ) )
                return Create( module.ModelType, Path.GetFileName( filePath ), module.Import( filePath ) );

            return new StreamNode( Path.GetFileName( filePath ), File.OpenRead( filePath ) );
        }

        public static DataNode Create( Stream source, string fileName = null )
        {
            return Create( source, FormatModuleRegistry.ModelTypes, fileName );
        }

        public static DataNode Create( string filePath )
        {
            return Create( filePath, FormatModuleRegistry.ModelTypes );
        }

        public static string GetSpecialName( Type type )
        {
            var name = ( type.GetCustomAttributes( false ).FirstOrDefault(
                x => x is DataNodeSpecialNameAttribute ) as DataNodeSpecialNameAttribute )?.Name;

            if ( string.IsNullOrEmpty( name ) && type.Name.EndsWith( "Node", StringComparison.OrdinalIgnoreCase ) )
                return type.Name.Substring( 0, type.Name.Length - 4 );

            return name;
        }

        static DataNodeFactory()
        {
            sDataNodeTypes = new Dictionary<Type, Type>();

            // Try registering types with at least one generic type on the base type
            var assembly = Assembly.GetEntryAssembly();
            var types = assembly.GetTypes().Where( x => typeof( DataNode ).IsAssignableFrom( x ) && x.IsClass && !x.IsAbstract );
            foreach ( var type in types )
            {
                var baseType = type.BaseType;
                if ( baseType != null && baseType.IsGenericType )
                    Register( baseType.GetGenericArguments()[ 0 ], type );
            }
        }
    }
}
