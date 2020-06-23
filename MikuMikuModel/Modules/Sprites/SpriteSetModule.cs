using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Sprites;

namespace MikuMikuModel.Modules.Sprites
{
    public class SpriteSetModule : FormatModule<SpriteSet>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Sprite Set (Classic)", "bin", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Sprite Set (Modern)", "spr", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        public override bool Match( string fileName )
        {
            return fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase )
                ? fileName.StartsWith( "spr_", StringComparison.OrdinalIgnoreCase ) // TODO: Should this check be done?
                : base.Match( fileName );
        }

        public override bool Match( byte[] buffer )
        {
            return buffer[ 0 ] == 'S' && buffer[ 1 ] == 'P' && buffer[ 2 ] == 'R' && buffer[ 3 ] == 'C' ||
                   buffer[ 0 ] == 0 && buffer[ 1 ] == 0 && buffer[ 2 ] == 0 && buffer[ 3 ] == 0;
        }

        public override SpriteSet Import( string filePath )
        {
            return BinaryFile.Load<SpriteSet>( filePath );
        }

        protected override SpriteSet ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<SpriteSet>( source, true );
        }

        protected override void ExportCore( SpriteSet model, Stream destination, string fileName )
        {
            model.Save( destination, true );
        }
    }
}