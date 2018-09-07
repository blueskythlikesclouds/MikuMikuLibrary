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
        HasSectionFormat = 4,

        /// <summary>
        /// Whether or not the object holds onto the given source stream
        /// and keeps it till the object is disposed.
        /// If this flag does not exist, the source stream is used only
        /// once, simply for reading data.
        /// </summary>
        UsesSourceStream = 8,
    }
}
