#include "Pch.h"

using namespace System;
using namespace Reflection;
using namespace Runtime::InteropServices;

[assembly:AssemblyTitleAttribute( "Miku Miku Library (Native)" )];
[assembly:AssemblyDescriptionAttribute( "A library that provides the native back-end to Miku Miku Library. This library is loaded automatically." )];
[assembly:AssemblyConfigurationAttribute( "" )];
[assembly:AssemblyCompanyAttribute( "" )];
[assembly:AssemblyProductAttribute( "Miku Miku Library (Native)" )];
[assembly:AssemblyCopyrightAttribute( "Copyright © 2020 Skyth (MIT License)" )];
[assembly:AssemblyTrademarkAttribute( "" )];
[assembly:AssemblyCultureAttribute( "" )];

[assembly:ComVisible( false )];

[assembly:CLSCompliantAttribute( true )];

#ifdef _DEBUG
[assembly:AssemblyVersionAttribute( "1.0.*" )];
#else
[assembly:AssemblyVersionAttribute( "1.0.0" )];
#endif

[assembly:AssemblyKeyFileAttribute( "..\\MikuMikuLibrary.snk" )];