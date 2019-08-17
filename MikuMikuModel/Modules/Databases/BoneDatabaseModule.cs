using System;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.Modules.Databases
{
    public class BoneDatabaseModule : FormatModule<BoneDatabase>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Bone Database";
        public override string[] Extensions => new[] { "bin", "bon" };

        public override bool Match( string fileName )
        {
            return base.Match( fileName ) && Path.GetFileNameWithoutExtension( fileName )
                       .Equals( "bone_data", StringComparison.OrdinalIgnoreCase );
        }

        protected override BoneDatabase ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<BoneDatabase>( source, true );
        }

        protected override void ExportCore( BoneDatabase model, Stream destination, string fileName )
        {
            model.Save( destination, true );
        }
    }
}