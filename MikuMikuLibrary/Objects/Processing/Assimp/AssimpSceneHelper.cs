using System;
using System.IO;
using System.Linq;
using Assimp.Configs;
using Ai = Assimp;

namespace MikuMikuLibrary.Objects.Processing.Assimp
{
    public static class AssimpSceneHelper
    {
        public static Ai.Scene Import( string filePath )
        {
            var aiContext = new Ai.AssimpContext();

            aiContext.SetConfig( new FBXPreservePivotsConfig( false ) );
            aiContext.SetConfig( new MaxBoneCountConfig( 64 ) );
            aiContext.SetConfig( new MeshTriangleLimitConfig( 524288 ) );
            aiContext.SetConfig( new MeshVertexLimitConfig( 32768 ) );
            aiContext.SetConfig( new VertexBoneWeightLimitConfig( 4 ) );
            aiContext.SetConfig( new VertexCacheSizeConfig( 63 ) );

            return aiContext.ImportFile( filePath,
                Ai.PostProcessSteps.JoinIdenticalVertices | Ai.PostProcessSteps.Triangulate | 
                Ai.PostProcessSteps.SplitLargeMeshes | Ai.PostProcessSteps.LimitBoneWeights |
                Ai.PostProcessSteps.ImproveCacheLocality | Ai.PostProcessSteps.SortByPrimitiveType |
                Ai.PostProcessSteps.SplitByBoneCount | Ai.PostProcessSteps.FlipUVs );
        }

        public static void Export( Ai.Scene aiScene, string filePath,
            Ai.PostProcessSteps postProcessSteps = Ai.PostProcessSteps.None )
        {
            var aiContext = new Ai.AssimpContext();

            string formatExtension = Path.GetExtension( filePath ).Substring( 1 );

            string formatId = aiContext.GetSupportedExportFormats()
                .First( x => x.FileExtension.Equals( formatExtension, StringComparison.OrdinalIgnoreCase ) ).FormatId;

            aiContext.ExportFile( aiScene, filePath, formatId, postProcessSteps );
        }
    }
}