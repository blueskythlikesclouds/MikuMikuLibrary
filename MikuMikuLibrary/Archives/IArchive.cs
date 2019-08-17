using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO;

namespace MikuMikuLibrary.Archives
{
    public interface IArchive<THandle> : IEnumerable<THandle>, IBinaryFile
    {
        bool CanAdd { get; }
        bool CanRemove { get; }

        IEnumerable<THandle> Entries { get; }

        bool Contains( THandle handle );
        void Add( THandle handle, Stream source, bool leaveOpen, ConflictPolicy conflictPolicy );
        void Add( THandle handle, string fileName, ConflictPolicy conflictPolicy );
        void Remove( THandle handle );
        void Clear();

        EntryStream<THandle> Open( THandle handle, EntryStreamMode mode );
    }
}