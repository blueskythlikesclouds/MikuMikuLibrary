#pragma once

using namespace System;
using namespace Text;

namespace MikuMikuLibrary
{
    struct Utf8String
    {
        byte buffer[ 1024 ];

        const char* ToCStr() const
        {
            return ( const char* ) buffer;
        }

        Utf8String( String^ string )
        {
            const pin_ptr<const wchar_t> data = PtrToStringChars( string );
            const int length = Encoding::UTF8->GetBytes( ( wchar_t* ) data, string->Length, buffer, sizeof( buffer ) );

            buffer[ length ] = NULL;
        }
    };
}