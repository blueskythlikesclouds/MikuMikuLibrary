using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.Processing;

namespace MikuMikuModel.Modules.Textures
{
    public class TextureModule : FormatModule<Texture>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Portable Network Graphics", "png", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "DirectDraw Surface", "dds", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Graphic Interchange Format", "gif", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "HDP Format", "hdp", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Icon", "ico", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Joint Photographic Group", "jpeg", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Joint Photographic Group", "jpg", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "JXR Format", "jxr", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Truevision TARGA", "tga", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Tagged Image File Format", "tif", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Tagged Image File Format", "tiff", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "WDP Format", "wdp", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        public override Texture Import( string filePath )
        {
            return TextureEncoder.EncodeFromFile( filePath, TextureFormat.Unknown, true );
        }

        public override void Export( Texture model, string filePath )
        {
            TextureDecoder.DecodeToFile( model, filePath );
        }

        protected override Texture ImportCore( Stream source, string fileName ) => throw new NotSupportedException();
        protected override void ExportCore( Texture model, Stream destination, string fileName ) => throw new NotSupportedException();
    }
}