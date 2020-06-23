using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;

namespace MikuMikuModel.Modules.Databases
{
    public class TextureDatabaseModule : FormatModule<TextureDatabase>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Texture Database (Classic)", "bin", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Texture Database (Modern)", "txi", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        public override bool Match( string fileName )
        {
            if ( fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
            {
                if ( fileName.StartsWith( "mdata_", StringComparison.OrdinalIgnoreCase ) )
                    fileName = fileName.Remove( 0, 6 );

                return Path.GetFileNameWithoutExtension( fileName )
                    .Equals( "tex_db", StringComparison.OrdinalIgnoreCase );
            }

            return base.Match( fileName );
        }

        protected override TextureDatabase ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<TextureDatabase>( source, true );
        }

        protected override void ExportCore( TextureDatabase model, Stream destination, string fileName )
        {
            model.Save( destination, true );
        }
    }
}