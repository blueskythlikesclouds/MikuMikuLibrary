using Assimp;
using MikuMikuLibrary.Models.Processing.Assimp;
using System;
using System.IO;

namespace MikuMikuModel.FormatModules
{
    public class AssimpSceneFormatModule : FormatModule<Scene>
    {
        public override FormatModuleFlags Flags =>
            FormatModuleFlags.Import | FormatModuleFlags.Export;

        public override string Name => "Assimp Model";
        public override string[] Extensions => new[] { "dae", "fbx", "obj" };

        protected override bool CanImportCore( Stream source, string fileName )
        {
            return source is FileStream;
        }

        protected override void ExportCore( Scene obj, Stream destination, string fileName )
        {
            if ( destination is FileStream fileStream )
                SceneUtilities.Export( obj, fileStream.Name );
            else
                throw new ArgumentException( "AssimpSceneFormatModule can only export to FileStream", nameof( destination ) );
        }

        protected override Scene ImportCore( Stream source, string fileName )
        {
            if ( source is FileStream fileStream )
                return SceneUtilities.Import( fileStream.Name );
            else
                throw new ArgumentException( "AssimpSceneFormatModule can only import from FileStream", nameof( source ) );
        }
    }
}
