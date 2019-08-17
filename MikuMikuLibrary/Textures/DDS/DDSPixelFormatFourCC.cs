//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

namespace MikuMikuLibrary.Textures.DDS
{
    /// <summary>
    ///     Old method of identifying Compressed textures.
    ///     DX10 indicates new texture, the DX10 Additional header will contain the true format. See <see cref="DXGI_FORMAT" />
    ///     .
    /// </summary>
    public enum DDSPixelFormatFourCC
    {
        /// <summary>
        ///     Used when FourCC is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     (BC1) Block Compressed Texture. Compresses 4x4 texels.
        ///     Used for Simple Non Alpha.
        /// </summary>
        DXT1 = 0x31545844, // 1TXD i.e. DXT1 backwards

        /// <summary>
        ///     (BC2) Block Compressed Texture. Compresses 4x4 texels.
        ///     Used for Sharp Alpha. Premultiplied alpha.
        /// </summary>
        DXT2 = 0x32545844,

        /// <summary>
        ///     (BC2) Block Compressed Texture. Compresses 4x4 texels.
        ///     Used for Sharp Alpha.
        /// </summary>
        DXT3 = 0x33545844,

        /// <summary>
        ///     (BC3) Block Compressed Texture. Compresses 4x4 texels.
        ///     Used for Gradient Alpha. Premultiplied alpha.
        /// </summary>
        DXT4 = 0x34545844,

        /// <summary>
        ///     (BC3) Block Compressed Texture. Compresses 4x4 texels.
        ///     Used for Gradient Alpha.
        /// </summary>
        DXT5 = 0x35545844,

        /// <summary>
        ///     Fancy new DirectX 10+ format indicator. DX10 Header will contain true format.
        /// </summary>
        DX10 = 0x30315844,

        /// <summary>
        ///     (BC4) Block Compressed Texture. Compresses 4x4 texels.
        ///     Used for Normal (bump) Maps. 8 bit single channel with alpha.
        /// </summary>
        ATI1 = 0x31495441,

        /// <summary>
        ///     (BC5) Block Compressed Texture. Compresses 4x4 texels.
        ///     Used for Normal (bump) Maps. Pair of 8 bit channels.
        /// </summary>
        ATI2N_3Dc = 0x32495441,

        R8G8B8 = 20,
        A8R8G8B8,
        X8R8G8B8,
        R5G6B5,
        X1R5G5B5,
        A1R5G5B5,
        A4R4G4B4,
        R3G3B2,
        A8,
        A8R3G3B2,
        X4R4G4B4,
        A2B10G10R10,
        A8B8G8R8,
        X8B8G8R8,
        G16R16,
        A2R10G10B10,
        A16B16G16R16,

        A8P8 = 40,
        P8,

        L8 = 50,
        A8L8,
        A4L4,

        V8U8 = 60,
        L6V5U5,
        X8L8V8U8,
        Q8W8V8U8,
        V16U16,
        A2W10V10U10,

        UYVY = 0x59565955,
        R8G8_B8G8 = 0x47424752,
        YUY2 = 0x32595559,
        G8R8_G8B8 = 0x42475247,

        D16_LOCKABLE = 70,
        D32,
        D15S1,
        D24S8,
        D24X8,
        D24X4S4,
        D16,

        D32F_LOCKABLE = 82,
        D24FS8,

        L16 = 81,

        Q16Q16V16U16 = 110,
        R16F,
        G16R16F,
        A16B16G16R16F,
        R32F,
        G32R32F,
        A32B32G32R32F,
        CxV8U8
    }
}