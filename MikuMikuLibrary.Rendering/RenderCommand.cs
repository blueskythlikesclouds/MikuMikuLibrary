using System.Numerics;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Rendering.Arrays;
using MikuMikuLibrary.Rendering.Materials;

namespace MikuMikuLibrary.Rendering
{
    public struct RenderCommand
    {
        public Matrix4x4 Transformation;

        public BoundingSphere BoundingSphere;

        public VertexArray VertexArray;
        public ElementArray ElementArray;

        public Material Material;
    }
}