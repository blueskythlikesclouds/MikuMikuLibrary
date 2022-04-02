// Code by Thatrandomlurker
using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.PostProcessTables.DepthOfFieldTable;

namespace MikuMikuModel.Modules.PostProcessTables
{
    public class DOFTableModule : FormatModule<DOFTable>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Depth of Field Table", "dft", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
        };

        public override bool Match( byte[] buffer )
        {
            return buffer[0] == 'D' && buffer[1] == 'O' && buffer[2] == 'F' && buffer[3] == 'T';
        }

        public override DOFTable Import( string filePath )
        {
            return BinaryFile.Load<DOFTable>( filePath );
        }

        protected override DOFTable ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<DOFTable>( source, true );
        }

        protected override void ExportCore( DOFTable model, Stream destination, string fileName )
        {
            model.Save( destination, true );
        }

    }
}
