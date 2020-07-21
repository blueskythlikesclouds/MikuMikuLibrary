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
            return hasAlpha && formatHint == TextureFormat::DXT1 ? TextureFormat::DXT5 : formatHint;

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

            if ( DirectX::GenerateMipMaps( scratchImage.GetImages(), scratchImage.GetImageCount(), scratchImage.GetMetadata(), DirectX::TEX_FILTER_DEFAULT, 0, mipChain ) )
                throw gcnew Exception( "Failed to generate mip-maps" );

            return EncodeToTexture( mipChain, AdjustFormatHint( formatHint, !mipChain.IsAlphaAllOpaque() ) );
        }

        return EncodeToTexture( scratchImage, AdjustFormatHint( formatHint, !scratchImage.IsAlphaAllOpaque() ) );
    }
}