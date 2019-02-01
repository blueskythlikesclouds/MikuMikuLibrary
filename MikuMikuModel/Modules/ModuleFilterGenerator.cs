using System;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuModel.Modules
{
    public static class ModuleFilterGenerator
    {
        public static string GenerateFilter( IFormatModule module ) =>
            $"{module.Name}|{string.Join( ";", module.Extensions.Select( x => $"*.{x}" ) )}";

        public static string GenerateFilter( Type modelType ) =>
            GenerateFilter( FormatModuleRegistry.ModulesByType[ modelType ] );

        public static string GenerateFilter( IEnumerable<IFormatModule> modules, FormatModuleFlags flags )
        {
            var moduleList = modules.Where( x => x.Flags.HasFlag( flags ) ).ToList();

            if ( moduleList.Count == 0 )
                return string.Empty;

            if ( moduleList.Count == 1 )
                return GenerateFilter( moduleList[ 0 ] );

            return string.Format( "All files|{0}|{1}",
                string.Join( ";", moduleList.SelectMany( x => x.Extensions ).Distinct().Select( x => $"*.{x}" ) ),
                string.Join( "|", moduleList.Select( GenerateFilter ) ) );
        }

        public static string GenerateFilter( IEnumerable<Type> modelTypes, FormatModuleFlags flags ) =>
            GenerateFilter( modelTypes.Select( x => FormatModuleRegistry.ModulesByType[ x ] ), flags );

        public static string GenerateFilter( FormatModuleFlags flags ) =>
            GenerateFilter( FormatModuleRegistry.Modules, flags );
    }
}