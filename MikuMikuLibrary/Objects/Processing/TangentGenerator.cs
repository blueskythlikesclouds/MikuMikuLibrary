namespace MikuMikuLibrary.Objects.Processing;

public class TangentGenerator
{
    // This function requires isolated meshes as input!!!
    public static void Generate(Object obj)
    {
        Native.TangentGenerator.Generate(obj);
    }
}