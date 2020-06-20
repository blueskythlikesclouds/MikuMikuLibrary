using System.Numerics;

namespace MikuMikuLibrary.Geometry
{
    public struct BoundingSphere
    {
        public Vector3 Center;
        public float Radius;

        public BoundingBox ToBoundingBox()
        {
            return new BoundingBox
            {
                Center = Center,
                Width = Radius * 2,
                Height = Radius * 2,
                Depth = Radius * 2
            };
        }

        public override string ToString() => 
            $"[{Center}, {Radius}]";
    }
}