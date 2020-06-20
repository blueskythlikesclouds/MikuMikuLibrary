using System;

namespace MikuMikuModel.Nodes
{
    public class NodeRenameEventArgs : EventArgs
    {
        public string PreviousName { get; }

        public NodeRenameEventArgs( string previousName )
        {
            PreviousName = previousName;
        }
    }

    public class NodeAddEventArgs : EventArgs
    {
        public INode AddedNode { get; }
        public int Index { get; }

        public NodeAddEventArgs( INode addedNode, int index )
        {
            AddedNode = addedNode;
            Index = index;
        }
    }

    public class NodeRemoveEventArgs : EventArgs
    {
        public INode RemovedNode { get; }

        public NodeRemoveEventArgs( INode removedNode )
        {
            RemovedNode = removedNode;
        }
    }

    public class NodeImportEventArgs : EventArgs
    {
        public string FilePath { get; }

        public NodeImportEventArgs( string filePath )
        {
            FilePath = filePath;
        }
    }

    public class NodeExportEventArgs : EventArgs
    {
        public string FilePath { get; }

        public NodeExportEventArgs( string filePath )
        {
            FilePath = filePath;
        }
    }

    public class NodeReplaceEventArgs : EventArgs
    {
        public object PreviousData { get; }

        public NodeReplaceEventArgs( object previousData )
        {
            PreviousData = previousData;
        }
    }

    public class NodeMoveEventArgs : EventArgs
    {
        public INode MovedNode { get; }
        public int PreviousIndex { get; }
        public int NewIndex { get; }

        public NodeMoveEventArgs( INode movedNode, int previousIndex, int newIndex )
        {
            MovedNode = movedNode;
            PreviousIndex = previousIndex;
            NewIndex = newIndex;
        }
    }
}