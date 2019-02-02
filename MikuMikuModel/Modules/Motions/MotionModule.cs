using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Motions;

namespace MikuMikuModel.Modules.Motions
{
    public class MotionModule : FormatModule<Motion>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Motion";
        public override string[] Extensions => new[] { "mot" };

        public override bool Match( byte[] buffer ) =>
            buffer[ 0 ] == 'M' && buffer[ 1 ] == 'O' && buffer[ 2 ] == 'T' && buffer[ 3 ] == 'C';

        protected override Motion ImportCore( Stream source, string fileName ) =>
            BinaryFile.Load<Motion>( source, true );

        protected override void ExportCore( Motion model, Stream destination, string fileName ) =>
            model.Save( destination, true );
    }
}