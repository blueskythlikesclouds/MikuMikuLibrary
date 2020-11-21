#pragma once

using namespace System;

namespace MikuMikuLibrary::Objects::Processing
{
    using namespace Interfaces;

    public ref class StripifierCore sealed : public IStripifier
    {
    public:
        virtual array<uint>^ Stripify( array<uint>^ indices );
    };
}