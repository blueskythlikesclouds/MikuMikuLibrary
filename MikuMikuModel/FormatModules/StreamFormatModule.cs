using System.IO;

namespace MikuMikuModel.FormatModules
{
    public class StreamFormatModule : FormatModule<Stream>
    {
        public override FormatModuleFlags Flags => 
            FormatModuleFlags.Import | FormatModuleFlags.Export;

        public override string Name => "Stream";
        public override string[] Extensions => new[] { "*" };

        // Override the importer so it doesn't close the file stream
        public override Stream Import( string fileName )
        {
            return Import( File.OpenRead( fileName ), Path.GetFileName( fileName ) );
        }

        protected override bool CanImportCore( Stream source, string fileName )
        {
            // We can import literally anything
            return true;
        }

        protected override void ExportCore( Stream obj, Stream destination, string fileName )
        {
            // Just copy the stream
            obj.Seek( 0, SeekOrigin.Begin );
            obj.CopyTo( destination );
        }

        protected override Stream ImportCore( Stream source, string fileName )
        {
            // Just return the stream
            return source;
        }
    }
}
