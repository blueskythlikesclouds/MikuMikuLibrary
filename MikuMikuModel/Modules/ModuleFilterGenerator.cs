using System;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuModel.Modules
{
    public static class ModuleFilterGenerator
    {
        public static string GenerateFilter( IFormatModule module, FormatExtensionFlags flags )
        {
            return string.Join( "|", module.Extensions.Where( x => x.Flags.HasFlag( flags ) ).Select( x => $"{x.Name}|*.{x.Extension}" ) );
        }

        public static string GenerateFilter( Type modelType, FormatExtensionFlags flags )
        {
            return GenerateFilter( FormatModuleRegistry.ModulesByType[ modelType ], flags );
        }

        public static string GenerateFilter( IEnumerable<IFormatModule> modules, FormatExtensionFlags flags )
        {
            var moduleList = modules.Where( x => x.Extensions.Any( y => y.Flags.HasFlag( flags ) ) ).ToList();

            if ( moduleList.Count == 0 )
                return string.Empty;

            if ( moduleList.Count == 1 && moduleList[ 0 ].Extensions.Count == 1 )
                return GenerateFilter( moduleList[ 0 ], flags );

            return string.Format( "All files|{0}|{1}",

                string.Join( ";", moduleList.SelectMany( x => x.Extensions )
                    .Where( x => x.Flags.HasFlag( flags ) ).Select( x => x.Extension ).Distinct().Select( x => $"*.{x}" ) ),

                string.Join( "|", moduleList.Select( x => GenerateFilter( x, flags ) ) ) );
        }

        public static string GenerateFilter( IEnumerable<Type> modelTypes, FormatExtensionFlags flags )
        {
            return GenerateFilter( modelTypes.Select( x => FormatModuleRegistry.ModulesByType[ x ] ), flags );
        }
    }
}