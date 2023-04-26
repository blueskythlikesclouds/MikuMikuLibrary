#pragma once

using namespace System;

namespace MikuMikuLibrary::Objects::Processing
{
    using namespace Interfaces;

    public ref class UnifierCore sealed : public IUnifier
    {
    public:
        virtual void Unify(Mesh^ mesh);
    };
}