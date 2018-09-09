using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MikuMikuModel.FormatModules
{
    public static class FormatModuleUtilities
    {
        public static string GetFilter( IFormatModule module )
        {
            return $"{module.Name} Files|{string.Join( ";", module.Extensions.Select( x => $"*.{x}" ) )}";
        }

        public static string GetFilter( Type type )
        {
            if ( !FormatModuleRegistry.ModulesByType.TryGetValue( type, out IFormatModule module ) )
                throw new ArgumentException( "Couldn't find module with type", nameof( type ) );

            return GetFilter( module );
        }

        public static string GetFilter( IEnumerable<IFormatModule> modules, FormatModuleFlags flags )
        {
            if ( modules.Count() == 1 )
                return GetFilter( modules.First() );

            var allSupportedFiles = $"All Files|{string.Join( ";", modules.SelectMany( x => x.Extensions ).Distinct().Select( x => $"*.{x}" ) )}";
            return $"{allSupportedFiles}|{string.Join( "|", modules.Where( x => x.Flags.HasFlag( flags ) ).Select( x => GetFilter( x ) ) )}";
        }

        public static string GetFilter( IEnumerable<Type> types, FormatModuleFlags flags )
        {
            return GetFilter( EnumerateModulesFromTypes( types, flags ), flags );
        }

        public static bool HasAnyExtension( string fileName, IFormatModule module )
        {
            string extension = Path.GetExtension( fileName ).Trim( '.' );
            return module.Extensions.Any( x => x == "*" || x.Equals( extension, StringComparison.OrdinalIgnoreCase ) );
        }

        public static IEnumerable<IFormatModule> EnumerateModulesFromTypes( IEnumerable<Type> types, FormatModuleFlags flags )
        {
            foreach ( var type in types )
            {
                if ( FormatModuleRegistry.ModulesByType.TryGetValue( type, out IFormatModule module ) && module.Flags.HasFlag( flags ) )
                    yield return module;
            }
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
            return GetModuleForImport( source, EnumerateModulesFromTypes( typesToCheck, FormatModuleFlags.Import ), fileName );
        }

        public static IFormatModule GetModuleForImport( string filePath, IEnumerable<Type> typesToCheck )
        {
            return GetModuleForImport( filePath, EnumerateModulesFromTypes( typesToCheck, FormatModuleFlags.Import ) );
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
            return GetModuleForExport( fileName, EnumerateModulesFromTypes( types, FormatModuleFlags.Export ) );
        }

        public static IFormatModule GetModuleForExport( string fileName )
        {
            return GetModuleForExport( fileName, FormatModuleRegistry.Modules );
        }

        public static Stream ExportToStream( string fileName, object obj )
        {
            if ( obj is Stream stream )
                return stream;

            var type = obj.GetType();
            if ( !FormatModuleRegistry.ModulesByType.TryGetValue( type, out IFormatModule module ) )
                throw new ArgumentException( "Could not find suitable format module for object", nameof( obj ) );

            // Should I do Export flag check? nah format module will throw the exception anyways

            var memoryStream = new MemoryStream();
            module.Export( obj, memoryStream, fileName );
            return memoryStream;
        }

        public static string SelectModuleImport<T>( string title = "Select a file to import from.", string filePath = null )
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = GetFilter( typeof( T ) );
                dialog.Title = title;
                dialog.FileName = filePath;
                dialog.ValidateNames = true;
                dialog.AddExtension = true;

                if ( dialog.ShowDialog() == DialogResult.OK )
                    return dialog.FileName;
            }

            return null;
        }

        public static string SelectModuleExport<T>( string title = "Select a file to export to.", string filePath = null )
        {
            using ( var dialog = new SaveFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.Filter = GetFilter( typeof( T ) );
                dialog.OverwritePrompt = true;
                dialog.Title = title;
                dialog.FileName = filePath;
                dialog.ValidateNames = true;
                dialog.AddExtension = true;

                if ( dialog.ShowDialog() == DialogResult.OK )
                    return dialog.FileName;
            }

            return null;
        }
    }
}
