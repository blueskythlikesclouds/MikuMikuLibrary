#include "TextureEncoderCore.h"
#include "TextureHelper.h"

using namespace System;
using namespace Drawing;
using namespace Imaging;

namespace MikuMikuLibrary::Textures::Processing
{
    void LoadImageFile( String^ filePath, DirectX::ScratchImage& scratchImage )
    {
        const pin_ptr<const wchar_t> filePathPtr = PtrToStringChars( filePath );

        HRESULT result;

        if ( filePath->EndsWith( ".dds", StringComparison::OrdinalIgnoreCase ) )
            result = DirectX::LoadFromDDSFile( filePathPtr, DirectX::DDS_FLAGS_NONE, nullptr, scratchImage );

        else if ( filePath->EndsWith( ".tga", StringComparison::OrdinalIgnoreCase ) )
            result = DirectX::LoadFromTGAFile( filePathPtr, nullptr, scratchImage );

        else
            result = DirectX::LoadFromWICFile( filePathPtr, DirectX::WIC_FLAGS_IGNORE_SRGB, nullptr, scratchImage );

        if ( FAILED( result ) )
            throw gcnew Exception( String::Format( "Failed to load image file ({0})", filePath ) );
    }

    void Encode( const DirectX::Image& image, byte* destination, const DXGI_FORMAT format )
    {
        if ( image.format == format )
            memcpy( destination, image.pixels, image.rowPitch * image.height );

        else
        {
            DirectX::ScratchImage scratchImage;

            HRESULT result;

            if ( DirectX::IsCompressed( format ) )
                result = DirectX::Compress( image, format, DirectX::TEX_COMPRESS_DITHER | DirectX::TEX_COMPRESS_PARALLEL, DirectX::TEX_THRESHOLD_DEFAULT, scratchImage );

            else
                result = DirectX::Convert( image, format, DirectX::TEX_FILTER_DEFAULT, DirectX::TEX_THRESHOLD_DEFAULT, scratchImage );

            if ( FAILED( result ) )
                throw gcnew Exception( "Failed to encode texture" );

            memcpy( destination, scratchImage.GetPixels(), scratchImage.GetPixelsSize() );
        }
    }

    void EncodeYCbCr( const DirectX::Image& image, DirectX::XMFLOAT2* yaBuffer, DirectX::XMFLOAT2* cbcrBuffer )
    {
        for ( size_t i = 0; i < image.width; i++ )
        {
            for ( size_t j = 0; j < image.height; j++ )
            {
                const size_t offset = j * image.width + i;

                const Color8& color = ( ( Color8* ) image.pixels )[ offset ];

                const float r = color.r / 255.0f;
                const float g = color.g / 255.0f;
                const float b = color.b / 255.0f;
                const float a = color.a / 255.0f;

                const float y = r * 0.212593317f + g * 0.715214610f + b * 0.0721921176f;

                const float cb = (r * -0.114568502f + g * -0.385435730f + b * 0.5000042320f + 0.503929f) / 1.003922f;
                const float cr = (r * 0.500004232f + g * -0.454162151f + b * -0.0458420813f + 0.503929f) / 1.003922f;

                yaBuffer[ offset ].x = y;
                yaBuffer[ offset ].y = a;

                cbcrBuffer[ offset ].x = cb;
                cbcrBuffer[ offset ].y = cr;
            }
        }
    }

    Texture^ EncodeYCbCrToTexture( const DirectX::Image& image )
    {
        array<SubTexture^, 2>^ subTextures = gcnew array<SubTexture^, 2>( 2, 1 );

        DirectX::ScratchImage luminanceImage;
        luminanceImage.Initialize2D( DXGI_FORMAT_R32G32_FLOAT, image.width, image.height, 1, 1 );

        DirectX::ScratchImage chromaImage;
        chromaImage.Initialize2D( DXGI_FORMAT_R32G32_FLOAT, image.width, image.height, 1, 1 );

        EncodeYCbCr( image, ( DirectX::XMFLOAT2* ) luminanceImage.GetPixels(), ( DirectX::XMFLOAT2* ) chromaImage.GetPixels() );

        DirectX::ScratchImage scaledChromaImage;

        if ( FAILED( DirectX::Resize( *chromaImage.GetImage( 0, 0, 0 ), image.width >> 1, image.height >> 1, DirectX::TEX_FILTER_BOX, scaledChromaImage ) ) )
            throw gcnew Exception( "Failed to resize chroma image" );

        DirectX::ScratchImage compressedLuminanceImage;

        if ( FAILED( DirectX::Compress( *luminanceImage.GetImage( 0, 0, 0 ), DXGI_FORMAT_BC5_UNORM, DirectX::TEX_COMPRESS_PARALLEL, DirectX::TEX_THRESHOLD_DEFAULT, compressedLuminanceImage ) ) )
            throw gcnew Exception( "Failed to compress luminance image" );

        DirectX::ScratchImage compressedChromaImage;

        if ( FAILED( DirectX::Compress( *scaledChromaImage.GetImage( 0, 0, 0 ), DXGI_FORMAT_BC5_UNORM, DirectX::TEX_COMPRESS_PARALLEL, DirectX::TEX_THRESHOLD_DEFAULT, compressedChromaImage ) ) )
            throw gcnew Exception( "Failed to compress chroma image" );

        Texture^ texture = gcnew Texture( ( int ) image.width, ( int ) image.height, TextureFormat::ATI2, 1, 2 );

        {
            const pin_ptr<byte> dataPtr = &texture[ 0, 0 ]->Data[ 0 ];
            memcpy( dataPtr, compressedLuminanceImage.GetPixels(), texture[ 0, 0 ]->Data->Length );
        }

        {
            const pin_ptr<byte> dataPtr = &texture[ 0, 1 ]->Data[ 0 ];
            memcpy( dataPtr, compressedChromaImage.GetPixels(), texture[ 0, 1 ]->Data->Length );
        }

        return texture;
    }

    SubTexture^ EncodeToSubTexture( const DirectX::Image& image, const TextureFormat formatHint )
    {
        if ( !DirectX::IsCompressed( image.format ) )
        {
            TextureFormat format = formatHint;

            if ( formatHint != TextureFormat::Unknown && !TextureFormatUtilities::IsBlockCompressed( formatHint ) )
                format = TextureFormatUtilities::HasAlpha( formatHint ) ? TextureFormat::RGBA8 : TextureFormat::RGB8;

            SubTexture^ subTexture = gcnew SubTexture( ( int ) image.width, ( int ) image.height, format );
            const pin_ptr<byte> dataPtr = &subTexture->Data[ 0 ];

            if ( !TextureFormatUtilities::IsBlockCompressed( format ) )
            {
                const std::unique_ptr<byte[]> encodedData = std::make_unique<byte[]>( image.width * image.height * 4 );
                Encode( image, encodedData.get(), UNCOMPRESSED_FORMAT );

                if ( format == TextureFormat::RGB8 )
                {
                    for ( int i = 0; i < subTexture->Width; i++ )
                    {
                        for ( int j = 0; j < subTexture->Height; j++ )
                        {
                            byte* srcPixel = encodedData.get() + ( ( j * subTexture->Width ) + i ) * 4;
                            byte* dstPixel = ( byte* ) dataPtr + ( ( j * subTexture->Width ) + i ) * 3;

                            const Color8& color = *( Color8* ) srcPixel;

                            dstPixel[ 0 ] = color.r;
                            dstPixel[ 1 ] = color.g;
                            dstPixel[ 2 ] = color.b;
                        }
                    }
                }

                else if ( format == TextureFormat::RGBA8 )
                {
                    for ( int i = 0; i < subTexture->Width; i++ )
                    {
                        for ( int j = 0; j < subTexture->Height; j++ )
                        {
                            byte* srcPixel = encodedData.get() + ( ( j * subTexture->Width ) + i ) * 4;
                            byte* dstPixel = ( byte* ) dataPtr + ( ( j * subTexture->Width ) + i ) * 4;

                            const Color8& color = *( Color8* ) srcPixel;

                            dstPixel[ 0 ] = color.r;
                            dstPixel[ 1 ] = color.g;
                            dstPixel[ 2 ] = color.b;
                            dstPixel[ 3 ] = color.a;
                        }
                    }
                }
            }

            else
            {
                Encode( image, dataPtr, GetDXGIFormat( format ) );
            }

            return subTexture;
        }

        const TextureFormat format = GetTextureFormat( image.format );

        if ( format == TextureFormat::Unknown )
            throw gcnew Exception( "Unsupported compression type" );

        SubTexture^ subTexture = gcnew SubTexture( ( int ) image.width, ( int ) image.height, format );

        const pin_ptr<byte> dataPtr = &subTexture->Data[ 0 ];
        memcpy( dataPtr, image.pixels, subTexture->Data->Length );

        return subTexture;
    }

    Texture^ EncodeToTexture( const DirectX::ScratchImage& scratchImage, const TextureFormat formatHint )
    {
        const int cubeIndexMap[] = { 0, 1, 2, 3, 5, 4 };

        const DirectX::TexMetadata& metadata = scratchImage.GetMetadata();

        array<SubTexture^, 2>^ subTextures = gcnew array<SubTexture^, 2>( ( int ) metadata.arraySize, ( int ) metadata.mipLevels );
        for ( size_t i = 0; i < metadata.arraySize; i++ )
        {
            const size_t index = metadata.arraySize == 6 ? cubeIndexMap[ i ] : i;

            for ( size_t j = 0; j < metadata.mipLevels; j++ )
                subTextures[ ( int ) index, ( int ) j ] = EncodeToSubTexture( *scratchImage.GetImage( j, i, 0 ), formatHint );
        }

        return gcnew Texture( subTextures );
    }

    TextureFormat AdjustFormatHint( const TextureFormat formatHint, const bool hasAlpha )
    {
        if ( formatHint == TextureFormat::Unknown )
            return hasAlpha ? TextureFormat::DXT5 : TextureFormat::DXT1;

        if ( TextureFormatUtilities::IsBlockCompressed( formatHint ) )
            return hasAlpha && formatHint == TextureFormat::DXT1 ? TextureFormat::DXT5 : !hasAlpha && formatHint == TextureFormat::DXT5 ? TextureFormat::DXT1 : formatHint;

        return hasAlpha ? TextureFormat::RGBA8 : TextureFormat::RGB8;
    }

    SubTexture^ TextureEncoderCore::EncodeFromBitmap( Bitmap^ bitmap, TextureFormat formatHint )
    {
        const bool hasAlpha = Extensions::BitmapEx::HasTransparency( bitmap );

        BitmapData^ bitmapData = bitmap->LockBits( Drawing::Rectangle( 0, 0, bitmap->Width, bitmap->Height ), ImageLockMode::ReadOnly, PixelFormat::Format32bppArgb );

        DirectX::Image image;
        MakeImage( image, bitmap->Width, bitmap->Height, UNCOMPRESSED_FORMAT, ( byte* ) bitmapData->Scan0.ToPointer() );

        SubTexture^ subTexture = EncodeToSubTexture( image, AdjustFormatHint( formatHint, hasAlpha ) );

        bitmap->UnlockBits( bitmapData );

        return subTexture;
    }

    Texture^ TextureEncoderCore::EncodeFromBitmap( Bitmap^ bitmap, TextureFormat formatHint, bool generateMipMaps )
    {
        const bool hasAlpha = Extensions::BitmapEx::HasTransparency( bitmap );

        BitmapData^ bitmapData = bitmap->LockBits( Drawing::Rectangle( 0, 0, bitmap->Width, bitmap->Height ), ImageLockMode::ReadOnly, PixelFormat::Format32bppArgb );

        DirectX::Image image;
        MakeImage( image, bitmap->Width, bitmap->Height, UNCOMPRESSED_FORMAT, ( byte* ) bitmapData->Scan0.ToPointer() );

        Texture^ texture;

        if ( generateMipMaps )
        {
            DirectX::ScratchImage mipChain;

            if ( FAILED( DirectX::GenerateMipMaps( image, DirectX::TEX_FILTER_DEFAULT, 0, mipChain ) ) )
                throw gcnew Exception( "Failed to generate mip-maps" );

            texture = EncodeToTexture( mipChain, AdjustFormatHint( formatHint, hasAlpha ) );
        }

        else
        {
            array<SubTexture^, 2>^ subTextures = gcnew array<SubTexture^, 2>( 1, 1 );

            subTextures[ 0, 0 ] = EncodeToSubTexture( image, AdjustFormatHint( formatHint, hasAlpha ) );

            texture = gcnew Texture( subTextures );
        }

        bitmap->UnlockBits( bitmapData );

        return texture;
    }

    Texture^ TextureEncoderCore::EncodeYCbCrFromBitmap( Bitmap^ bitmap )
    {
        BitmapData^ bitmapData = bitmap->LockBits( Drawing::Rectangle( 0, 0, bitmap->Width, bitmap->Height ), ImageLockMode::ReadOnly, PixelFormat::Format32bppArgb );

        DirectX::Image image;
        MakeImage( image, bitmap->Width, bitmap->Height, UNCOMPRESSED_FORMAT, ( byte* ) bitmapData->Scan0.ToPointer() );

        Texture^ texture = EncodeYCbCrToTexture( image );

        bitmap->UnlockBits( bitmapData );

        return texture;
    }

    SubTexture^ TextureEncoderCore::EncodeFromFile( String^ filePath, TextureFormat formatHint )
    {
        DirectX::ScratchImage scratchImage;
        LoadImageFile( filePath, scratchImage );

        return EncodeToSubTexture( *scratchImage.GetImage( 0, 0, 0 ), AdjustFormatHint( formatHint, !scratchImage.IsAlphaAllOpaque() ) );
    }

    Texture^ TextureEncoderCore::EncodeFromFile( String^ filePath, TextureFormat formatHint, bool generateMipMaps )
    {
        DirectX::ScratchImage scratchImage;
        LoadImageFile( filePath, scratchImage );

        const DirectX::TexMetadata& metadata = scratchImage.GetMetadata();

        // If texture is DDS, ignore hints.
        if ( filePath->EndsWith( ".dds", StringComparison::OrdinalIgnoreCase ) )
        {
            formatHint = TextureFormat::RGBA8;
            generateMipMaps = false;
        }

        if ( !DirectX::IsCompressed( metadata.format ) && generateMipMaps && metadata.mipLevels == 1 )
        {
            DirectX::ScratchImage mipChain;

            if ( FAILED( DirectX::GenerateMipMaps( scratchImage.GetImages(), scratchImage.GetImageCount(), scratchImage.GetMetadata(), DirectX::TEX_FILTER_DEFAULT, 0, mipChain ) ) )
                throw gcnew Exception( "Failed to generate mip-maps" );

            return EncodeToTexture( mipChain, AdjustFormatHint( formatHint, !mipChain.IsAlphaAllOpaque() ) );
        }

        return EncodeToTexture( scratchImage, AdjustFormatHint( formatHint, !scratchImage.IsAlphaAllOpaque() ) );
    }

    Texture^ TextureEncoderCore::EncodeYCbCrFromFile(String^ filePath)
    {
        DirectX::ScratchImage scratchImage;
        LoadImageFile( filePath, scratchImage );

        const DirectX::TexMetadata& metadata = scratchImage.GetMetadata();

        if ( metadata.format == UNCOMPRESSED_FORMAT )
            return EncodeYCbCrToTexture( *scratchImage.GetImage( 0, 0, 0 ) );

        DirectX::ScratchImage convertedScratchImage;

        if ( DirectX::IsCompressed( metadata.format ) )
        {
            if ( FAILED( DirectX::Decompress( *scratchImage.GetImage( 0, 0, 0 ), UNCOMPRESSED_FORMAT, convertedScratchImage ) ) )
                throw gcnew Exception( "Failed to decompress image" );
        }
        else
        {
            if ( FAILED( DirectX::Convert( *scratchImage.GetImage( 0, 0, 0 ), UNCOMPRESSED_FORMAT, DirectX::TEX_FILTER_DEFAULT, DirectX::TEX_THRESHOLD_DEFAULT, convertedScratchImage ) ) )
                throw gcnew Exception( "Failed to convert image" );
        }

        return EncodeYCbCrToTexture( *convertedScratchImage.GetImage( 0, 0, 0 ) );
    }
}
