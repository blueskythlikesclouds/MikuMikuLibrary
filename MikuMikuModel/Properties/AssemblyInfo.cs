using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle( "Miku Miku Model" )]
[assembly: AssemblyDescription( "This program is the GUI front-end of Miku Miku Library. It allows you to view and edit formats supported by the library, such as models, textures, motions and sprites." )]
[assembly: AssemblyConfiguration( "" )]
[assembly: AssemblyCompany( "" )]
[assembly: AssemblyProduct( "Miku Miku Model" )]
[assembly: AssemblyCopyright( "Copyright © 2020 Skyth (MIT License)" )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid( "c79fac4b-d1dd-4f2d-bc92-a72c00e07931" )]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
#if DEBUG
[assembly: AssemblyVersion( "2.0.*" )]
#else
[assembly: AssemblyVersion( "2.0.3" )]
#endif