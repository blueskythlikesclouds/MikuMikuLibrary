using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Objects.Extra.Parameters;

namespace MikuMikuModel.Modules.Objects.Extra.Parameters
{
    public class OsageSkinParameterSetModule : FormatModule<OsageSkinParameterSet>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Osage Skin Parameter Set (Classic)", "txt", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Osage Skin Parameter Set (Modern)", "osp", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        public override bool Match( string fileName )
        {
            return fileName.EndsWith( ".txt", StringComparison.OrdinalIgnoreCase ) ? 
                fileName.StartsWith( "ext_skp_", StringComparison.OrdinalIgnoreCase )
                : base.Match( fileName );
        }

        protected override OsageSkinParameterSet ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<OsageSkinParameterSet>( source, true );
        }

        protected override void ExportCore( OsageSkinParameterSet model, Stream destination, string fileName )
        {
            model.Save( destination, true );
        }
    }
}