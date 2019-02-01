using System;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Models;
using MikuMikuModel.Configurations;

namespace MikuMikuModel.Modules.Models
{
    public class ModelModule : FormatModule<Model>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Model";
        public override string[] Extensions => new[] { "bin", "osd" };

        public override bool Match( string fileName ) =>
            fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase )
                ? fileName.EndsWith( "_obj.bin", StringComparison.OrdinalIgnoreCase ) // TODO: Should this check be done?
                : base.Match( fileName );

        public override bool Match( byte[] buffer ) =>
            ( buffer[ 0 ] == 'M' && buffer[ 1 ] == 'O' && buffer[ 2 ] == 'S' && buffer[ 3 ] == 'D' ) ||
            ( buffer[ 0 ] == 0 && buffer[ 1 ] == 0x25 && buffer[ 2 ] == 0x06 && buffer[ 3 ] == 0x05 );

        public override Model Import( string filePath )
        {
            var model = new Model();
            model.Load( filePath,
                ConfigurationList.Instance.CurrentConfiguration?.ObjectDatabase,
                ConfigurationList.Instance.CurrentConfiguration?.TextureDatabase );

            return model;
        }

        protected override Model ImportCore( Stream source, string fileName ) =>
            BinaryFile.Load<Model>( source, true );

        protected override void ExportCore( Model obj, Stream destination, string fileName )
        {
            obj.Save( destination,
                ConfigurationList.Instance.CurrentConfiguration?.ObjectDatabase,
                ConfigurationList.Instance.CurrentConfiguration?.TextureDatabase,
                ConfigurationList.Instance.CurrentConfiguration?.BoneDatabase, true );
        }
    }
}