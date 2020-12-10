using System.Numerics;
using MikuMikuLibrary.Geometry;

namespace MikuMikuLibrary.Rendering.Cameras
{
    public struct Frustum
    {
        public Plane Left;
        public Plane Right;
        public Plane Top;
        public Plane Bottom;
        public Plane Near;
        public Plane Far;

        public bool Intersect( BoundingSphere boundingSphere )
        {
            if ( Plane.DotCoordinate( Left, boundingSphere.Center ) <= -boundingSphere.Radius ) return false;
            if ( Plane.DotCoordinate( Right, boundingSphere.Center ) <= -boundingSphere.Radius ) return false;
            if ( Plane.DotCoordinate( Top, boundingSphere.Center ) <= -boundingSphere.Radius ) return false;
            if ( Plane.DotCoordinate( Bottom, boundingSphere.Center ) <= -boundingSphere.Radius ) return false;
            if ( Plane.DotCoordinate( Near, boundingSphere.Center ) <= -boundingSphere.Radius ) return false;
            if ( Plane.DotCoordinate( Far, boundingSphere.Center ) <= -boundingSphere.Radius ) return false;

            return true;
        }

        public Frustum( Matrix4x4 projection, Matrix4x4 view )
            : this( view * projection )
        {

        }

        public Frustum( Matrix4x4 matrix )
        {
            Left = new Plane(
                matrix.M14 + matrix.M11,
                matrix.M24 + matrix.M21,
                matrix.M34 + matrix.M31,
                matrix.M44 + matrix.M41 );

            Right = new Plane(
                matrix.M14 - matrix.M11,
                matrix.M24 - matrix.M21,
                matrix.M34 - matrix.M31,
                matrix.M44 - matrix.M41 );

            Top = new Plane(
                matrix.M14 - matrix.M12,
                matrix.M24 - matrix.M22,
                matrix.M34 - matrix.M32,
                matrix.M44 - matrix.M42 );

            Bottom = new Plane(
                matrix.M14 + matrix.M12,
                matrix.M24 + matrix.M22,
                matrix.M34 + matrix.M32,
                matrix.M44 + matrix.M42 );

            Near = new Plane(
                matrix.M14 + matrix.M13,
                matrix.M24 + matrix.M23,
                matrix.M34 + matrix.M33,
                matrix.M44 + matrix.M43 );

            Far = new Plane(
                matrix.M14 - matrix.M13,
                matrix.M24 - matrix.M23,
                matrix.M34 - matrix.M33,
                matrix.M44 - matrix.M43 );

            Left = Plane.Normalize( Left );
            Right = Plane.Normalize( Right );
            Top = Plane.Normalize( Top );
            Bottom = Plane.Normalize( Bottom );
            Near = Plane.Normalize( Near );
            Far = Plane.Normalize( Far );
        }
    }
}