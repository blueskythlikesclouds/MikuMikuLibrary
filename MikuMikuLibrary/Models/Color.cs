namespace MikuMikuLibrary.Models
{
    public struct Color
    {
        public float R, G, B, A;

        public static Color Zero
        {
            get { return new Color( 0, 0, 0, 0 ); }
        }

        public static Color One
        {
            get { return new Color( 1, 1, 1, 1 ); }
        }

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
