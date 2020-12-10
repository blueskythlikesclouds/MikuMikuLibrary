using MikuMikuLibrary.Misc;

namespace MikuMikuLibrary.IBLs
{
    public enum LightMapType
    {
        DiffuseIBL,
        DiffuseIBLShadowed,

        SpecularIBLShiny,
        SpecularIBLRough,

        SpecularIBLShinyShadowed,
        SpecularIBLRoughShadowed
    }

    public enum LightMapSide
    {
        PositiveX,
        NegativeX,

        PositiveY,
        NegativeY,

        PositiveZ,
        NegativeZ
    }

    public class LightMap
    {
        public const int SideCount = 6;

        public int Width { get; set; }
        public int Height { get; set; }

        public Half[][] Sides { get; }

        public LightMap()
        {
            Sides = new Half[ SideCount ][];
        }
    }
}