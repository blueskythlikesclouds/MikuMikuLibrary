using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace MikuMikuModel.FormatModules
{
    public static class FormatModuleUtilities
    {
        public static string GetFilter( IFormatModule module )
        {
            return string.Join( "|", module.Extensions.Select( x => $"{x.ToUpperInvariant()} {module.Name} Files|*.{x}" ) );
        }

        public static string GetFilter( IEnumerable<IFormatModule> modules )
        {
            if ( modules.Count() == 1 )
                return GetFilter( modules.First() );

            var allSupportedFiles = $"All Files|{string.Join( ";", modules.SelectMany( x => x.Extensions ).Distinct().Select( x => $"*.{x}" ) )}";
            return $"{allSupportedFiles}|{string.Join( "|", modules.Select( x => GetFilter( x ) ) )}";
        }

        public static string GetFilter( Type type )
        {
            return string.Join( "|", FormatModuleRegistry.EnumerateModulesWithType( type ).Select( x => GetFilter( x ) ) );
        }

        public static string GetFilter( IEnumerable<Type> types )
        {
            return GetFilter( GetModulesFromTypes( types ) );
        }

        public static bool HasAnyExtension( string fileName, IFormatModule module )
        {
            string extension = Path.GetExtension( fileName ).Trim( '.' );
            return module.Extensions.Any( x => x == "*" || x.Equals( extension, StringComparison.OrdinalIgnoreCase ) );
        }

        public static IEnumerable<IFormatModule> GetModulesFromTypes( IEnumerable<Type> types )
        {
            return types.SelectMany( x => FormatModuleRegistry.EnumerateModulesWithType( x ) );
        }

        public static IFormatModule GetModuleForImport( Stream source, IEnumerable<IFormatModule> modulesToCheck, string fileName = null )
        {
            // Get modules which can import this file
            var position = source.Position;
            var modules = modulesToCheck.Where( x =>
            {
                bool ret = x.CanImport( source, fileName );
                source.Seek( position, SeekOrigin.Begin );
                return ret;
            } ).ToList();

            if ( modules.Count > 1 )
                modules.RemoveAll( x => x.Extensions.Contains( "*" ) );

            return modules.Count == 1 ? modules[ 0 ] : null;
        }

        public static IFormatModule GetModuleForImport( string filePath, IEnumerable<IFormatModule> modulesToCheck )
        {
            using ( var stream = File.OpenRead( filePath ) )
                return GetModuleForImport( stream, modulesToCheck, Path.GetFileName( filePath ) );
        }

        public static IFormatModule GetModuleForImport( Stream source, IEnumerable<Type> typesToCheck, string fileName = null )
        {
            return GetModuleForImport( source, GetModulesFromTypes( typesToCheck ), fileName );
        }

        public static IFormatModule GetModuleForImport( string filePath, IEnumerable<Type> typesToCheck )
        {
            return GetModuleForImport( filePath, GetModulesFromTypes( typesToCheck ) );
        }

        public static IFormatModule GetModuleForImport( Stream source, string fileName = null )
        {
            return GetModuleForImport( source, FormatModuleRegistry.Modules, fileName );
        }

        public static IFormatModule GetModuleForImport( string filePath )
        {
            return GetModuleForImport( filePath, FormatModuleRegistry.Modules );
        }

        public static IFormatModule GetModuleForExport( string fileName, IEnumerable<IFormatModule> modules )
        {
            var extension = Path.GetExtension( fileName ).Trim( '.' );
            var modulesMatch = modules.Where( x => x.Flags.HasFlag( FormatModuleFlags.Export ) && x.Extensions.Contains( "*" ) || x.Extensions.Contains( extension, StringComparer.OrdinalIgnoreCase ) ).ToList();

            if ( modulesMatch.Count > 1 )
                modulesMatch.RemoveAll( x => x.Extensions.Contains( "*" ) );

            return modulesMatch.SingleOrDefault();
        }

        public static IFormatModule GetModuleForExport( string fileName, IEnumerable<Type> types )
        {
            return GetModuleForExport( fileName, GetModulesFromTypes( types ) );
        }

        public static IFormatModule GetModuleForExport( string fileName )
        {
            return GetModuleForExport( fileName, FormatModuleRegistry.Modules );
        }
    }
}
