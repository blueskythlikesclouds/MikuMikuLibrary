using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MikuMikuModel.Configurations;
using MikuMikuModel.Modules;
using MikuMikuModel.Nodes.IO;

namespace MikuMikuModel.Nodes
{
    public static class NodeFactory
    {
        private static readonly Dictionary<Type, Type> sNodeTypes;

        public static IReadOnlyDictionary<Type, Type> NodeTypes => sNodeTypes;

        public static INode Create( Type type, string name, object data )
        {
            if ( !NodeTypes.TryGetValue( type, out var nodeType ) )
                return null;

            object[] args = { name, data };
            return Activator.CreateInstance( nodeType,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, args, null ) as INode;
        }

        public static INode Create<T>( string name, T data )
        {
            if ( !NodeTypes.TryGetValue( typeof( T ), out var nodeType ) )
                return null;

            object[] args = { name, data };
            return Activator.CreateInstance( nodeType,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, args, null ) as INode;
        }

        public static INode Create( string filePath, IEnumerable<Type> typesToMatch )
        {
            var module = ModuleImportUtilities.GetModule( filePath );
            if ( module == null || !NodeTypes.ContainsKey( module.ModelType ) )
                throw new InvalidDataException( "File type could not be determined." );

            ConfigurationList.Instance.DetermineCurrentConfiguration( filePath );
            return Create( module.ModelType, Path.GetFileName( filePath ), module.Import( filePath ) );
        }

        public static INode Create( string filePath )
        {
            return Create( filePath, FormatModuleRegistry.ModelTypes );
        }

        static NodeFactory()
        {
            sNodeTypes = new Dictionary<Type, Type>();

            var assembly = Assembly.GetExecutingAssembly();

            var types = assembly.GetTypes().Where(
                x => typeof( INode ).IsAssignableFrom( x ) && x.IsClass && !x.IsAbstract );

            foreach ( var type in types )
                for ( var baseType = type.BaseType; baseType != null; baseType = baseType.BaseType )
                    if ( baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof( Node<> ) )
                    {
                        sNodeTypes[ baseType.GetGenericArguments()[ 0 ] ] = type;
                        break;
                    }
        }
    }
}