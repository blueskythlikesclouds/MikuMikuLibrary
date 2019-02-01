using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MikuMikuModel.Modules
{
    public static class ModuleExportUtilities
    {
        public static IFormatModule GetModule( IEnumerable<IFormatModule> modulesToMatch, string fileName )
        {
            fileName = Path.GetFileName( fileName );
            string extension = Path.GetExtension( fileName ).Trim( '.' );

            var moduleList = modulesToMatch
                .Where( x =>
                    x.Flags.HasFlag( FormatModuleFlags.Import ) &&
                    ( x.Extensions.Contains( "*" ) ||
                      x.Extensions.Contains( extension, StringComparer.OrdinalIgnoreCase ) ) )
                .ToList();

            if ( moduleList.Count > 1 )
                moduleList.RemoveAll( x => x.Extensions.Contains( "*" ) || !x.Match( fileName ) );

            return moduleList.Count != 1 ? null : moduleList[ 0 ];
        }

        public static IFormatModule GetModule( IEnumerable<Type> modelTypesToMatch, string fileName ) =>
            GetModule( modelTypesToMatch.Select( x => FormatModuleRegistry.ModulesByType[ x ] ), fileName );

    }
}