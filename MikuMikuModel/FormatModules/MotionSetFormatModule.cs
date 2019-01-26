using System;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Motions;

namespace MikuMikuModel.FormatModules
{
    public class MotionSetFormatModule : FormatModule<MotionSet>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;

        public override string Name => "Motion Set";
        public override string[] Extensions => new[] { "bin" };

        protected override bool CanImportCore( Stream source, string fileName ) =>
            fileName.StartsWith( "mot_", StringComparison.OrdinalIgnoreCase );

        protected override MotionSet ImportCore( Stream source, string fileName ) =>
            BinaryFile.Load<MotionSet>( source, true );

        protected override void ExportCore( MotionSet obj, Stream destination, string fileName ) =>
            obj.Save( destination, true );
    }
}