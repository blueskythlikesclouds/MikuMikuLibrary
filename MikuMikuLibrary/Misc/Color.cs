using System;

namespace MikuMikuLibrary.Misc
{
    public struct Color : IEquatable<Color>
    {
        public float R, G, B, A;

        public static readonly Color White = new Color( 1, 1, 1, 1 );
        public static readonly Color Transparent = new Color( 0, 0, 0, 0 );

        public Color( float r, float g, float b, float a )
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = R.GetHashCode();
                hash = hash * 31 + G.GetHashCode();
                hash = hash * 31 + B.GetHashCode();
                return hash * 31 + A.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"<{R}, {G}, {B}, {A}>";
        }

        public bool Equals( Color other )
        {
            return other.R == R && other.G == G && other.B == B && other.A == A;
        }
    }
}