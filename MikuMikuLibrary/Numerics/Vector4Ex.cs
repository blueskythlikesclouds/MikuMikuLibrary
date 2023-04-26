namespace MikuMikuLibrary.Numerics;

public static class Vector4Ex
{
    public static Vector4 NormalizeSum(this Vector4 value)
    {
        float sum = value.X + value.Y + value.Z + value.W;

        if (sum > 0.0f)
        {
            float reciprocalSum = 1.0f / sum;

            return new Vector4(
                value.X * reciprocalSum,
                value.Y * reciprocalSum,
                value.Z * reciprocalSum,
                value.W * reciprocalSum);
        }

        return value;
    }
}