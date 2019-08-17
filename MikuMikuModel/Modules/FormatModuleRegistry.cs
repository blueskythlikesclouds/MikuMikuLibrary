using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MikuMikuModel.Modules
{
    public static class FormatModuleRegistry
    {
        private static readonly Dictionary<Type, IFormatModule> sModules = new Dictionary<Type, IFormatModule>();

        public static IReadOnlyDictionary<Type, IFormatModule> ModulesByType => sModules;
        public static IEnumerable<Type> ModelTypes => sModules.Keys;
        public static IEnumerable<IFormatModule> Modules => sModules.Values;

        public static void Register( IFormatModule module )
        {
            if ( sModules.ContainsKey( module.ModelType ) )
                return;

            sModules[ module.ModelType ] = module;
        }

        static FormatModuleRegistry()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var types = assembly.GetTypes().Where(
                x => typeof( IFormatModule ).IsAssignableFrom( x ) && x.IsClass && !x.IsAbstract );

            foreach ( var type in types )
                Register( ( IFormatModule ) Activator.CreateInstance( type ) );
        }
    }
}