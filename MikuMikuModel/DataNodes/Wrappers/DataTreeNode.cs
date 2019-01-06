using System.Linq;
using System.Windows.Forms;

namespace MikuMikuModel.DataNodes.Wrappers
{
    public class DataTreeNode : TreeNode
    {
        private static readonly ImageList sImageList = new ImageList();
        private DataNode mDataNode;

        public static ImageList ImageList => sImageList;
        public DataNode DataNode => mDataNode;

        public new string Name
        {
            get => mDataNode.Name;
            set => mDataNode.Rename( value );
        }

        public new string Text
        {
            get => mDataNode.Name;
            set => mDataNode.Rename( value );
        }

        public override ContextMenuStrip ContextMenuStrip => mDataNode.ContextMenuStrip;

        public void InitializeView()
        {
            if ( !mDataNode.IsViewInitialized )
            {
                Nodes.Clear();
                mDataNode.InitializeView();
            }

            if ( Nodes.Count >= 1 && string.IsNullOrEmpty( Nodes[ 0 ].Name ) )
                Nodes.RemoveAt( 0 );
        }

        private void OnNameChanged( object sender, DataNodeNameChangedEventArgs e )
        {
            base.Name = base.Text = mDataNode.Name;
            //TreeView.SelectedNode = this;
        }

        private void OnNodeAdded( object sender, DataNodeNodeEventArgs e )
        {
            var child = new DataTreeNode( e.ChildNode );
            Nodes.Add( child );
            //TreeView.SelectedNode = child;
        }

        private void OnNodeRemoved( object sender, DataNodeNodeEventArgs e )
        {
            var child = Nodes.OfType<DataTreeNode>().FirstOrDefault( x => x.mDataNode == e.ChildNode );

            if ( child != null )
                Nodes.Remove( child );
        }

        private void OnNodeMoved( object sender, DataNodeNodeMovedEventArgs e )
        {
            var child = Nodes[ e.OldIndex ];
            Nodes.RemoveAt( e.OldIndex );
            Nodes.Insert( e.NewIndex, child );
            //TreeView.SelectedNode = child;
        }

        private void OnDataReplaced( object sender, DataNodeDataReplacedEventArgs e )
        {
            InitializeView();
        }

        public DataTreeNode( DataNode dataNode )
        {
            base.Name = dataNode.Name;
            base.Text = dataNode.Name;

            var imageKey = $"{dataNode.Icon.GetHashCode()}";
            if ( !sImageList.Images.ContainsKey( imageKey ) )
                sImageList.Images.Add( imageKey, dataNode.Icon );

            ImageKey = imageKey;
            SelectedImageKey = imageKey;

            dataNode.NameChanged += OnNameChanged;
            if ( dataNode.Flags.HasFlag( DataNodeFlags.Branch ) )
            {
                dataNode.NodeAdded += OnNodeAdded;
                dataNode.NodeRemoved += OnNodeRemoved;
                dataNode.NodeMoved += OnNodeMoved;
                dataNode.DataReplaced += OnDataReplaced;
            }

            mDataNode = dataNode;

            // Make it look like we have a child node
            // if we are branch
            if ( dataNode.Flags.HasFlag( DataNodeFlags.Branch ) )
                Nodes.Add( new TreeNode( string.Empty ) );
        }
    }
}
