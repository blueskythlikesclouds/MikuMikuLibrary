using System.Drawing;
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
            BeginUpdate();
            PopulateNode( e.Node as NodeAsTreeNode );

            for ( int i = 0; i < e.Node.Nodes.Count; )
            {
                if ( e.Node.Nodes[ i ] is NodeAsTreeNode )
                    i++;

                else
                    e.Node.Nodes.RemoveAt( i );
            }

            EndUpdate();

            base.OnAfterExpand( e );
        }

        public NodeTreeView()
        {
            ImageList = NodeAsTreeNode.ImageList;
            HideSelection = false;
        }
    }
}