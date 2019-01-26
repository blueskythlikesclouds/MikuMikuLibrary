using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MikuMikuModel.DataNodes
{
    [Flags]
    public enum DataNodeFlags
    {
        CanAdd = 1 << 0,
        CanRemove = 1 << 1,
        CanMove = 1 << 2,
        CanReplace = 1 << 3,
        CanImport = 1 << 4,
        CanExport = 1 << 5,
        CanRename = 1 << 6,
    };

    public interface IDataNode
    {
        string Name { get; }
        object DataObject { get; }
        Type DataType { get; }

        IDataNode Parent { get; }

        IEnumerable<IDataNode> Nodes { get; }

        bool IsObjectLocked { get; }
        bool HasPendingChanges { get; }

        void Add( IDataNode node );
        void Insert( int index, IDataNode node );

        void Remove();
        void Remove( IDataNode node );

        void MoveUp();
        void MoveDown();

        void Replace();
        void Replace( object dataObject );
        void Replace( string filePath );

        void Import();
        void Import( string filePath );

        void Export();
        void Export( string filePath );

        void Rename( string name );

        void UnlockObject();
        void LockObject();
    }

    public class DataNodeAddEventArgs : EventArgs
    {
        public IDataNode AddedNode { get; }

        public DataNodeAddEventArgs( IDataNode addedNode )
        {
            AddedNode = addedNode;
        }
    }

    public class DataNodeRemoveEventArgs : EventArgs
    {
        public IDataNode RemovedNode { get; }

        public DataNodeRemoveEventArgs( IDataNode removedNode )
        {
            RemovedNode = removedNode;
        }
    }

    public class DataNodeMoveEventArgs : EventArgs
    {
        public IDataNode MovedNode { get; }

        public DataNodeMoveEventArgs( IDataNode movedNode )
        {
            MovedNode = movedNode;
        }
    }

    public class DataNodeReplaceEventArgs : EventArgs
    {
        public object PreviousObject { get; }

        public DataNodeReplaceEventArgs( object previousObject )
        {
            PreviousObject = previousObject;
        }
    }

    public class DataNodeImportEventArgs : EventArgs
    {
        public IDataNode ImportedNode { get; }

        public DataNodeImportEventArgs( IDataNode importedNode )
        {
            ImportedNode = importedNode;
        }
    }

    public class DataNodeExportEventArgs : EventArgs
    {
        public string FilePath { get; }

        public DataNodeExportEventArgs( string filePath )
        {
            FilePath = filePath;
        }
    }

    public class DataNodeOnRenameEventArgs : EventArgs
    {
        public string PreviousName { get; }

        public DataNodeOnRenameEventArgs( string previousName )
        {
            PreviousName = previousName;
        }
    }
}