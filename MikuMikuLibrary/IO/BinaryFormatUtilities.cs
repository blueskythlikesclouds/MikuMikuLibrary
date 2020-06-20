using System;

namespace MikuMikuLibrary.IO
{
    public static class BinaryFormatUtilities
    {
        public static bool IsClassic( BinaryFormat format ) => 
            format <= BinaryFormat.FT;

        public static bool IsModern( BinaryFormat format ) => 
            format >= BinaryFormat.F2nd;

        public static AddressSpace GetAddressSpace( BinaryFormat format ) => 
            format == BinaryFormat.X ? AddressSpace.Int64 : AddressSpace.Int32;

        public static BinaryFormat GetFormat( AddressSpace addressSpace ) => 
            addressSpace == AddressSpace.Int64 ? BinaryFormat.X : BinaryFormat.F2nd;
    }
}