#pragma once

namespace MikuMikuLibrary::Textures::Processing
{
    struct Color8
    {
        byte b : 8;
        byte g : 8;
        byte r : 8;
        byte a : 8;

        Color8( const byte r, const byte g, const byte b, const byte a ) : b( b ), g( g ), r( r ), a( a ) {}
    };

    const DXGI_FORMAT UNCOMPRESSED_FORMAT = DXGI_FORMAT_B8G8R8A8_UNORM;

    DXGI_FORMAT GetDXGIFormat( TextureFormat format );
    TextureFormat GetTextureFormat( DXGI_FORMAT format );

    void MakeImage( DirectX::Image& image, uint width, uint height, DXGI_FORMAT format, byte* pixels );
}