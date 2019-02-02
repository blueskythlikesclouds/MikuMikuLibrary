using System.IO;
using System.Text;
using MikuMikuLibrary.Archives.Farc;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.Modules.Archives
{
    public class FarcArchiveModule : FormatModule<FarcArchive>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "FARC Archive";
        public override string[] Extensions => new[] { "farc" };

        public override bool Match( byte[] buffer )
        {
            string signature = Encoding.UTF8.GetString( buffer, 0, 4 );
            return signature == "FArc" || signature == "FArC" || signature == "FARC";
        }

        public override FarcArchive Import( string filePath ) => 
            BinaryFile.Load<FarcArchive>( filePath );

        protected override FarcArchive ImportCore( Stream source, string fileName ) =>
            BinaryFile.Load<FarcArchive>( source, true );

        protected override void ExportCore( FarcArchive model, Stream destination, string fileName ) =>
            model.Save( destination, true );
    }
}