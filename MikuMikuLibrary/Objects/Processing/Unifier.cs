namespace MikuMikuLibrary.Objects.Processing;

public class Unifier
{
    public static void Unify(Mesh mesh)
    {
        Native.Unifier.Unify(mesh);
    }

    public static void Unify(Object obj)
    {
        foreach (var mesh in obj.Meshes)
            Unify(mesh);
    }
}