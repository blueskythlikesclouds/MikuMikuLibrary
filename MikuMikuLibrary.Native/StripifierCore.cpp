#include "StripifierCore.h"

using namespace Collections::Generic;
using namespace Linq;

namespace MikuMikuLibrary::Objects::Processing
{
    array<uint>^ StripifierCore::Stripify( array<uint>^ indices )
    {
        if ( indices == nullptr || indices->Length == 0 )
            return nullptr;

        array<uint>^ result = gcnew array<uint>( ( int ) meshopt_stripifyBound( indices->Length ) );

        size_t count;
        {
            const pin_ptr<uint> indicesPtr = &indices[ 0 ];
            const pin_ptr<uint> resultPtr = &result[ 0 ];

            count = meshopt_stripify( resultPtr, indicesPtr, indices->Length, Enumerable::Max( (IEnumerable<uint>^)indices ) + 1, 0 );
        }

        Array::Resize( result, ( int ) count );
        return result;
    }
}