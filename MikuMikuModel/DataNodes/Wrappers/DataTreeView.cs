using System.Linq;
using System.Windows.Forms;

namespace MikuMikuModel.DataNodes.Wrappers
{
    public class DataTreeView : TreeView
    {
        public new DataTreeNode SelectedNode
        {
            get { return ( base.SelectedNode as DataTreeNode ); }
            set { base.SelectedNode = value; }
        }

        public new DataTreeNode TopNode
        {
            get { return Nodes.Count != 0 ? ( DataTreeNode )Nodes[ 0 ] : null; }
        }

        public DataNode TopDataNode
        {
            get { return TopNode?.DataNode; }
        }

        public DataNode SelectedDataNode
        {
            get { return SelectedNode?.DataNode; }
        }

        public Control ControlOfSelectedDataNode
        {
            get { return SelectedDataNode?.Control; }
        }

        private void InitializeNodeView( DataTreeNode treeNode )
        {
            if ( treeNode == null || treeNode.DataNode.IsViewInitialized )
                return;

            BeginUpdate();
            {
                treeNode.InitializeView();
                SelectedNode = treeNode;
            }
            EndUpdate();
        }

        protected override void OnNodeMouseClick( TreeNodeMouseClickEventArgs e )
        {
            InitializeNodeView( e.Node as DataTreeNode );
            base.OnNodeMouseClick( e );
        }

        protected override void OnAfterExpand( TreeViewEventArgs e )
        {
            InitializeNodeView( e.Node as DataTreeNode );
            base.OnAfterExpand( e );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                foreach ( var dataNode in Nodes.OfType<DataTreeNode>().Select( x => x.DataNode ) )
                    dataNode.Dispose();
            }

            base.Dispose( disposing );
        }

        public DataTreeView()
        {
            ImageList = DataTreeNode.ImageList;
        }
    }
}
