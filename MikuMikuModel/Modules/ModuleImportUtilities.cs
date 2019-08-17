using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MikuMikuModel.Modules
{
    public static class ModuleImportUtilities
    {
        public static IFormatModule GetModule( IEnumerable<IFormatModule> modulesToMatch, string fileName,
            Func<Stream> streamGetter )
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

            if ( moduleList.Count <= 1 )
                return moduleList.Count == 0 ? null : moduleList[ 0 ];

            var buffer = new byte[ 16 ];
            using ( var stream = streamGetter() )
            {
                stream.Read( buffer, 0, 16 );
            }

            moduleList.RemoveAll( x => !x.Match( buffer ) );
            return moduleList.Count != 1 ? null : moduleList[ 0 ];
        }

        public static IFormatModule GetModule( IEnumerable<Type> modelTypesToMatch, string fileName,
            Func<Stream> streamGetter )
        {
            return GetModule( modelTypesToMatch.Select( x => FormatModuleRegistry.ModulesByType[ x ] ), fileName,
                streamGetter );
        }

        public static IFormatModule GetModule( IEnumerable<IFormatModule> modulesToMatch, string filePath )
        {
            return GetModule( modulesToMatch, Path.GetFileName( filePath ), () => File.OpenRead( filePath ) );
        }

        public static IFormatModule GetModule( IEnumerable<Type> modelTypesToMatch, string filePath )
        {
            return GetModule( modelTypesToMatch, Path.GetFileName( filePath ), () => File.OpenRead( filePath ) );
        }

        public static IFormatModule GetModule( string fileName, Func<Stream> streamGetter )
        {
            return GetModule( FormatModuleRegistry.Modules, fileName, streamGetter );
        }

        public static IFormatModule GetModule( string filePath )
        {
            return GetModule( FormatModuleRegistry.Modules, filePath );
        }

        public static T ImportFile<T>( string filePath )
        {
            if ( !FormatModuleRegistry.ModulesByType.TryGetValue( typeof( T ), out var module ) )
                return default;

            return ( T ) module.Import( filePath );
        }

        public static string SelectModuleImport<T>( string title = "Select a file to import.", string filePath = null )
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = ModuleFilterGenerator.GenerateFilter( typeof( T ) );
                dialog.Title = title;
                dialog.FileName = filePath;
                dialog.ValidateNames = true;
                dialog.AddExtension = true;

                if ( dialog.ShowDialog() == DialogResult.OK )
                    return dialog.FileName;
            }

            return null;
        }

        public static string[] SelectModuleImportMultiselect<T>( string title = "Select file(s) to import.",
            string filePath = null )
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = ModuleFilterGenerator.GenerateFilter( typeof( T ) );
                dialog.Title = title;
                dialog.FileName = filePath;
                dialog.ValidateNames = true;
                dialog.AddExtension = true;
                dialog.Multiselect = true;

                if ( dialog.ShowDialog() == DialogResult.OK )
                    return dialog.FileNames;
            }

            return null;
        }

        public static string SelectModuleImport( IEnumerable<Type> modelTypes,
            string title = "Select a file to import from.", string filePath = null )
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = ModuleFilterGenerator.GenerateFilter( modelTypes, FormatModuleFlags.Import );
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