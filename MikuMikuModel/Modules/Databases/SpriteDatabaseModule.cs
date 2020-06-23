using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.Modules.Databases
{
    public class SpriteDatabaseModule : FormatModule<SpriteDatabase>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Sprite Database (Classic)", "bin", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Sprite Database (Modern)", "spi", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

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