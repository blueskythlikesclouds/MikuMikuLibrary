#pragma once

using namespace System;

namespace MikuMikuLibrary::Objects::Processing::Fbx
{
    using namespace Interfaces;

    public ref class FbxExporterCore sealed : public IFbxExporter
    {
        FbxManager* lManager;

    public:
        FbxExporterCore();
        ~FbxExporterCore();

        virtual void ExportToFile( ObjectSet^ objectSet, String^ destinationFilePath );
    };
}