using OpenTK;

namespace MikuMikuLibrary.Rendering.Extensions
{
    public static unsafe class NumericsEx
    {
        public static Vector2 ToGL( this System.Numerics.Vector2 value ) => *( Vector2* ) &value;
        public static Vector3 ToGL( this System.Numerics.Vector3 value ) => *( Vector3* ) &value;
        public static Vector4 ToGL( this System.Numerics.Vector4 value ) => *( Vector4* ) &value;
        public static Matrix4 ToGL( this System.Numerics.Matrix4x4 value ) => *( Matrix4* ) &value;
    }
}
