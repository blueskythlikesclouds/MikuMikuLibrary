namespace MikuMikuLibrary.Misc
{
    public struct Color
    {
        public float R, G, B, A;

        public static readonly Color One = new Color( 1, 1, 1, 1 );
        public static readonly Color Zero = new Color( 0, 0, 0, 0 );

        public Color( float r, float g, float b, float a )
        {
            R = r; G = g; B = b; A = a;
        }

        public override string ToString()
        {
            return $"<{R}, {G}, {B}, {A}>";
        }
    }
}
