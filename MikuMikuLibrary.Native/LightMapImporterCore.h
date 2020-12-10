#pragma once

using namespace System;

namespace MikuMikuLibrary::IBLs::Processing
{
    using namespace Interfaces;

    public ref class LightMapImporterCore : public ILightMapImporter
    {
    public:
        virtual LightMap^ ImportFromFile( String^ filePath, int width, int height );
    };
}