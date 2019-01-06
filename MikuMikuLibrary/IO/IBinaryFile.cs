using System;
using System.IO;

namespace MikuMikuLibrary.IO
{
    public interface IBinaryFile : IDisposable
    {
        BinaryFileFlags Flags { get; }
        BinaryFormat Format { get; set; }
        Endianness Endianness { get; set; }

        void Load( string filePath );
        void Load( Stream source, bool leaveOpen );
        void Save( string filePath );
        void Save( Stream destination, bool leaveOpen );
    }

    [Flags]
    public enum BinaryFileFlags
    {
        Load = 1,
        Save = 2,
        HasModernVersion = 4,
        UsesSourceStream = 8,
    }
}
