// Code by Thatrandomlurker
using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.PostProcessTables.BloomTable;

namespace MikuMikuModel.Modules.PostProcessTables
{
    public class BloomTableModule : FormatModule<BloomTable>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Bloom Table", "blt", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
        };

        public override bool Match( byte[] buffer )
        {
            return buffer[0] == 'B' && buffer[1] == 'L' && buffer[2] == 'M' && buffer[3] == 'T';
        }

        public override BloomTable Import( string filePath )
        {
            return BinaryFile.Load<BloomTable>( filePath );
        }

        protected override BloomTable ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<BloomTable>( source, true );
        }

        protected override void ExportCore( BloomTable model, Stream destination, string fileName )
        {
            model.Save( destination, true );
        }

    }
}
