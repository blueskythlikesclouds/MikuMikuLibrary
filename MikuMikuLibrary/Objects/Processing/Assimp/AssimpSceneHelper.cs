using Assimp.Configs;
using Ai = Assimp;

namespace MikuMikuLibrary.Objects.Processing.Assimp;

public static class AssimpSceneHelper
{
    public static Ai.Scene Import(string filePath)
    {
        var aiContext = new Ai.AssimpContext();

        aiContext.SetConfig(new FBXPreservePivotsConfig(false));

        return aiContext.ImportFile(filePath,
            Ai.PostProcessSteps.Triangulate |
            Ai.PostProcessSteps.SortByPrimitiveType |
            Ai.PostProcessSteps.FlipUVs);
    }

    public static void Export(Ai.Scene aiScene, string filePath,
        Ai.PostProcessSteps postProcessSteps = Ai.PostProcessSteps.None)
    {
        var aiContext = new Ai.AssimpContext();

        string formatExtension = Path.GetExtension(filePath).Substring(1);

        string formatId = aiContext.GetSupportedExportFormats()
            .First(x => x.FileExtension.Equals(formatExtension, StringComparison.OrdinalIgnoreCase)).FormatId;

        aiContext.ExportFile(aiScene, filePath, formatId, postProcessSteps);
    }
}