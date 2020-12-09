using System;
using System.IO;
using System.Text;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.IO
{
    public interface IBinaryFile : IDisposable
    {
        BinaryFileFlags Flags { get; }
        BinaryFormat Format { get; set; }
        Endianness Endianness { get; set; }
        Encoding Encoding { get; }

        void Read( EndianBinaryReader reader, ISection section = null );
        void Write( EndianBinaryWriter writer, ISection section = null );

        void Load( string filePath );
        void Load( Stream source, bool leaveOpen = false );
        void Save( string filePath );
        void Save( Stream destination, bool leaveOpen = false );
    }

    [Flags]
    public enum BinaryFileFlags
    {
        Load = 1 << 0,
        Save = 1 << 1,
        HasSectionFormat = 1 << 2,
        UsesSourceStream = 1 << 3
    }
}