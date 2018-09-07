using System;
using System.Numerics;

namespace MikuMikuLibrary.Models
{
    public struct BoundingSphere
    {
        public Vector3 Center;
        public float Radius;

        public void Merge( BoundingSphere bSphere )
        {
            // TODO
            return;
        }

        public override string ToString()
        {
            return $"[{Center}, {Radius}]";
        }

        public BoundingSphere( AxisAlignedBoundingBox aabb )
        {
            Center = aabb.Center;
            Radius = aabb.SizeMax / 2f;
        }
    }
}
