using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.Modules.Databases
{
    public class ObjectDatabaseModule : FormatModule<ObjectDatabase>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Object Database (Classic)", "bin", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Object Database (Modern)", "osi", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

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

        protected override ObjectDatabase ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<ObjectDatabase>( source, true );
        }

        protected override void ExportCore( ObjectDatabase model, Stream destination, string fileName )
        {
            model.Save( destination, true );
        }
    }
}