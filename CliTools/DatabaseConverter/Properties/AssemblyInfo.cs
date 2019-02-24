using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle( "Database Converter" )]
[assembly: AssemblyDescription( "A program that converts database files from Hatsune Miku: Project DIVA games to .xml or vice versa." )]
[assembly: AssemblyConfiguration( "" )]
[assembly: AssemblyCompany( "" )]
[assembly: AssemblyProduct( "Database Converter" )]
[assembly: AssemblyCopyright( "Copyright © 2019 Skyth" )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid( "aed51515-37b5-4549-ab70-2a624083e86b" )]

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
[assembly: AssemblyVersion( "1.0.*" )]
#else
[assembly: AssemblyVersion( "1.0.1" )]
#endif
