using System.Windows.Forms;

namespace MikuMikuModel.Nodes.Wrappers
{
    public class NodeTreeView : TreeView
    {
        public new NodeAsTreeNode SelectedNode
        {
            get => base.SelectedNode as NodeAsTreeNode;
            set => base.SelectedNode = value;
        }

        public new NodeAsTreeNode TopNode => Nodes.Count != 0 ? ( NodeAsTreeNode ) Nodes[ 0 ] : null;
        public INode TopDataNode => TopNode?.Node;
        public INode SelectedDataNode => SelectedNode?.Node;
        public Control ControlOfSelectedDataNode => SelectedDataNode?.Control;

        private void PopulateNode( NodeAsTreeNode node )
        {
            if ( node == null || node.Node.IsPopulated || !node.Node.Flags.HasFlag( NodeFlags.Add ) )
                return;

            BeginUpdate();
            {
                node.Populate();
                SelectedNode = node;
            }
            EndUpdate();
        }

        protected override void OnAfterSelect( TreeViewEventArgs e )
        {
            PopulateNode( e.Node as NodeAsTreeNode );
            base.OnAfterSelect( e );
        }

        protected override void OnAfterExpand( TreeViewEventArgs e )
        {
            PopulateNode( e.Node as NodeAsTreeNode );

            if ( e.Node.Nodes.Count >= 1 && string.IsNullOrEmpty( e.Node.Nodes[ 0 ].Name ) )
                e.Node.Nodes.RemoveAt( 0 );

            base.OnAfterExpand( e );
        }

        public NodeTreeView()
        {
            ImageList = NodeAsTreeNode.ImageList;
            HideSelection = false;
        }
    }
}