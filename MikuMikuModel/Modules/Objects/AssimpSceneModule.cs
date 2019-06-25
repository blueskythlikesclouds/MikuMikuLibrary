﻿using System;
using System.IO;
using Assimp;
using MikuMikuLibrary.Objects.Processing.Assimp;

namespace MikuMikuModel.Modules.Objects
{
    public class AssimpSceneModule : FormatModule<Scene>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Assimp Scene";
        public override string[] Extensions => new[] { "dae", "fbx", "obj" };

        protected override Scene ImportCore( Stream source, string fileName ) =>
            source is FileStream fileStream
                ? SceneUtilities.Import( fileStream.Name )
                : throw new ArgumentException( "Assimp scene can only be imported from a file stream",
                    nameof( source ) );

        protected override void ExportCore( Scene model, Stream destination, string fileName )
        {
            if ( destination is FileStream fileStream )
                SceneUtilities.Export( model, fileStream.Name );
            else
                throw new ArgumentException( "Assimp scene can only be exported to file stream",
                    nameof( destination ) );
        }
    }
}