using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MikuMikuModel.Configurations;

namespace MikuMikuModel.Nodes
{
    public class ReferenceNode : INode
    {
        private readonly string mName;
        private readonly bool mUsesCustomName;

        public INode Node { get; }

        public IEnumerator<INode> GetEnumerator()
        {
            return Node.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IEnumerable ) Node ).GetEnumerator();
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => Node.PropertyChanged += value;
            remove => Node.PropertyChanged -= value;
        }

        public NodeFlags Flags =>
            Node.Flags & ~( mUsesCustomName ? NodeFlags.Rename : NodeFlags.None );

        public object Data => Node.Data;

        public Type DataType => Node.DataType;

        public INode Parent { get; set; }

        public IList<INode> Nodes => Node.Nodes;

        public string Name
        {
            get => mUsesCustomName ? mName : Node.Name;
            set => Node.Name = value;
        }

        public ContextMenuStrip ContextMenuStrip => Node.ContextMenuStrip;

        public Control Control => Node.Control;

        public Bitmap Image => Node.Image;

        public object Tag
        {
            get => Node.Tag;
            set => Node.Tag = value;
        }

        public Configuration SourceConfiguration
        {
            get => Node.SourceConfiguration;
            set => Node.SourceConfiguration = value;
        }

        public bool IsPopulated => Node.IsPopulated;

        public bool IsPendingSynchronization => Node.IsPendingSynchronization;

        public event EventHandler<NodeRenameEventArgs> Renamed
        {
            add => Node.Renamed += value;
            remove => Node.Renamed -= value;
        }

        public event EventHandler<NodeAddEventArgs> Added
        {
            add => Node.Added += value;
            remove => Node.Added -= value;
        }

        public event EventHandler<NodeRemoveEventArgs> Removed
        {
            add => Node.Removed += value;
            remove => Node.Removed -= value;
        }

        public event EventHandler<NodeImportEventArgs> Imported
        {
            add => Node.Imported += value;
            remove => Node.Imported -= value;
        }

        public event EventHandler<NodeExportEventArgs> Exported
        {
            add => Node.Exported += value;
            remove => Node.Exported -= value;
        }

        public event EventHandler<NodeReplaceEventArgs> Replaced
        {
            add => Node.Replaced += value;
            remove => Node.Replaced -= value;
        }

        public event EventHandler<NodeMoveEventArgs> Moved
        {
            add => Node.Moved += value;
            remove => Node.Moved -= value;
        }

        public void Populate()
        {
            Node.Populate();
        }

        public void Synchronize()
        {
            Node.Synchronize();
        }

        public T FindParent<T>() where T : INode
        {
            for ( var parent = Parent; parent != null; parent = parent.Parent )
                if ( parent is T node )
                    return node;

            return default;
        }

        public T FindNode<T>( string nodeName, bool searchChildren ) where T : INode
        {
            return Node.FindNode<T>( nodeName, searchChildren );
        }

        public void Rename( string name )
        {
            Node.Rename( name );
        }

        public void Rename()
        {
            Node.Rename();
        }

        public void Remove()
        {
            Node.Remove();
        }

        public void Import( string filePath )
        {
            Node.Import( filePath );
        }

        public string[] Import()
        {
            return Node.Import();
        }

        public void Export( string filePath )
        {
            Node.Export( filePath );
        }

        public string Export()
        {
            return Node.Export();
        }

        public void Replace( object data )
        {
            Node.Replace( data );
        }

        public void Replace( string filePath )
        {
            Node.Replace( filePath );
        }

        public string Replace()
        {
            return Node.Replace();
        }

        public void Move( int index, int targetIndex )
        {
            Node.Move( index, targetIndex );
        }

        public void MoveUp()
        {
            Node.MoveUp();
        }

        public void MoveDown()
        {
            Node.MoveDown();
        }

        public void Dispose()
        {
        }

        public void DisposeData()
        {
        }

        public ReferenceNode( INode node )
        {
            Node = node;
        }

        public ReferenceNode( string name, INode node ) : this( node )
        {
            mName = name;
            mUsesCustomName = true;
        }
    }
}