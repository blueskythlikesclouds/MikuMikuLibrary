using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.IO.Sections
{
    public interface ISection : IDisposable
    {
        AddressSpace AddressSpace { get; set; }
        EndianBinaryReader Reader { get; }
        Stream BaseStream { get; }
        EndianBinaryWriter Writer { get; }
        object Data { get; }
        long DataOffset { get; }
        long DataSize { get; }
        Type DataType { get; }
        Endianness Endianness { get; set; }
        SectionFlags Flags { get; }
        BinaryFormat Format { get; }
        Encoding Encoding { get; }
        SectionMode Mode { get; }
        SectionInfo SectionInfo { get; }
        IEnumerable<ISection> Sections { get; }
        long SectionSize { get; }
        string Signature { get; }

        void ProcessData();
        void Read( Stream source );
        void Read( Stream source, bool skipSignature );
        void Write( Stream destination );
        void Write( Stream destination, int depth );
    }

    public enum SectionMode
    {
        Read,
        Write
    }

    [Flags]
    public enum SectionFlags
    {
        None = 0,
        HasNoRelocationTable = 1
    }
}