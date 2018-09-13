using System;
using System.IO;
using System.Linq;
using Ai = Assimp;

namespace MikuMikuLibrary.Models
{
    public static class SceneUtilities
    {
        public static Ai.Scene Import( string fileName )
        {
            var aiContext = new Ai.AssimpContext();
            aiContext.SetConfig( new Ai.Configs.MeshVertexLimitConfig( 32768 ) );
            aiContext.SetConfig( new Ai.Configs.MaxBoneCountConfig( 64 ) );

            return aiContext.ImportFile( fileName, Ai.PostProcessSteps.JoinIdenticalVertices | Ai.PostProcessSteps.Triangulate | Ai.PostProcessSteps.GenerateSmoothNormals | 
                                                   Ai.PostProcessSteps.SplitLargeMeshes | Ai.PostProcessSteps.LimitBoneWeights | Ai.PostProcessSteps.ValidateDataStructure | 
                                                   Ai.PostProcessSteps.ImproveCacheLocality | Ai.PostProcessSteps.SortByPrimitiveType |  Ai.PostProcessSteps.FindInvalidData | 
                                                   Ai.PostProcessSteps.OptimizeMeshes | Ai.PostProcessSteps.SplitByBoneCount | Ai.PostProcessSteps.Debone | Ai.PostProcessSteps.RemoveRedundantMaterials );
        }

        public static void Export( Ai.Scene aiScene, string fileName, Ai.PostProcessSteps postProcessSteps = Ai.PostProcessSteps.None )
        {
            var aiContext = new Ai.AssimpContext();

            var formatExtension = Path.GetExtension( fileName ).Substring( 1 );
            var formatId = aiContext.GetSupportedExportFormats()
                .First( x => x.FileExtension.Equals( formatExtension, StringComparison.OrdinalIgnoreCase ) ).FormatId;

            aiContext.ExportFile( aiScene, fileName, formatId, postProcessSteps );
        }
    }
}
