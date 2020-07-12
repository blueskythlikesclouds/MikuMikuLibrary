using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Objects.Processing.Fbx;
using MikuMikuModel.Configurations;

namespace MikuMikuModel.Modules.Objects
{
    public class ObjectSetModule : FormatModule<ObjectSet>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Object Set (Classic)", "bin", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "Object Set (Modern)", "osd", FormatExtensionFlags.Import | FormatExtensionFlags.Export ),
            new FormatExtension( "FBX Exporter", "fbx", FormatExtensionFlags.Export )
        };

        public override bool Match( string fileName )
        {
            return fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase )
                ? fileName.EndsWith( "_obj.bin", StringComparison.OrdinalIgnoreCase ) // TODO: Should this check be done?
                : base.Match( fileName );
        }

        public override bool Match( byte[] buffer )
        {
            return buffer[ 0 ] == 'M' && buffer[ 1 ] == 'O' && buffer[ 2 ] == 'S' && buffer[ 3 ] == 'D' ||
                   buffer[ 0 ] == 0 && buffer[ 1 ] == 0x25 && buffer[ 2 ] == 0x06 && buffer[ 3 ] == 0x05;
        }

        public override ObjectSet Import( string filePath )
        {
            var model = new ObjectSet();
            model.Load( filePath,
                ConfigurationList.Instance.CurrentConfiguration?.ObjectDatabase,
                ConfigurationList.Instance.CurrentConfiguration?.TextureDatabase );

            return model;
        }

        public override void Export( ObjectSet model, string filePath )
        {
            if ( filePath.EndsWith( ".fbx", StringComparison.OrdinalIgnoreCase ) )
                FbxExporter.ExportToFile( model, filePath );

            else
                base.Export( model, filePath );
        }

        protected override ObjectSet ImportCore( Stream source, string fileName )
        {
            return BinaryFile.Load<ObjectSet>( source, true );
        }

        protected override void ExportCore( ObjectSet model, Stream destination, string fileName )
        {
            model.Save( destination,
                ConfigurationList.Instance.CurrentConfiguration?.ObjectDatabase,
                ConfigurationList.Instance.CurrentConfiguration?.TextureDatabase,
                ConfigurationList.Instance.CurrentConfiguration?.BoneDatabase, true );
        }
    }
}