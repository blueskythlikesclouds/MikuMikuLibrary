//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System;

namespace MikuMikuLibrary.Textures.DDS
{
    [Flags]
    public enum DDSHeaderCaps
    {
        Complex = 0x8,
        MipMap = 0x400000,
        Texture = 0x1000,
    }

    [Flags]
    public enum DDSHeaderCaps2
    {
        CubeMap = 0x200,
        CubeMapPositiveX = 0x400,
        CubeMapNegativeX = 0x800,
        CubeMapPositiveY = 0x1000,
        CubeMapNegativeY = 0x2000,
        CubeMapPositiveZ = 0x4000,
        CubeMapNegativeZ = 0x8000,
        Volume = 0x200000,
    }
}