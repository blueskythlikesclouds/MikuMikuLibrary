using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.Textures;

namespace MikuMikuModel.Modules.Textures
{
    public class TextureModule : FormatModule<Texture>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "DirectDraw Surface", "dds", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        protected override Texture ImportCore( Stream source, string fileName )
        {
            return TextureEncoder.Encode( source );
        }

        protected override void ExportCore( Texture model, Stream destination, string fileName )
        {
            TextureDecoder.DecodeToDDS( model, destination );
        }
    }
}