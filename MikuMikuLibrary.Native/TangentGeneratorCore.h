#pragma once

using namespace System;

namespace MikuMikuLibrary::Objects::Processing
{
    using namespace Interfaces;

    public ref class TangentGeneratorCore sealed : public ITangentGenerator
    {
    public:
        virtual void Generate(Objects::Object^ obj);
    };
}