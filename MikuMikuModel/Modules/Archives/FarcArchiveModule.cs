using System.Collections.Generic;
using System.IO;
using System.Text;
using MikuMikuLibrary.Archives;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.Modules.Archives
{
    public class FarcArchiveModule : FormatModule<FarcArchive>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "FARC Archive", "farc", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        public override bool Match( byte[] buffer )
        {
            string signature = Encoding.UTF8.GetString( buffer, 0, 4 );
            return signature == "FArc" || signature == "FArC" || signature == "FARC";
        }

        public override FarcArchive Import( string filePath )
        {
            return BinaryFile.Load<FarcArchive>( filePath );
        }

        protected override FarcArchive ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<FarcArchive>( source, true );
        }

        protected override void ExportCore( FarcArchive model, Stream destination, string fileName )
        {
            model.Save( destination, true );
        }
    }
}