using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using System;
using System.IO;

namespace MikuMikuModel.FormatModules
{
    public class ObjectDatabaseFormatModule : FormatModule<ObjectDatabase>
    {
        public override FormatModuleFlags Flags
        {
            get { return FormatModuleFlags.Import | FormatModuleFlags.Export; }
        }

        public override string Name
        {
            get { return "Object Database"; }
        }

        public override string[] Extensions
        {
            get { return new[] { "bin", "osi" }; }
        }

        protected override bool CanImportCore( Stream source, string fileName )
        {
            if ( fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
                return fileName.StartsWith( "obj_db", StringComparison.OrdinalIgnoreCase );

            return fileName.EndsWith( ".osi", StringComparison.OrdinalIgnoreCase );
        }

        protected override void ExportCore( ObjectDatabase obj, Stream destination, string fileName )
        {
            obj.Save( destination, true );
        }

        protected override ObjectDatabase ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<ObjectDatabase>( source, true );
        }
    }
}
