using System.Collections.Generic;
using System.IO;

namespace MikuMikuModel.Modules.IO
{
    public class StreamModule : FormatModule<Stream>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "File Stream", "*", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        public override Stream Import( string fileName )
        {
            return Import( File.OpenRead( fileName ), Path.GetFileName( fileName ) );
        }

        protected override Stream ImportCore( Stream source, string fileName )
        {
            return source;
        }

        protected override void ExportCore( Stream model, Stream destination, string fileName )
        {
            if ( model.CanSeek )
                model.Seek( 0, SeekOrigin.Begin );

            model.CopyTo( destination );
        }
    }
}