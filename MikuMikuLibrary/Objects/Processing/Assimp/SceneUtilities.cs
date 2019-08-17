using System;
using System.IO;
using System.Linq;
using Assimp.Configs;
using Ai = Assimp;

namespace MikuMikuLibrary.Objects.Processing.Assimp
{
    public static class SceneUtilities
    {
        public static Ai.Scene Import( string fileName )
        {
            var aiContext = new Ai.AssimpContext();
            aiContext.SetConfig( new MeshVertexLimitConfig( 32768 ) );
            aiContext.SetConfig( new VertexCacheSizeConfig( 63 ) );
            aiContext.SetConfig( new MaxBoneCountConfig( 48 ) );

            return aiContext.ImportFile( fileName,
                Ai.PostProcessSteps.CalculateTangentSpace | Ai.PostProcessSteps.Triangulate |
                Ai.PostProcessSteps.SplitLargeMeshes | Ai.PostProcessSteps.ImproveCacheLocality |
                Ai.PostProcessSteps.SortByPrimitiveType | Ai.PostProcessSteps.SplitByBoneCount );
        }

        public static void Export( Ai.Scene aiScene, string fileName,
            Ai.PostProcessSteps postProcessSteps = Ai.PostProcessSteps.None )
        {
            var aiContext = new Ai.AssimpContext();

            string formatExtension = Path.GetExtension( fileName ).Substring( 1 );
            string formatId = aiContext.GetSupportedExportFormats()
                .First( x => x.FileExtension.Equals( formatExtension, StringComparison.OrdinalIgnoreCase ) ).FormatId;

            aiContext.ExportFile( aiScene, fileName, formatId, postProcessSteps );
        }
    }
}