using System.IO;

namespace MikuMikuModel.Modules.IO
{
    public class StreamModule : FormatModule<Stream>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Stream";
        public override string[] Extensions => new[] { "*" };

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