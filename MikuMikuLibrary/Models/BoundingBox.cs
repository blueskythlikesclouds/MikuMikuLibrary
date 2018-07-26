using System;
using System.Collections.Generic;
using System.Numerics;

namespace MikuMikuLibrary.Models
{
    public class BoundingBox
    {
        public Vector3 Min, Max;

        public Vector3 Center => ( Min + Max ) / 2.0f;
        public float SizeX => Max.X - Min.X;
        public float SizeY => Max.Y - Min.Y;
        public float SizeZ => Max.Z - Min.Z;
        public float SizeMax => Math.Max( SizeX, Math.Max( SizeY, SizeZ ) );

        public void AddPoint( Vector3 point )
        {
            Min = Vector3.Min( Min, point );
            Max = Vector3.Max( Max, point );
        }

        public void Merge( BoundingBox aabb )
        {
            Min = Vector3.Min( Min, aabb.Min );
            Max = Vector3.Max( Max, aabb.Max );
        }

        public static BoundingBox FromPoints( IEnumerable<Vector3> points )
        {
            var boundingBox = new BoundingBox();
            foreach ( var point in points )
                boundingBox.AddPoint( point );
            return boundingBox;
        }

        public BoundingBox()
        {
            Min = new Vector3( float.PositiveInfinity );
            Max = new Vector3( float.NegativeInfinity );
        }
    }
}
