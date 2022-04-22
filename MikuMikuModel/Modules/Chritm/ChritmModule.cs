using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Chritm;

namespace MikuMikuModel.Modules.Chritm
{
    public class ChritmSetModule : FormatModule<ChritmSet>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Chritm Table (Modern)", "itmt", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        public override bool Match(string fileName)
        {
            return fileName.EndsWith(".itmt", StringComparison.OrdinalIgnoreCase);
        }

        public override bool Match(byte[] buffer)
        {
            return buffer[0] == 'I' && buffer[1] == 'T' && buffer[2] == 'E' && buffer[3] == 'M';
        }

        public override ChritmSet Import(string filePath)
        {
            return BinaryFile.Load<ChritmSet>(filePath);
        }

        protected override ChritmSet ImportCore(Stream source, string fileName)
        {
            return BinaryFile.Load<ChritmSet>(source, true);
        }

        protected override void ExportCore(ChritmSet model, Stream destination, string fileName)
        {
            model.Save(destination, true);
        }
    }
}