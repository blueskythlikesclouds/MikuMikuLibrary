using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.Modules.Databases
{
    public class BoneDatabaseModule : FormatModule<BoneDatabase>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Bone Database (Classic)", "bin", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Bone Database (Modern)", "bon", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

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