#include "LightMapImporterCore.h"

namespace MikuMikuLibrary::IBLs::Processing
{
    using namespace Misc;

    LightMap^ CreateLightMap(const DirectX::ScratchImage& scratchImage)
    {
        DirectX::ScratchImage convertedScratchImage;

        DirectX::Convert(scratchImage.GetImages(), scratchImage.GetImageCount(), scratchImage.GetMetadata(),
            DXGI_FORMAT_R16G16B16A16_FLOAT, DirectX::TEX_FILTER_DEFAULT, DirectX::TEX_THRESHOLD_DEFAULT, convertedScratchImage);

        const DirectX::TexMetadata metadata = convertedScratchImage.GetMetadata();

        LightMap^ lightMap = gcnew LightMap();

        lightMap->Width = (int) metadata.width;
        lightMap->Height = (int) metadata.height;

        for (size_t i = 0; i < metadata.arraySize; i++)
        {
            array<Half>^ data = gcnew array<Half>((int) (metadata.width * metadata.height * 4));
            const pin_ptr<Half> dataPtr = &data[0];

            const DirectX::Image* image = convertedScratchImage.GetImage(0, i, 0);
            memcpy(dataPtr, image->pixels, data->Length * sizeof(Half));

            lightMap->Sides[i] = data;
        }

        return lightMap;
    }

    LightMap^ CreateLightMap(const DirectX::ScratchImage& scratchImage, int width, int height)
    {
        if (width > 0 && height > 0)
        {
            DirectX::ScratchImage scaledScratchImage;

            DirectX::Resize(scratchImage.GetImages(), scratchImage.GetImageCount(), scratchImage.GetMetadata(),
                width, height, DirectX::TEX_FILTER_BOX, scaledScratchImage);

            return CreateLightMap(scaledScratchImage);
        }

        return CreateLightMap(scratchImage);
    }

    LightMap^ LightMapImporterCore::ImportFromFile(String^ filePath, int width, int height)
    {
        const pin_ptr<const wchar_t> filePathPtr = PtrToStringChars(filePath);

        DirectX::TexMetadata metadata;
        DirectX::ScratchImage scratchImage;

        if (FAILED(DirectX::LoadFromDDSFile(filePathPtr, DirectX::DDS_FLAGS_NONE, &metadata, scratchImage)))
            throw gcnew Exception("Failed to load DDS image file");

        if (!(metadata.miscFlags & DirectX::TEX_MISC_TEXTURECUBE))
            throw gcnew Exception("Image must be a cube map");

        const DXGI_FORMAT format = DXGI_FORMAT_R32G32B32A32_FLOAT;

        if (metadata.format == format)
            return CreateLightMap(scratchImage, width, height);

        if (DirectX::IsCompressed(metadata.format))
        {
            DirectX::ScratchImage decompressedScratchImage;
            DirectX::Decompress(scratchImage.GetImages(), scratchImage.GetImageCount(), scratchImage.GetMetadata(), format, decompressedScratchImage);

            return CreateLightMap(decompressedScratchImage, width, height);
        }

        DirectX::ScratchImage convertedScratchImage;
        DirectX::Convert(scratchImage.GetImages(), scratchImage.GetImageCount(), scratchImage.GetMetadata(), 
            format, DirectX::TEX_FILTER_DEFAULT, DirectX::TEX_THRESHOLD_DEFAULT, convertedScratchImage);

        return CreateLightMap(convertedScratchImage, width, height);
    }
}