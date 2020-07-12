#include "NativeContext.h"

namespace MikuMikuLibrary
{
    void NativeContext::Initialize()
    {
        CoInitializeEx( nullptr, COINIT_MULTITHREADED );
    }
}