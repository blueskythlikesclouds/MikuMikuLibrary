namespace MikuMikuLibrary.Objects.Processing;

public static class Optimizer
{
    public static void Optimize(Mesh mesh, bool generateStrips)
    {
        Native.Optimizer.Optimize(mesh, generateStrips);
    }

    public static void Optimize(Object obj, bool generateStrips)
    {
        foreach (var mesh in obj.Meshes)
            Optimize(mesh, generateStrips);
    }
}