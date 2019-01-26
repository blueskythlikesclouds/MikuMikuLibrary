using System;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.FormatModules
{
    public class MotionDatabaseFormatModule : FormatModule<MotionDatabase>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Motion Database";
        public override string[] Extensions => new[] { "bin" };

        protected override bool CanImportCore( Stream source, string fileName ) =>
            fileName.StartsWith( "mot_db", StringComparison.OrdinalIgnoreCase );

        protected override MotionDatabase ImportCore( Stream source, string fileName ) =>
            BinaryFile.Load<MotionDatabase>( source, true );

        protected override void ExportCore( MotionDatabase obj, Stream destination, string fileName ) =>
            obj.Save( destination, true );
    }
}