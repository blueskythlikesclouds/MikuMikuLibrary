#include "TextureHelper.h"

using namespace System;

namespace MikuMikuLibrary::Textures::Processing
{
    DXGI_FORMAT GetDXGIFormat( const TextureFormat format )
    {
        switch ( format )
        {
        case TextureFormat::DXT1a:
        case TextureFormat::DXT1:
            return DXGI_FORMAT_BC1_UNORM;

        case TextureFormat::DXT3:
            return DXGI_FORMAT_BC2_UNORM;

        case TextureFormat::DXT5:
            return DXGI_FORMAT_BC3_UNORM;

        case TextureFormat::ATI1:
            return DXGI_FORMAT_BC4_UNORM;

        case TextureFormat::ATI2:
            return DXGI_FORMAT_BC5_UNORM;
        }

        return DXGI_FORMAT_UNKNOWN;
    }

    TextureFormat GetTextureFormat( const DXGI_FORMAT format )
    {
        switch ( format )
        {
        case DXGI_FORMAT_BC1_TYPELESS:
        case DXGI_FORMAT_BC1_UNORM:
        case DXGI_FORMAT_BC1_UNORM_SRGB:
            return TextureFormat::DXT1;

        case DXGI_FORMAT_BC2_TYPELESS:
        case DXGI_FORMAT_BC2_UNORM:
        case DXGI_FORMAT_BC2_UNORM_SRGB:
            return TextureFormat::DXT3;

        case DXGI_FORMAT_BC3_TYPELESS:
        case DXGI_FORMAT_BC3_UNORM:
        case DXGI_FORMAT_BC3_UNORM_SRGB:
            return TextureFormat::DXT5;

        case DXGI_FORMAT_BC4_TYPELESS:
        case DXGI_FORMAT_BC4_UNORM:
            return TextureFormat::ATI1;

        case DXGI_FORMAT_BC5_TYPELESS:
        case DXGI_FORMAT_BC5_UNORM:
            return TextureFormat::ATI2;
        }

        return TextureFormat::Unknown;
    }

    void MakeImage( DirectX::Image& image, const uint width, const uint height, const DXGI_FORMAT format, byte* pixels )
    {
        image.width = width;
        image.height = height;
        image.format = format;
        image.pixels = pixels;

        if ( FAILED( DirectX::ComputePitch( image.format, image.width, image.height, image.rowPitch, image.slicePitch ) ) )
            throw gcnew Exception( "Failed to compute pitch for image" );
    }
}