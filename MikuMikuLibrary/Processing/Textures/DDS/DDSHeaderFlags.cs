//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System;

namespace MikuMikuLibrary.Processing.Textures.DDS
{
    [Flags]
    public enum DDSHeaderFlags
    {
        Caps = 0x1,
        Height = 0x2,
        Width = 0x4,
        Pitch = 0x8,
        PixelFormat = 0x1000,
        MipMapCount = 0x20000,
        LinearSize = 0x80000,
        Depth = 0x800000
    }
}