using MikuMikuLibrary.IO.Common;
using System;
using System.IO;

namespace MikuMikuLibrary.IO
{
    public interface IBinaryFile
    {
        BinaryFileFlags Flags { get; }
        BinaryFormat Format { get; set; }
        Endianness Endianness { get; set; }

        void Load( string filePath );
        void Load( Stream source );
        void Save( string filePath );
        void Save( Stream destination );
    }

    [Flags]
    public enum BinaryFileFlags
    {
        Load = 1,
        Save = 2,
        HasSectionFormat = 4,
    }
}
