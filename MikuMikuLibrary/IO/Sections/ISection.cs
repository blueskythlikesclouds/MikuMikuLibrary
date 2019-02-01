using MikuMikuLibrary.IO.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace MikuMikuLibrary.IO.Sections
{
    public interface ISection : IDisposable
    {
        AddressSpace AddressSpace { get; set; }
        EndianBinaryReader Reader { get; }
        Stream BaseStream { get; }
        EndianBinaryWriter Writer { get; }
        object DataObject { get; }
        long DataOffset { get; }
        long DataSize { get; }
        Type DataType { get; }
        Endianness Endianness { get; set; }
        SectionFlags Flags { get; }
        BinaryFormat Format { get; }
        SectionMode Mode { get; }
        SectionInfo SectionInfo { get; }
        IEnumerable<ISection> Sections { get; }
        long SectionSize { get; }
        string Signature { get; }

        void ProcessDataObject();
        void Read( Stream source );
        void Read( Stream source, bool skipSignature );
        void Write( Stream destination );
        void Write( Stream destination, int depth );
    }

    public enum SectionMode { Read, Write };

    [Flags]
    public enum SectionFlags
    {
        None = 0,
        HasRelocationTable = 1 << 0,
        HasEndianReverseTable = 1 << 1,
    };
}