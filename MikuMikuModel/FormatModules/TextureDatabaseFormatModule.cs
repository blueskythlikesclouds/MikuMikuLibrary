using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using System;
using System.IO;

namespace MikuMikuModel.FormatModules
{
    public class TextureDatabaseFormatModule : FormatModule<TextureDatabase>
    {
        public override FormatModuleFlags Flags
        {
            get { return FormatModuleFlags.Import | FormatModuleFlags.Export; }
        }

        public override string Name
        {
            get { return "Texture Database"; }
        }

        public override string[] Extensions
        {
            get { return new[] { "bin", "txi" }; }
        }

        protected override bool CanImportCore( Stream source, string fileName )
        {
            if ( fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
                return fileName.StartsWith( "tex_db", StringComparison.OrdinalIgnoreCase );

            return fileName.EndsWith( ".txi", StringComparison.OrdinalIgnoreCase );
        }

        protected override void ExportCore( TextureDatabase obj, Stream destination, string fileName )
        {
            obj.Save( destination, true );
        }

        protected override TextureDatabase ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<TextureDatabase>( source, true );
        }
    }
}
