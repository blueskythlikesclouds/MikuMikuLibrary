using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MikuMikuModel.Configurations;

namespace MikuMikuModel.Nodes
{
    public interface INode : IEnumerable<INode>, INotifyPropertyChanged, IDisposable
    {
        NodeFlags Flags { get; }

        object Data { get; }
        Type DataType { get; }

        INode Parent { get; set; }
        IList<INode> Nodes { get; }

        string Name { get; set; }
        ContextMenuStrip ContextMenuStrip { get; }
        Control Control { get; }
        Bitmap Image { get; }
        object Tag { get; set; }

        Configuration SourceConfiguration { get; set; }

        bool IsPopulated { get; }
        bool IsPendingSynchronization { get; }

        event EventHandler<NodeRenameEventArgs> Renamed;
        event EventHandler<NodeAddEventArgs> Added;
        event EventHandler<NodeRemoveEventArgs> Removed;
        event EventHandler<NodeImportEventArgs> Imported;
        event EventHandler<NodeExportEventArgs> Exported;
        event EventHandler<NodeReplaceEventArgs> Replaced;
        event EventHandler<NodeMoveEventArgs> Moved;

        void Populate();
        void Synchronize();

        T FindParent<T>() where T : INode;
        T FindNode<T>( string nodeName, bool searchChildren = false ) where T : INode;

        void Rename( string name );
        void Rename();

        void Remove();

        void Import( string filePath );
        string[] Import();

        void Export( string filePath );
        string Export();

        void Replace( object data );
        void Replace( string filePath );
        string Replace();

        void Move( int index, int targetIndex );
        void MoveUp();
        void MoveDown();

        void DisposeData();
    }

    [Flags]
    public enum NodeFlags
    {
        None = 0 << 0,
        Rename = 1 << 0,
        Add = 1 << 1,
        Remove = 1 << 2,
        Import = 1 << 3,
        Export = 1 << 4,
        Replace = 1 << 5,
        Move = 1 << 6
    }
}