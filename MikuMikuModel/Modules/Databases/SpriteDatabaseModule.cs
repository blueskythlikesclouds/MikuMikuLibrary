using System;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.Modules.Databases
{
    public class SpriteDatabaseModule : FormatModule<SpriteDatabase>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Sprite Database";
        public override string[] Extensions => new[] { "bin", "spi" };

        public override bool Match( string fileName )
        {
            if ( fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
            {
                if ( fileName.StartsWith( "mdata_", StringComparison.OrdinalIgnoreCase ) )
                    fileName = fileName.Remove( 0, 6 );

                return Path.GetFileNameWithoutExtension( fileName )
                    .Equals( "spr_db", StringComparison.OrdinalIgnoreCase );
            }

            return base.Match( fileName );
        }

        protected override SpriteDatabase ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<SpriteDatabase>( source, true );
        }

        protected override void ExportCore( SpriteDatabase model, Stream destination, string fileName )
        {
            model.Save( destination, true );
        }
    }
}