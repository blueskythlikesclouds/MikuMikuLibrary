using System;

namespace MikuMikuLibrary.IO
{
    public enum AddressSpace
    {
        Int32,
        Int64
    }

    public static class AddressSpaceEx
    {
        public static int GetByteSize( this AddressSpace addressSpace )
        {
            switch ( addressSpace )
            {
                case AddressSpace.Int32:
                    return 4;

                case AddressSpace.Int64:
                    return 8;

                default:
                    throw new ArgumentOutOfRangeException( nameof( addressSpace ) );
            }
        }

        public static BinaryFormat GetCorrespondingModernFormat( this AddressSpace addressSpace )
        {
            switch ( addressSpace )
            {
                case AddressSpace.Int32:
                    return BinaryFormat.F2nd;

                case AddressSpace.Int64:
                    return BinaryFormat.X;

                default:
                    throw new ArgumentOutOfRangeException( nameof( addressSpace ), addressSpace, null );
            }
        }
    }
}