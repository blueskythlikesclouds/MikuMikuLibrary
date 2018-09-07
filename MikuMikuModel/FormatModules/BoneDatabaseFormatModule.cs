using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using System;
using System.IO;

namespace MikuMikuModel.FormatModules
{
    public class BoneDatabaseFormatModule : FormatModule<BoneDatabase>
    {
        public override FormatModuleFlags Flags
        {
            get { return FormatModuleFlags.Import | FormatModuleFlags.Export; }
        }

        public override string Name
        {
            get { return "Bone Database"; }
        }

        public override string[] Extensions
        {
            get { return new[] { "bin", "bon" }; }
        }

        protected override bool CanImportCore( Stream source, string fileName )
        {
            if ( fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
                return fileName.StartsWith( "bone_data", StringComparison.OrdinalIgnoreCase );

            return fileName.EndsWith( ".bon", StringComparison.OrdinalIgnoreCase );
        }

        protected override void ExportCore( BoneDatabase obj, Stream destination, string fileName )
        {
            obj.Save( destination );
        }

        protected override BoneDatabase ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<BoneDatabase>( source );
        }
    }
}
