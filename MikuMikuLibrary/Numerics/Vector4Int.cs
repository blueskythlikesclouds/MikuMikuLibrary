namespace MikuMikuLibrary.Numerics;

public struct Vector4Int
{
    public int X;
    public int Y;
    public int Z;
    public int W;

    public Vector4Int(int x, int y, int z, int w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public static Vector4Int operator +(Vector4Int a, Vector4Int b)
    {
        return new Vector4Int(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
    }

    public static Vector4Int operator -(Vector4Int a, Vector4Int b)
    {
        return new Vector4Int(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
    }

    public static Vector4Int operator *(Vector4Int a, int scalar)
    {
        return new Vector4Int(a.X * scalar, a.Y * scalar, a.Z * scalar, a.W * scalar);
    }

    public static Vector4Int operator /(Vector4Int a, int scalar)
    {
        return new Vector4Int(a.X / scalar, a.Y / scalar, a.Z / scalar, a.W / scalar);
    }

    public int this[int index]
    {
        get => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            3 => W,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        set
        {
            switch (index)
            {
                case 0: X = value; break;
                case 1: Y = value; break;
                case 2: Z = value; break;
                case 3: W = value; break;
            }
        }
    }

    public override string ToString()
    {
        return $"<{X}, {Y}, {Z}, {W}>";
    }
}