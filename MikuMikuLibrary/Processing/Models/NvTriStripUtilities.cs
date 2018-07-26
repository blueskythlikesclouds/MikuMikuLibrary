using System;
using System.IO;
using System.Reflection;

namespace MikuMikuLibrary.Processing.Models
{
    public static class NvTriStripUtilities
    {
        private static MethodInfo generateStripsMethod;
        private static PropertyInfo indicesProperty;
        private static PropertyInfo typeProperty;

        private static readonly string DllName32 = Path.Combine(
            Path.GetDirectoryName( Assembly.GetEntryAssembly().Location ), "ManagedNvTriStrip32.dll" );

        private static readonly string DllName64 = Path.Combine(
            Path.GetDirectoryName( Assembly.GetEntryAssembly().Location ), "ManagedNvTriStrip64.dll" );

        public static ushort[] Generate( ushort[] indices )
        {
            var parameters = new object[] { indices, null };
            generateStripsMethod.Invoke( null, parameters );

            var array = ( Array )parameters[ 1 ];
            object primitiveGroup = array.GetValue( 0 );

            if ( ( int )typeProperty.GetValue( primitiveGroup ) != 0x01 )
                return null;

            return ( ushort[] )indicesProperty.GetValue( primitiveGroup );
        }

        static NvTriStripUtilities()
        {
            Assembly assembly;

            if ( IntPtr.Size == 8 )
                assembly = Assembly.LoadFile( DllName64 );
            else
                assembly = Assembly.LoadFile( DllName32 );

            var utilityType = assembly.GetType( "ManagedNvTriStrip.NvTriStripUtility" );
            var primitiveGroupType = assembly.GetType( "ManagedNvTriStrip.PrimitiveGroup" );
            var primitiveGroupArrayType = assembly.GetType( "ManagedNvTriStrip.PrimitiveGroup[]&" );

            generateStripsMethod = utilityType.GetMethod( "GenerateStrips",
                new Type[] { typeof( ushort[] ), primitiveGroupArrayType } );

            indicesProperty = primitiveGroupType.GetProperty( "Indices" );
            typeProperty = primitiveGroupType.GetProperty( "Type" );
        }
    }
}
