using System.Linq;
using System.Windows.Forms;
using MikuMikuLibrary.Archives;
using MikuMikuModel.Nodes;
using MikuMikuModel.Nodes.Archives;
using MikuMikuModel.Nodes.Wrappers;

namespace MikuMikuModel.GUI.Forms
{
    public partial class FarcArchiveViewForm<T> : Form where T : class
    {
        private readonly FarcArchive mFarcArchive;
        private readonly FarcArchiveNode mRootNode;

        public INode SelectedNode
        {
            get
            {
                var node = ( mNodeTreeView.SelectedDataNode as ReferenceNode )?.Node;
                node?.Populate();
                return node;
            }
        }

        public int NodeCount => mNodeTreeView.Nodes.Count;

        public INode TopNode
        {
            get
            {
                var node = ( mNodeTreeView.TopDataNode as ReferenceNode )?.Node;
                node?.Populate();
                return node;
            }
        }

        protected override void OnFormClosing( FormClosingEventArgs e )
        {
            if ( DialogResult == DialogResult.OK && mNodeTreeView.SelectedNode == null )
            {
                MessageBox.Show( "Please select a node.", Program.Name, MessageBoxButtons.OK,
                    MessageBoxIcon.Error );

                e.Cancel = true;
            }

            base.OnFormClosing( e );
        }

        private void OnNodeMouseDoubleClick( object sender, TreeNodeMouseClickEventArgs e )
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                mComponents?.Dispose();
                mFarcArchive?.Dispose();
                mRootNode?.Dispose();
                mNodeTreeView.TopNode?.Dispose();
            }

            base.Dispose( disposing );
        }

        public FarcArchiveViewForm( FarcArchive farcArchive )
        {
            InitializeComponent();

            mFarcArchive = farcArchive;
            mRootNode = new FarcArchiveNode( "FARC Archive", mFarcArchive );
            mRootNode.Populate();

            foreach ( var node in mRootNode.Nodes.Where( x => x.DataType == typeof( T ) ) )
                mNodeTreeView.Nodes.Add( new NodeAsTreeNode( new ReferenceNode( node ), true )
                    { HideContextMenuStrip = true } );
        }
    }
}