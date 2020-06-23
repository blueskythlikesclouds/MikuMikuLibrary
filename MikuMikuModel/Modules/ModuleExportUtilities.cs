using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MikuMikuModel.Modules
{
    public static class ModuleExportUtilities
    {
        public static IFormatModule GetModule( IEnumerable<IFormatModule> modulesToMatch, string fileName )
        {
            fileName = Path.GetFileName( fileName );
            string extension = Path.GetExtension( fileName ).Trim( '.' );

            var moduleList = modulesToMatch.Where( x =>
                x.Extensions.Where( y => y.Flags.HasFlag( FormatExtensionFlags.Export ) )
                    .Any( y => y.Extension == "*" || y.Extension.Equals( extension, StringComparison.OrdinalIgnoreCase ) ) ).ToList();

            if ( moduleList.Count > 1 )
                moduleList.RemoveAll(
                    x => x.Extensions.Any( y => y.Flags.HasFlag( FormatExtensionFlags.Export ) && y.Extension == "*" ) || !x.Match( fileName ) );

            return moduleList.Count != 1 ? null : moduleList[ 0 ];
        }

        public static IFormatModule GetModule( IEnumerable<Type> modelTypesToMatch, string fileName )
        {
            return GetModule( modelTypesToMatch.Select( x => FormatModuleRegistry.ModulesByType[ x ] ), fileName );
        }

        public static string SelectModuleExport<T>( string title = "Select a file to export to.", string filePath = null ) =>
            SelectModuleExport( new[] { typeof( T ) }, title, filePath );

        public static string SelectModuleExport( IEnumerable<Type> modelTypes,
            string title = "Select a file to export to.", string filePath = null )
        {
            using ( var dialog = new SaveFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = ModuleFilterGenerator.GenerateFilter( modelTypes, FormatExtensionFlags.Export );
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