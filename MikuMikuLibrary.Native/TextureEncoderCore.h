#pragma once

using namespace System;
using namespace Drawing;

namespace MikuMikuLibrary::Textures::Processing
{
    using namespace Interfaces;

    public ref class TextureEncoderCore sealed : public ITextureEncoder
    {
    public:
        virtual SubTexture^ EncodeFromBitmap( Bitmap^ bitmap, TextureFormat formatHint );
        virtual Texture^ EncodeFromBitmap( Bitmap^ bitmap, TextureFormat formatHint, bool mipMapHint );

        virtual SubTexture^ EncodeFromFile( String^ filePath, TextureFormat formatHint );
        virtual Texture^ EncodeFromFile( String^ filePath, TextureFormat formatHint, bool mipMapHint );
    };
}