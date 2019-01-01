using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MikuMikuModel.FormatModules
{
    public static class FormatModuleRegistry
    {
        private static readonly Dictionary<Type, IFormatModule> modules = new Dictionary<Type, IFormatModule>();

        public static IReadOnlyDictionary<Type, IFormatModule> ModulesByType => modules;

        public static IEnumerable<Type> ModelTypes => modules.Keys;
        public static IEnumerable<IFormatModule> Modules => modules.Values;

        public static void Register( IFormatModule module )
        {
            if ( modules.ContainsKey( module.ModelType ) )
                throw new ArgumentException( "Format module is already registered", nameof( module ) );

            modules.Add( module.ModelType, module );
        }

        public static bool TryRegister( IFormatModule module )
        {
            if ( modules.ContainsKey( module.ModelType ) )
                return false;

            modules.Add( module.ModelType, module );
            return true;
        }

        static FormatModuleRegistry()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var types = assembly.GetTypes().Where(
                x => typeof( IFormatModule ).IsAssignableFrom( x ) && x.IsClass && !x.IsAbstract );

            foreach ( var type in types )
            {
                var instance = ( IFormatModule )Activator.CreateInstance( type );
                TryRegister( instance );
            }
        }
    }
}
