using Ai = Assimp;

namespace MikuMikuLibrary.Objects.Processing.Assimp;

public static class AssimpExtensions
{
    public static Vector2 ToNumerics(this Ai.Vector2D value) => new(value.X, value.Y);

    public static Vector3 ToNumerics(this Ai.Vector3D value) => new(value.X, value.Y, value.Z);

    public static Matrix4x4 ToNumerics(this Ai.Matrix4x4 value)
    {
        return new Matrix4x4(
            value.A1, value.A2, value.A3, value.A4,
            value.B1, value.B2, value.B3, value.B4,
            value.C1, value.C2, value.C3, value.C4,
            value.D1, value.D2, value.D3, value.D4);
    }

    public static Matrix4x4 ToNumericsTransposed(this Ai.Matrix4x4 value)
    {
        return new Matrix4x4(
            value.A1, value.B1, value.C1, value.D1,
            value.A2, value.B2, value.C2, value.D2,
            value.A3, value.B3, value.C3, value.D3,
            value.A4, value.B4, value.C4, value.D4);
    }

    public static Ai.Vector2D ToAssimp(this Vector2 value) => new(value.X, value.Y);

    public static Ai.Vector3D ToAssimp(this Vector3 value) => new(value.X, value.Y, value.Z);

    public static Ai.Matrix4x4 ToAssimp(this Matrix4x4 value)
    {
        return new Ai.Matrix4x4(
            value.M11, value.M12, value.M13, value.M14,
            value.M21, value.M22, value.M23, value.M24,
            value.M31, value.M32, value.M33, value.M34,
            value.M41, value.M42, value.M43, value.M44);
    }

    public static Ai.Matrix4x4 ToAssimpTransposed(this Matrix4x4 value)
    {
        return new Ai.Matrix4x4(
            value.M11, value.M21, value.M31, value.M41,
            value.M12, value.M22, value.M32, value.M42,
            value.M13, value.M23, value.M33, value.M43,
            value.M14, value.M24, value.M34, value.M44);
    }

    public static Vector4 ToNumerics(this Ai.Color4D value) => new(value.R, value.G, value.B, value.A);

    public static Ai.Color4D ToAssimpColor(this Vector4 value) => new(value.X, value.Y, value.Z, value.W);
}