using System.IO;
using MikuMikuLibrary.Textures;

namespace MikuMikuModel.Modules.Textures
{
    public class TextureModule : FormatModule<Texture>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Texture";
        public override string[] Extensions => new[] { "dds" };

        protected override Texture ImportCore( Stream source, string fileName ) =>
            TextureEncoder.Encode( source );

        protected override void ExportCore( Texture model, Stream destination, string fileName ) =>
            TextureDecoder.DecodeToDDS( model, destination );
    }
}