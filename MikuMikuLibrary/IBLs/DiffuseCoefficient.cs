using System.Numerics;

namespace MikuMikuLibrary.IBLs
{
    public class DiffuseCoefficient
    {
        public Matrix4x4 R { get; set; } = Matrix4x4.Identity;
        public Matrix4x4 G { get; set; } = Matrix4x4.Identity;
        public Matrix4x4 B { get; set; } = Matrix4x4.Identity;
    }
}