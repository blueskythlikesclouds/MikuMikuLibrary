using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Aets;

namespace MikuMikuModel.Modules.Aets
{
    public class AetSetModule : FormatModule<AetSet>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Aet Set (Classic)", "bin", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Aet Set (Modern)", "aec", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        public override bool Match( string fileName )
        {
            return fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase )
                ? fileName.StartsWith( "aet_", StringComparison.OrdinalIgnoreCase ) // TODO: Should this check be done?
                : base.Match( fileName );
        }

        public override bool Match( byte[] buffer )
        {
            return buffer[ 0 ] == 'A' && buffer[ 1 ] == 'E' && buffer[ 2 ] == 'T' && buffer[ 3 ] == 'C';
        }

        public override AetSet Import( string filePath )
        {
            return BinaryFile.Load<AetSet>( filePath );
        }

        protected override AetSet ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<AetSet>( source, true );
        }

        protected override void ExportCore( AetSet model, Stream destination, string fileName )
        {
            model.Save( destination, true );
        }
    }
}