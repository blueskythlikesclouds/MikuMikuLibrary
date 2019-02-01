using System.IO;

namespace MikuMikuModel.Modules.IO
{
    public class StreamModule : FormatModule<Stream>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Stream";
        public override string[] Extensions => new[] { "*" };

        public override Stream Import( string fileName ) => 
            Import( File.OpenRead( fileName ), Path.GetFileName( fileName ) );

        protected override Stream ImportCore( Stream source, string fileName ) => source;

        protected override void ExportCore( Stream obj, Stream destination, string fileName )
        {
            if ( obj.CanSeek )
                obj.Seek( 0, SeekOrigin.Begin );

            obj.CopyTo( destination );
        }
    }
}