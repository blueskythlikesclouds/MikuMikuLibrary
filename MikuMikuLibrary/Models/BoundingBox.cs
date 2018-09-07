using System;
using System.Collections.Generic;
using System.Numerics;

namespace MikuMikuLibrary.Models
{
    public struct BoundingBox
    {
        public Vector3 Center;
        public float Width;
        public float Height;
        public float Depth;

        public override string ToString()
        {
            return $"[{Center}, <{Width}, {Height}, {Depth}>]";
        }

        public BoundingBox( AxisAlignedBoundingBox aabb )
        {
            Center = aabb.Center;
            Width = aabb.SizeX;
            Height = aabb.SizeY;
            Depth = aabb.SizeZ;
        }
    }

    public class AxisAlignedBoundingBox
    {
        public Vector3 Min, Max;

        public Vector3 Center => ( Min + Max ) / 2.0f;
        public float SizeX => Max.X - Min.X;
        public float SizeY => Max.Y - Min.Y;
        public float SizeZ => Max.Z - Min.Z;
        public float SizeMax => Math.Max( SizeX, Math.Max( SizeY, SizeZ ) );
        public float Area => SizeX * SizeY * SizeZ;

        public void AddPoint( Vector3 point )
        {
            Min = Vector3.Min( Min, point );
            Max = Vector3.Max( Max, point );
        }

        public void Merge( AxisAlignedBoundingBox aabb )
        {
            Min = Vector3.Min( Min, aabb.Min );
            Max = Vector3.Max( Max, aabb.Max );
        }

        public AxisAlignedBoundingBox()
        {
            Min = new Vector3( float.PositiveInfinity );
            Max = new Vector3( float.NegativeInfinity );
        }

        public AxisAlignedBoundingBox( IEnumerable<Vector3> points ) : this()
        {
            foreach ( var point in points )
                AddPoint( point );
        }
    }
}
