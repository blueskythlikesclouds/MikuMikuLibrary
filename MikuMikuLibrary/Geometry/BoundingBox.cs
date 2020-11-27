using System;
using System.Numerics;

namespace MikuMikuLibrary.Geometry
{
    public struct BoundingBox
    {
        public Vector3 Center;
        public float Width;
        public float Height;
        public float Depth;

        public BoundingSphere ToBoundingSphere()
        {
            float radius = Math.Max( Width, Math.Max( Height, Depth ) ) * ( float ) Math.Sqrt( 2.0 ) / 2.0f;

            return new BoundingSphere
            {
                Center = Center,
                Radius = radius
            };
        }

        public override string ToString() => 
            $"[{Center}, <{Width}, {Height}, {Depth}>]";

        public BoundingBox( AxisAlignedBoundingBox aabb )
        {
            Center = aabb.Center;
            Width = aabb.SizeX;
            Height = aabb.SizeY;
            Depth = aabb.SizeZ;
        }
    }
}