using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Textures;

namespace MikuMikuModel.Modules.Textures
{
    public class TextureSetModule : FormatModule<TextureSet>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Texture Set (Classic)", "bin", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Texture Set (Modern)", "txd", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        public override bool Match( string fileName )
        {
            return fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase )
                ? fileName.EndsWith( "_tex.bin", StringComparison.OrdinalIgnoreCase ) // TODO: Should this check be done?
                : base.Match( fileName );
        }

        public override bool Match( byte[] buffer )
        {
            int signature = BitConverter.ToInt32( buffer, 0 );
            return signature == 0x03505854 || signature == 0x54585003 || signature == 0x4458544D;
        }

        protected override TextureSet ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<TextureSet>( source, true );
        }

        protected override void ExportCore( TextureSet model, Stream destination, string fileName )
        {
            model.Save( destination, true );
        }
    }
}