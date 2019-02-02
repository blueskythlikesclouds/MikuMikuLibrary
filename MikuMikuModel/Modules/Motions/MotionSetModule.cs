using System;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Motions;

namespace MikuMikuModel.Modules.Motions
{
    public class MotionSetModule : FormatModule<MotionSet>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Motion Set";
        public override string[] Extensions => new[] { "bin" };

        public override bool Match( string fileName ) =>
            Path.GetFileNameWithoutExtension( fileName ).StartsWith( "mot_", StringComparison.OrdinalIgnoreCase );

        protected override MotionSet ImportCore( Stream source, string fileName ) =>
            BinaryFile.Load<MotionSet>( source, true );

        protected override void ExportCore( MotionSet model, Stream destination, string fileName ) =>
            model.Save( destination, true );
    }
}