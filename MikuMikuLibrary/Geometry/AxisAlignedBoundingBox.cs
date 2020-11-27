using System;
using System.Collections.Generic;
using System.Numerics;

namespace MikuMikuLibrary.Geometry
{
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

        public BoundingSphere ToBoundingSphere()
        {
            return new BoundingSphere
            {
                Center = Center,
                Radius = SizeMax * ( float ) Math.Sqrt( 2.0 ) / 2.0f
            };
        }

        public BoundingBox ToBoundingBox()
        {
            return new BoundingBox
            {
                Center = Center,
                Width = SizeX,
                Height = SizeY,
                Depth = SizeZ
            };
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