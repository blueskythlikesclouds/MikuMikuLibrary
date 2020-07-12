#include "TextureDecoderCore.h"
#include "TextureHelper.h"

using namespace System;
using namespace Imaging;
using namespace IO;

namespace MikuMikuLibrary::Textures::Processing
{
    struct UncompressedFormatInfo
    {
        uint BytesPerPixel;
        Color8( *Decoder )( byte* );
    };

    Color8 DecodeA8( byte* pixel );
    Color8 DecodeRGB8( byte* pixel );
    Color8 DecodeRGBA8( byte* pixel );
    Color8 DecodeRGB5( byte* pixel );
    Color8 DecodeRGB5A1( byte* pixel );
    Color8 DecodeRGBA4( byte* pixel );

    const UncompressedFormatInfo FORMAT_INFOS[] =
    {
        { 2, DecodeA8 },
        { 3, DecodeRGB8 },
        { 4, DecodeRGBA8 },
        { 2, DecodeRGB5 },
        { 2, DecodeRGB5A1 },
        { 2, DecodeRGBA4 },
    };

    Color8 DecodeA8( byte* pixel )
    {
        return Color8( 0xFF, 0xFF, 0xFF, pixel[ 0 ] );
    }

    Color8 DecodeRGB8( byte* pixel )
    {
        return Color8( pixel[ 0 ], pixel[ 1 ], pixel[ 2 ], 0xFF );
    }

    Color8 DecodeRGBA8( byte* pixel )
    {
        return Color8( pixel[ 0 ], pixel[ 1 ], pixel[ 2 ], pixel[ 3 ] );
    }

    Color8 DecodeRGB5( byte* pixel )
    {
        const ushort value = *( ushort* ) pixel;

        byte r = value & 0x1F;
        byte g = ( value >> 5 ) & 0x1F;
        byte b = ( value >> 10 ) & 0x1F;

        r = ( r << 3 ) | ( r >> 2 );
        g = ( g << 3 ) | ( g >> 2 );
        b = ( b << 3 ) | ( b >> 2 );

        return Color8( r, g, b, 0xFF );
    }

    Color8 DecodeRGB5A1( byte* pixel )
    {
        const ushort value = *( ushort* ) pixel;

        byte r = value & 0x1F;
        byte g = ( value >> 5 ) & 0x1F;
        byte b = ( value >> 10 ) & 0x1F;
        byte a = value >> 15;

        r = ( r << 3 ) | ( r >> 2 );
        g = ( g << 3 ) | ( g >> 2 );
        b = ( b << 3 ) | ( b >> 2 );
        a *= 0xFF;

        return Color8( r, g, b, a );
    }

    Color8 DecodeRGBA4( byte* pixel )
    {
        const ushort value = *( ushort* ) pixel;

        byte r = value & 0xF;
        byte g = ( value >> 4 ) & 0xF;
        byte b = ( value >> 8 ) & 0xF;
        byte a = ( value >> 12 ) & 0xF;

        r |= r << 4;
        g |= g << 4;
        b |= b << 4;
        a |= a << 4;

        return Color8( r, g, b, a );
    }

    void DecodeYCbCr( Texture^ texture, byte* destination, const size_t destinationLength )
    {
        assert( destinationLength == texture->Width * texture->Height * 4 );

        SubTexture^ luminance = texture[ 0 ];
        SubTexture^ chroma = texture[ 1 ];

        const pin_ptr<byte> luminanceDataPtr = &luminance->Data[ 0 ];
        const pin_ptr<byte> chromaDataPtr = &chroma->Data[ 0 ];

        DirectX::Image luminanceImage;
        MakeImage( luminanceImage, luminance->Width, luminance->Height, DXGI_FORMAT_BC5_UNORM, luminanceDataPtr );

        DirectX::Image chromaImage;
        MakeImage( chromaImage, chroma->Width, chroma->Height, DXGI_FORMAT_BC5_UNORM, chromaDataPtr );

        DirectX::ScratchImage luminanceScratchImage;

        if ( FAILED( DirectX::Decompress( luminanceImage, DXGI_FORMAT_R32G32_FLOAT, luminanceScratchImage ) ) )
            throw gcnew Exception( "Failed to decode luminance texture" );

        DirectX::ScratchImage chromaScratchImage;

        if ( FAILED( DirectX::Decompress( chromaImage, DXGI_FORMAT_R32G32_FLOAT, chromaScratchImage ) ) )
            throw gcnew Exception( "Failed to decode chroma texture" );

        DirectX::ScratchImage chromaScaledScratchImage;
        if ( FAILED( DirectX::Resize( chromaScratchImage.GetImages(), chromaScratchImage.GetImageCount(), chromaScratchImage.GetMetadata(),
            luminance->Width, luminance->Height, DirectX::TEX_FILTER_LINEAR, chromaScaledScratchImage ) ) )
        {
            throw gcnew Exception( "Failed to resize chroma texture" );
        }

        for ( int i = 0; i < luminance->Width; i++ )
        {
            for ( int j = 0; j < luminance->Height; j++ )
            {
                float* luminancePixel = ( float* ) ( luminanceScratchImage.GetPixels() + ( j * luminance->Width + i ) * 8 );
                float* chromaPixel = ( float* ) ( chromaScaledScratchImage.GetPixels() + ( j * luminance->Width + i ) * 8 );

                byte* dstPixel = destination + ( j * luminance->Width + i ) * 4;

                const float luminanceR = luminancePixel[ 0 ];
                const float chromaR = chromaPixel[ 0 ] * 1.003922f - 0.503929f;
                const float chromaG = chromaPixel[ 1 ] * 1.003922f - 0.503929f;

                const float r = std::min( 1.0f, std::max( 0.0f, luminanceR + 1.5748f * chromaG ) ) * 255.0f;
                const float g = std::min( 1.0f, std::max( 0.0f, luminanceR + -0.1873f * chromaR + -0.4681f * chromaG ) ) * 255.0f;
                const float b = std::min( 1.0f, std::max( 0.0f, luminanceR + 1.8556f * chromaR ) ) * 255.0f;
                const float a = std::min( 1.0f, std::max( 0.0f, luminancePixel[ 1 ] ) ) * 255.0f;

                *( Color8* ) dstPixel = Color8( ( byte ) r, ( byte ) g, ( byte ) b, ( byte ) a );
            }
        }
    }

    void DecodeSubTexture( SubTexture^ subTexture, byte* destination, const size_t destinationLength )
    {
        assert( destinationLength == subTexture->Width * subTexture->Height * 4 );

        const pin_ptr<byte> sourceDataPtr = &subTexture->Data[ 0 ];

        if ( !TextureFormatUtilities::IsBlockCompressed( subTexture->Format ) )
        {
            const UncompressedFormatInfo& info = FORMAT_INFOS[ ( int ) subTexture->Format ];

            for ( int i = 0; i < subTexture->Width; i++ )
            {
                for ( int j = 0; j < subTexture->Height; j++ )
                {
                    byte* srcPixel = ( byte* ) sourceDataPtr + ( ( j * subTexture->Width ) + i ) * info.BytesPerPixel;
                    byte* dstPixel = destination + ( ( j * subTexture->Width ) + i ) * 4;

                    *( Color8* ) dstPixel = info.Decoder( srcPixel );
                }
            }
        }

        else
        {
            DirectX::Image image;
            MakeImage( image, subTexture->Width, subTexture->Height, GetDXGIFormat( subTexture->Format ), sourceDataPtr );

            DirectX::ScratchImage scratchImage;

            HRESULT result;

            if ( DirectX::IsCompressed( image.format ) )
                result = DirectX::Decompress( image, UNCOMPRESSED_FORMAT, scratchImage );

            else
                result = DirectX::Convert( image, UNCOMPRESSED_FORMAT, DirectX::TEX_FILTER_DEFAULT, DirectX::TEX_THRESHOLD_DEFAULT, scratchImage );

            if ( FAILED( result ) )
                throw gcnew Exception( "Failed to decode sub-texture" );

            assert( destinationLength == scratchImage.GetPixelsSize() );

            // P*ss Yellow -> Purple because it's more pleasant to look at
            if ( image.format == DXGI_FORMAT_BC5_UNORM )
            {
                Color8* colors = ( Color8* ) scratchImage.GetPixels();

                for ( size_t i = 0; i < scratchImage.GetPixelsSize() / sizeof( Color8 ); i++ )
                    colors[ i ].b = 0xFF;
            }

            memcpy( destination, scratchImage.GetPixels(), scratchImage.GetPixelsSize() );
        }
    }

    REFGUID GetWicCodec( String^ filePath )
    {
        if ( filePath->EndsWith( ".bmp", StringComparison::OrdinalIgnoreCase ) )
            return GetWICCodec( DirectX::WIC_CODEC_BMP );

        if ( filePath->EndsWith( ".jpg", StringComparison::OrdinalIgnoreCase ) ||
            filePath->EndsWith( ".jpeg", StringComparison::OrdinalIgnoreCase ) )
            return GetWICCodec( DirectX::WIC_CODEC_JPEG );

        if ( filePath->EndsWith( ".png", StringComparison::OrdinalIgnoreCase ) )
            return GetWICCodec( DirectX::WIC_CODEC_PNG );

        if ( filePath->EndsWith( ".tif", StringComparison::OrdinalIgnoreCase ) ||
            filePath->EndsWith( ".tiff", StringComparison::OrdinalIgnoreCase ) )
            return GetWICCodec( DirectX::WIC_CODEC_TIFF );

        if ( filePath->EndsWith( ".gif", StringComparison::OrdinalIgnoreCase ) )
            return GetWICCodec( DirectX::WIC_CODEC_GIF );

        if ( filePath->EndsWith( ".hdp", StringComparison::OrdinalIgnoreCase ) ||
            filePath->EndsWith( ".jxr", StringComparison::OrdinalIgnoreCase ) ||
            filePath->EndsWith( ".wdp", StringComparison::OrdinalIgnoreCase ) )
            return GetWICCodec( DirectX::WIC_CODEC_WMP );

        if ( filePath->EndsWith( ".ico", StringComparison::OrdinalIgnoreCase ) )
            return GetWICCodec( DirectX::WIC_CODEC_ICO );

        return GetWICCodec( DirectX::WIC_CODEC_PNG );
    }

    void DecodeToWicOrTgaFile( String^ filePath, const uint width, const uint height, byte* pixels )
    {
        const pin_ptr<const wchar_t> filePathPtr = PtrToStringChars( filePath );

        DirectX::Image image;
        MakeImage( image, width, height, UNCOMPRESSED_FORMAT, pixels );

        HRESULT result;

        if ( filePath->EndsWith( ".tga", StringComparison::OrdinalIgnoreCase ) )
            result = SaveToTGAFile( image, filePathPtr );

        else
            result = SaveToWICFile( image, DirectX::WIC_FLAGS_FORCE_SRGB, GetWicCodec( filePath ), filePathPtr );

        if ( FAILED( result ) )
            throw gcnew Exception( String::Format( "Failed to save image file ({0})", filePath ) );
    }

    Bitmap^ TextureDecoderCore::DecodeToBitmap( SubTexture^ subTexture )
    {
        Bitmap^ bitmap = gcnew Bitmap( subTexture->Width, subTexture->Height, PixelFormat::Format32bppArgb );

        BitmapData^ bitmapData = bitmap->LockBits( Drawing::Rectangle( 0, 0, subTexture->Width, subTexture->Height ), ImageLockMode::WriteOnly, PixelFormat::Format32bppArgb );

        DecodeSubTexture( subTexture, ( byte* ) bitmapData->Scan0.ToPointer(), bitmapData->Stride * bitmapData->Height );

        bitmap->UnlockBits( bitmapData );

        return bitmap;
    }

    Bitmap^ TextureDecoderCore::DecodeToBitmap( Texture^ texture )
    {
        if ( texture->IsYCbCr )
        {
            array<Bitmap^, 2>^ data = gcnew array<Bitmap^, 2>( 1, 1 );

            Bitmap^ bitmap = gcnew Bitmap( texture->Width, texture->Height );
            BitmapData^ bitmapData = bitmap->LockBits( Drawing::Rectangle( 0, 0, texture->Width, texture->Height ), ImageLockMode::WriteOnly, PixelFormat::Format32bppArgb );
            {
                DecodeYCbCr( texture, ( byte* ) bitmapData->Scan0.ToPointer(), bitmapData->Stride * bitmapData->Height );
            }

            bitmap->UnlockBits( bitmapData );
            return bitmap;
        }

        return DecodeToBitmap( texture[ 0, 0 ] );
    }

    array<Bitmap^, 2>^ TextureDecoderCore::DecodeToBitmaps( Texture^ texture )
    {
        if ( texture->IsYCbCr )
        {
            array<Bitmap^, 2>^ data = gcnew array<Bitmap^, 2>( 1, 1 );
            data[ 0, 0 ] = DecodeToBitmap( texture );
            return data;
        }

        array<Bitmap^, 2>^ data = gcnew array<Bitmap^, 2>( texture->ArraySize, texture->MipMapCount );

        for ( int i = 0; i < texture->ArraySize; i++ )
        {
            for ( int j = 0; j < texture->MipMapCount; j++ )
                data[ i, j ] = DecodeToBitmap( texture[ i, j ] );
        }

        return data;
    }

    void TextureDecoderCore::DecodeToFile( SubTexture^ subTexture, String^ filePath )
    {
        if ( filePath->EndsWith( ".dds", StringComparison::OrdinalIgnoreCase ) )
        {
            const pin_ptr<const wchar_t> filePathPtr = PtrToStringChars( filePath );

            if ( !TextureFormatUtilities::IsBlockCompressed( subTexture->Format ) )
            {
                const size_t decodedDataLength = subTexture->Width * subTexture->Height * 4;
                const std::unique_ptr<byte[]> decodedData = std::make_unique<byte[]>( decodedDataLength );

                DirectX::Image image;
                MakeImage( image, subTexture->Width, subTexture->Height, UNCOMPRESSED_FORMAT, decodedData.get() );

                DecodeSubTexture( subTexture, decodedData.get(), decodedDataLength );

                if ( FAILED( SaveToDDSFile( image, DirectX::DDS_FLAGS_NONE, filePathPtr ) ) )
                    throw gcnew Exception( String::Format( "Failed to save image file ({0})", filePath ) );
            }

            else
            {
                const pin_ptr<byte> dataPtr = &subTexture->Data[ 0 ];

                DirectX::Image image;
                MakeImage( image, subTexture->Width, subTexture->Height, GetDXGIFormat( subTexture->Format ), dataPtr );

                if ( FAILED( SaveToDDSFile( image, DirectX::DDS_FLAGS_NONE, filePathPtr ) ) )
                    throw gcnew Exception( String::Format( "Failed to save image file ({0})", filePath ) );
            }
        }

        else
        {
            const size_t decodedDataLength = subTexture->Width * subTexture->Height * 4;
            const std::unique_ptr<byte[]> decodedData = std::make_unique<byte[]>( decodedDataLength );

            DecodeSubTexture( subTexture, decodedData.get(), decodedDataLength );

            DecodeToWicOrTgaFile( filePath, subTexture->Width, subTexture->Height, decodedData.get() );
        }
    }

    void TextureDecoderCore::DecodeToFile( Texture^ texture, String^ filePath )
    {
        const bool isCompressed = TextureFormatUtilities::IsBlockCompressed( texture->Format );

        if ( filePath->EndsWith( ".dds", StringComparison::OrdinalIgnoreCase ) )
        {
            DirectX::ScratchImage scratchImage;

            HRESULT result;

            DXGI_FORMAT format = UNCOMPRESSED_FORMAT;

            if ( TextureFormatUtilities::IsBlockCompressed( texture->Format ) )
                format = GetDXGIFormat( texture->Format );

            if ( texture->ArraySize == 6 )
                result = scratchImage.InitializeCube( format, texture->Width, texture->Height, 1, texture->MipMapCount );

            else
                result = scratchImage.Initialize2D( format, texture->Width, texture->Height, texture->ArraySize, texture->MipMapCount );

            if ( FAILED( result ) )
                throw gcnew Exception( "Failed to initialize image" );

            const int cubeIndexMap[] = { 0, 1, 2, 3, 5, 4 };

            for ( int i = 0; i < texture->ArraySize; i++ )
            {
                for ( int j = 0; j < texture->MipMapCount; j++ )
                {
                    SubTexture^ subTexture = texture[ i, j ];
                    const pin_ptr<byte> dataPtr = &subTexture->Data[ 0 ];

                    const DirectX::Image* image = scratchImage.GetImage(
                        j, texture->ArraySize == 6 ? cubeIndexMap[ i ] : i, 0 );

                    if ( !isCompressed )
                        DecodeSubTexture( subTexture, image->pixels, subTexture->Width * subTexture->Height * 4 );

                    else
                        memcpy( image->pixels, dataPtr, subTexture->Data->Length );
                }
            }

            const pin_ptr<const wchar_t> filePathPtr = PtrToStringChars( filePath );

            if ( FAILED( DirectX::SaveToDDSFile( scratchImage.GetImages(), scratchImage.GetImageCount(), scratchImage.GetMetadata(), DirectX::DDS_FLAGS_FORCE_DX9_LEGACY, filePathPtr ) ) )
                throw gcnew Exception( String::Format( "Failed to save image file ({0})", filePath ) );
        }

        else
        {
            const size_t decodedDataLength = texture->Width * texture->Height * 4;
            const std::unique_ptr<byte[]> decodedData = std::make_unique<byte[]>( decodedDataLength );

            if ( texture->IsYCbCr )
                DecodeYCbCr( texture, decodedData.get(), decodedDataLength );

            else
                DecodeSubTexture( texture[ 0 ], decodedData.get(), decodedDataLength );

            DecodeToWicOrTgaFile( filePath, texture->Width, texture->Height, decodedData.get() );
        }
    }
}