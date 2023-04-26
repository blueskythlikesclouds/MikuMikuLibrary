namespace MikuMikuLibrary.Objects.Processing.Interfaces;

public interface IOptimizer
{
    void Optimize(Mesh mesh, bool generateStrips);
}