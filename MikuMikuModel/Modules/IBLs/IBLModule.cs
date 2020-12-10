using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IBLs;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.Modules.IBLs
{
    public class IBLModule : FormatModule<IBL>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "IBL", "ibl", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        protected override IBL ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<IBL>( source );
        }

        protected override void ExportCore( IBL model, Stream destination, string fileName )
        {
            model.Save( destination );
        }
    }
}