using MikuMikuLibrary.Archives.Farc;
using System.IO;
using System.Text;

namespace MikuMikuModel.FormatModules
{
    public class FarcArchiveFormatModule : FormatModule<FarcArchive>
    {
        public override FormatModuleFlags Flags =>
            FormatModuleFlags.Import | FormatModuleFlags.Export;

        public override string Name => "FARC Archive";
        public override string[] Extensions => new[] { "farc" };

        public override FarcArchive Import( string filePath )
        {
            return FarcArchive.Load<FarcArchive>( filePath );
        }

        protected override bool CanImportCore( Stream source, string fileName )
        {
            byte[] signature = new byte[ 4 ];
            source.Read( signature, 0, 4 );

            switch ( Encoding.ASCII.GetString( signature ) )
            {
                case "FARC":
                    return true;
                case "FArC":
                    return true;
                case "FArc":
                    return true;
            }

            return false;
        }

        protected override FarcArchive ImportCore( Stream source, string fileName )
        {
            return FarcArchive.Load<FarcArchive>( source, true );
        }

        protected override void ExportCore( FarcArchive obj, Stream destination, string fileName )
        {
            obj.Save( destination, true );
        }
    }
}
