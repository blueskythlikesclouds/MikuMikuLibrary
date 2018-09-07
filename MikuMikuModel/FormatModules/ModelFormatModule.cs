using MikuMikuLibrary.Models;
using MikuMikuModel.Configurations;
using System.IO;

namespace MikuMikuModel.FormatModules
{
    public class ModelFormatModule : FormatModule<Model>
    {
        public override FormatModuleFlags Flags
        {
            get { return FormatModuleFlags.Import | FormatModuleFlags.Export; }
        }

        public override string Name
        {
            get { return "Model"; }
        }

        public override string[] Extensions
        {
            get { return new[] { "bin", "osd" }; }
        }

        protected override bool CanImportCore( Stream source, string fileName )
        {
            var sig = new byte[ 4 ];
            source.Read( sig, 0, 4 );

            return ( sig[ 0 ] == 'M' && sig[ 1 ] == 'O' && sig[ 2 ] == 'S' && sig[ 3 ] == 'D' ) ||
                ( sig[ 0 ] == 0 && sig[ 1 ] == 0x25 && sig[ 2 ] == 0x06 && sig[ 3 ] == 0x05 );
        }

        public override Model Import( string filePath )
        {
            var model = new Model();
            model.Load( filePath,
                ConfigurationList.Instance.CurrentConfiguration?.ObjectDatabase,
                ConfigurationList.Instance.CurrentConfiguration?.TextureDatabase );

            return model;
        }

        public override void Export( Model obj, string filePath )
        {
            obj.Save( filePath,
                ConfigurationList.Instance.CurrentConfiguration?.ObjectDatabase,
                ConfigurationList.Instance.CurrentConfiguration?.TextureDatabase,
                ConfigurationList.Instance.CurrentConfiguration?.BoneDatabase );
        }

        protected override Model ImportCore( Stream source, string fileName )
        {
            return Model.Load<Model>( source, true );
        }

        protected override void ExportCore( Model obj, Stream destination, string fileName )
        {
            obj.Save( destination,
                ConfigurationList.Instance.CurrentConfiguration?.ObjectDatabase,
                ConfigurationList.Instance.CurrentConfiguration?.TextureDatabase,
                ConfigurationList.Instance.CurrentConfiguration?.BoneDatabase, true );
        }
    }
}
