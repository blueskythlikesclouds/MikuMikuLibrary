using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assimp;
using MikuMikuLibrary.Objects.Processing.Assimp;

namespace MikuMikuModel.Modules.Objects
{
    public class AssimpSceneModule : FormatModule<Scene>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; }

        protected override Scene ImportCore( Stream source, string fileName )
        {
            return source is FileStream fileStream
                ? AssimpSceneHelper.Import( fileStream.Name )
                : throw new ArgumentException( "Assimp scene can only be imported from a file stream", nameof( source ) );
        }

        protected override void ExportCore( Scene model, Stream destination, string fileName )
        {
            if ( destination is FileStream fileStream )
                AssimpSceneHelper.Export( model, fileStream.Name );
            else
                throw new ArgumentException( "Assimp scene can only be exported to file stream", nameof( destination ) );
        }

        public override bool Match( byte[] buffer )
        {
            // If extension did not match in the first place, then the header won't match either.
            return false;
        }

        public AssimpSceneModule()
        {
            var aiContext = new AssimpContext();

            var descriptions = aiContext.GetImporterDescriptions();
            var extensions = new List<FormatExtension>( descriptions.Length * 2 );

            extensions.Add( new FormatExtension( "Collada Exporter", "dae", FormatExtensionFlags.Export ) );

            foreach ( var description in aiContext.GetImporterDescriptions() )
                extensions.AddRange( description.FileExtensions.Select( extension =>
                    new FormatExtension( description.Name, extension, FormatExtensionFlags.Import ) ) );

            extensions.TrimExcess();

            Extensions = extensions;
        }
    }
}