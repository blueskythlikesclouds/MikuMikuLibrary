using System;

namespace MikuMikuLibrary.IO
{
    public static class BinaryFormatUtilities
    {
        public static bool IsClassic( BinaryFormat format )
        {
            switch ( format )
            {
                case BinaryFormat.DT:
                case BinaryFormat.F:
                case BinaryFormat.FT:
                    return true;

                case BinaryFormat.F2nd:
                case BinaryFormat.X:
                    return false;
            }

            throw new ArgumentException( nameof( format ) );
        }

        public static bool IsModern( BinaryFormat format )
        {
            switch ( format )
            {
                case BinaryFormat.DT:
                case BinaryFormat.F:
                case BinaryFormat.FT:
                    return false;

                case BinaryFormat.F2nd:
                case BinaryFormat.X:
                    return true;
            }

            throw new ArgumentException( nameof( format ) );
        }
    }
}
