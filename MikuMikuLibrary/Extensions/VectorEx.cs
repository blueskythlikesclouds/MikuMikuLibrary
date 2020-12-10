using System.Numerics;

namespace MikuMikuLibrary.Extensions
{
    public static class VectorEx
    {
        public static Vector3 To3D( this Vector4 value )
        {
            return new Vector3( value.X, value.Y, value.Z );
        }

        public static Vector2 To2D( this Vector4 value )
        {
            return new Vector2( value.X, value.Y );
        }

        public static Vector2 To2D( this Vector3 value )
        {
            return new Vector2( value.X, value.Y );
        }
    }
}