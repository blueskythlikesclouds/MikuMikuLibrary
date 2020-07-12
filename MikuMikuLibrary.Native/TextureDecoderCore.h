#pragma once

using namespace System;
using namespace Drawing;

namespace MikuMikuLibrary::Textures::Processing
{
    using namespace Interfaces;

    public ref class TextureDecoderCore sealed : public ITextureDecoder
    {
    public:
        virtual Bitmap^ DecodeToBitmap( SubTexture^ subTexture );
        virtual Bitmap^ DecodeToBitmap( Texture^ texture );
        virtual array<Bitmap^, 2>^ DecodeToBitmaps( Texture^ texture );

        virtual void DecodeToFile( SubTexture^ subTexture, String^ filePath );
        virtual void DecodeToFile( Texture^ texture, String^ filePat );
    };
}