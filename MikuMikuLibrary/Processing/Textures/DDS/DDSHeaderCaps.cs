//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System;

namespace MikuMikuLibrary.Processing.Textures.DDS
{
    [Flags]
    public enum DDSHeaderCaps
    {
        Complex = 0x8,
        MipMap = 0x400000,
        Texture = 0x1000,
    }
}