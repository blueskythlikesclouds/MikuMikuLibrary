#pragma once

using namespace System;

namespace MikuMikuLibrary::Objects::Processing
{
    using namespace Interfaces;

    public ref class OptimizerCore sealed : public IOptimizer
    {
    public:
        virtual void Optimize(Mesh^ mesh, bool generateStrips);
    };
}