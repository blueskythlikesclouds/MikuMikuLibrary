using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO;

namespace MikuMikuLibrary.Archives
{
    public interface IArchive : IEnumerable<string>, IBinaryFile
    {
        bool CanAdd { get; }
        bool CanRemove { get; }

        IEnumerable<string> FileNames { get; }

        bool Contains( string fileName );
        void Add( string fileName, Stream source, bool leaveOpen, ConflictPolicy conflictPolicy );
        void Add( string fileName, string sourceFilePath, ConflictPolicy conflictPolicy );
        void Remove( string fileName );
        void Clear();

        EntryStream Open( string fileName, EntryStreamMode mode );
    }
}