using System.Linq;
using System.Windows.Forms;

namespace MikuMikuModel.DataNodes.Wrappers
{
    public class DataTreeNode : TreeNode
    {
        private static readonly ImageList imageList = new ImageList();

        public static ImageList ImageList
        {
            get { return imageList; }
        }

        private DataNode dataNode;

        public DataNode DataNode
        {
            get { return dataNode; }
        }

        public new string Name
        {
            get { return dataNode.Name; }
            set { dataNode.Rename( value ); }
        }

        public new string Text
        {
            get { return dataNode.Name; }
            set { dataNode.Rename( value ); }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return dataNode.ContextMenuStrip; }
        }

        public void InitializeView()
        {
            if ( !dataNode.IsViewInitialized )
            {
                // We simply need to clear the nodes here,
                // since the events we passed over will already
                // add the nodes to this node, hence why we don't need
                // to add them again. (commenting because I did
                // this mistake and I was like why is it so slow
                // afjaşksfşalfklaşwfkwaf)
                Nodes.Clear();
                dataNode.InitializeView();

                if ( Nodes.Count >= 1 && string.IsNullOrEmpty( Nodes[ 0 ].Name ) )
                    Nodes.RemoveAt( 0 );
            }
        }

        private void OnNameChanged( object sender, DataNodeNameChangedEventArgs e )
        {
            base.Name = base.Text = dataNode.Name;
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
            var child = Nodes.OfType<DataTreeNode>().FirstOrDefault( x => x.dataNode == e.ChildNode );

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
            if ( !imageList.Images.ContainsKey( imageKey ) )
                imageList.Images.Add( imageKey, dataNode.Icon );

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

            this.dataNode = dataNode;

            // Make it look like we have a child node
            // if we are branch
            if ( dataNode.Flags.HasFlag( DataNodeFlags.Branch ) )
                Nodes.Add( new TreeNode() );
        }
    }
}
