using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.Modules.Databases
{
    public class MotionDatabaseModule : FormatModule<MotionDatabase>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Motion Database (Classic)", "bin", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        public override bool Match( string fileName )
        {
            return base.Match( fileName ) && Path.GetFileNameWithoutExtension( fileName )
                .Equals( "mot_db", StringComparison.OrdinalIgnoreCase );
        }

        protected override MotionDatabase ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<MotionDatabase>( source, true );
        }

        protected override void ExportCore( MotionDatabase model, Stream destination, string fileName )
        {
            model.Save( destination, true );
        }
    }
}