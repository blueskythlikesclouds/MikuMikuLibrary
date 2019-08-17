using System;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Textures;

namespace MikuMikuModel.Modules.Textures
{
    public class TextureSetModule : FormatModule<TextureSet>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Texture Set";
        public override string[] Extensions => new[] { "bin", "txd" };

        public override bool Match( string fileName )
        {
            return fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase )
                ? fileName.EndsWith( "_tex.bin",
                    StringComparison.OrdinalIgnoreCase ) // TODO: Should this check be done?
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