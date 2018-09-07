using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.DDS;
using System;
using System.IO;

namespace MikuMikuModel.FormatModules
{
    public class TextureFormatModule : FormatModule<Texture>
    {
        public override FormatModuleFlags Flags
        {
            get { return FormatModuleFlags.Import | FormatModuleFlags.Export; }
        }

        public override string Name
        {
            get { return "Texture"; }
        }

        public override string[] Extensions
        {
            get { return new[] { "dds" }; }
        }

        protected override bool CanImportCore( Stream source, string fileName )
        {
            var sig = new byte[ 4 ];
            source.Read( sig, 0, 4 );

            return BitConverter.ToInt32( sig, 0 ) == DDSHeader.Magic;
        }

        protected override void ExportCore( Texture obj, Stream destination, string fileName )
        {
            TextureDecoder.DecodeToDDS( obj, destination );
        }

        protected override Texture ImportCore( Stream source, string fileName )
        {
            return TextureEncoder.Encode( source );
        }
    }
}
