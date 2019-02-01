using System;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.Modules.Databases
{
    public class ObjectDatabaseModule : FormatModule<ObjectDatabase>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Object Database";
        public override string[] Extensions => new[] { "bin", "osi" };

        public override bool Match( string fileName )
        {
            if ( fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
            {
                if ( fileName.StartsWith( "mdata_", StringComparison.OrdinalIgnoreCase ) )
                    fileName = fileName.Remove( 0, 6 );

                return Path.GetFileNameWithoutExtension( fileName )
                    .Equals( "obj_db", StringComparison.OrdinalIgnoreCase );
            }

            return base.Match( fileName );
        }

        protected override ObjectDatabase ImportCore( Stream source, string fileName ) =>
            BinaryFile.Load<ObjectDatabase>( source, true );

        protected override void ExportCore( ObjectDatabase obj, Stream destination, string fileName ) =>
            obj.Save( destination, true );
    }
}