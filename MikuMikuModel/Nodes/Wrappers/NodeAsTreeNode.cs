using System;
using System.Linq;
using System.Windows.Forms;
using MikuMikuModel.Resources.Styles;

namespace MikuMikuModel.Nodes.Wrappers
{
    public class NodeAsTreeNode : TreeNode, IDisposable
    {
        public static ImageList ImageList { get; } = new ImageList { ColorDepth = ColorDepth.Depth32Bit };

        public INode Node { get; }

        public bool HideContextMenuStrip { get; set; }

        public new string Name
        {
            get => Node.Name;
            set => Node.Name = value;
        }

        public new string Text
        {
            get => Node.Name;
            set => Node.Name = value;
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                if ( HideContextMenuStrip )
                    return null;

                var contextMenuStrip = Node.ContextMenuStrip;
                if ( contextMenuStrip != null )
                    contextMenuStrip.Renderer = StyleSet.CurrentStyle?.ToolStripRenderer;

                return contextMenuStrip;
            }
        }

        private void OnNodeMoved( object sender, NodeMoveEventArgs args )
        {
            var movedNode = Nodes[ args.PreviousIndex ];
            Nodes.RemoveAt( args.PreviousIndex );
            Nodes.Insert( args.NewIndex, movedNode );
            TreeView.SelectedNode = movedNode;
        }

        private void OnNodeRemoved( object sender, NodeRemoveEventArgs args )
        {
            var removedNode = Nodes.OfType<NodeAsTreeNode>()
                .FirstOrDefault( x => x.Node == args.RemovedNode );

            if ( removedNode != null ) Nodes.Remove( removedNode );
        }

        private void OnNodeAdded( object sender, NodeAddEventArgs args )
        {
            Nodes.Add( new NodeAsTreeNode( args.AddedNode ) { HideContextMenuStrip = HideContextMenuStrip } );
        }

        private void OnNodeRenamed( object sender, NodeRenameEventArgs args )
        {
            base.Text = base.Name = Node.Name;
        }

        public void Populate()
        {
            if ( Node.IsPopulated )
                return;

            Node.Populate();
        }

        public NodeAsTreeNode( INode node, bool hideNodes = false )
        {
            Node = node;

            base.Text = base.Name = Node.Name;

            if ( Node.Image != null )
            {
                string imageKey = $"{Node.Image.GetHashCode()}";
                if ( !ImageList.Images.ContainsKey( imageKey ) )
                    ImageList.Images.Add( imageKey, Node.Image );

                ImageKey = imageKey;
                SelectedImageKey = imageKey;
            }

            if ( node.Flags.HasFlag( NodeFlags.Rename ) )
                Node.Renamed += OnNodeRenamed;

            if ( node.Flags.HasFlag( NodeFlags.Add ) && !hideNodes )
                Node.Added += OnNodeAdded;

            Node.Removed += OnNodeRemoved;

            if ( node.Flags.HasFlag( NodeFlags.Move ) )
                Node.Moved += OnNodeMoved;

            if ( node.Flags.HasFlag( NodeFlags.Add ) && !hideNodes )
            {
                if ( node.IsPopulated )
                {
                    foreach ( var childNode in Node.Nodes )
                        Nodes.Add( new NodeAsTreeNode( childNode ) );
                }
                else
                {
                    Nodes.Add( new TreeNode() );
                }
            }
        }

        public void Dispose()
        {
            Node.Renamed -= OnNodeRenamed;
            Node.Added -= OnNodeAdded;
            Node.Removed -= OnNodeRemoved;
            Node.Moved -= OnNodeMoved;
            Node.Dispose();

            foreach ( var node in Nodes.OfType<NodeAsTreeNode>() )
                node.Dispose();
        }
    }
}