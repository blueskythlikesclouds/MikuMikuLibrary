// Code by Thatrandomlurker
using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.PostProcessTables.FogTable;

namespace MikuMikuModel.Modules.PostProcessTables
{
    public class FogTableModule : FormatModule<FogTable>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Fog Table", "fog", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
        };

        public override bool Match( byte[] buffer )
        {
            return buffer[0] == 'F' && buffer[1] == 'O' && buffer[2] == 'G' && buffer[3] == 'C';
        }

        public override FogTable Import( string filePath )
        {
            return BinaryFile.Load<FogTable>( filePath );
        }

        protected override FogTable ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<FogTable>( source, true );
        }

        protected override void ExportCore( FogTable model, Stream destination, string fileName )
        {
            model.Save( destination, true );
        }

    }
}
