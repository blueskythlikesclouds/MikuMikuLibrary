namespace MikuMikuLibrary.Objects.Processing;

public class ProcessingHelper
{
    public static void ProcessPostImport(Object obj, bool generateStrips)
    {
        // Assume all vertices are already isolated as that's what AssimpImporter does
        TangentGenerator.Generate(obj);
        Unifier.Unify(obj);
        Splitter.Split(obj);
        Optimizer.Optimize(obj, generateStrips);
        AabbCalculator.Calculate(obj);
    }

    public static void GenerateTangents(Object obj, bool generateStrips)
    {
        // Assume this is after the mesh has already gone through processing (eg. existing _obj.bin files)
        // Still need to split because the generator might end up generating more tangents than the vertex limit

        Isolator.Isolate(obj);
        TangentGenerator.Generate(obj);
        Unifier.Unify(obj);
        Splitter.Split(obj);
        Optimizer.Optimize(obj, generateStrips);
    }
}