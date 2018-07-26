using System.Collections.Generic;
using System.IO;

namespace MikuMikuLibrary.Archives
{
    public interface IArchive<THandle> : IEnumerable<THandle>
    {
        bool CanLoad { get; }
        bool CanSave { get; }
        bool CanAdd { get; }
        bool CanRemove { get; }

        bool Contains( THandle handle );
        void Add( THandle handle, Stream source, bool leaveOpen, ConflictPolicy conflictPolicy );
        void Add( THandle handle, string fileName, ConflictPolicy conflictPolicy );
        void Remove( THandle handle );

        EntryStream<THandle> Open( THandle handle );
        IEnumerable<THandle> EnumerateEntries();

        void Load( string fileName );
        void Load( Stream source, bool leaveOpen );
        void Save( string fileName );
        void Save( Stream destination, bool leaveOpen );
    }
}
