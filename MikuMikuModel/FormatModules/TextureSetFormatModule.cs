using MikuMikuLibrary.Textures;
using System;
using System.IO;

namespace MikuMikuModel.FormatModules
{
    public class TextureSetFormatModule : FormatModule<TextureSet>
    {
        public override FormatModuleFlags Flags =>
            FormatModuleFlags.Import | FormatModuleFlags.Export;

        public override string Name => "Texture Set";
        public override string[] Extensions => new[] { "bin", "txd" };

        protected override bool CanImportCore( Stream source, string fileName )
        {
            var sig = new byte[ 4 ];
            source.Read( sig, 0, 4 );
            var sigInt = BitConverter.ToInt32( sig, 0 );
            return sigInt == 0x03505854 || sigInt == 0x54585003 || sigInt == 0x4458544D;
        }

        protected override void ExportCore( TextureSet obj, Stream destination, string fileName )
        {
            obj.Save( destination, true );
        }

        protected override TextureSet ImportCore( Stream source, string fileName )
        {
            return TextureSet.Load<TextureSet>( source, true );
        }
    }
}
