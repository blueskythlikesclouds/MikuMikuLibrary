using System;
using System.Linq;
using System.Windows.Forms;
using MikuMikuLibrary.Archives;
using MikuMikuModel.Nodes;
using MikuMikuModel.Nodes.Archives;
using MikuMikuModel.Nodes.Wrappers;
using MikuMikuModel.Resources;
using MikuMikuModel.Resources.Styles;

namespace MikuMikuModel.GUI.Forms
{
    public partial class NodeSelectForm<T> : Form where T : class
    {
        private readonly ReferenceNode mRootNode;

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
                var node = ( mNodeTreeView.RootDataNode as ReferenceNode )?.Node;
                node?.Populate();
                return node;
            }
        }

        private void OnNodeMouseDoubleClick( object sender, TreeNodeMouseClickEventArgs e )
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void OnFormClosing( FormClosingEventArgs e )
        {
            if ( DialogResult == DialogResult.OK && mNodeTreeView.SelectedNode == null )
            {
                MessageBox.Show( "Please select a node.", Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error );
                e.Cancel = true;
            }

            base.OnFormClosing( e );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                mComponents?.Dispose();
                mRootNode?.Dispose();
                mNodeTreeView.RootNode?.Dispose();
            }

            base.Dispose( disposing );
        }

        public NodeSelectForm( INode rootNode, Func<T, bool> filter = null )
        {
            InitializeComponent();

            if ( StyleSet.CurrentStyle != null )
                StyleHelpers.ApplyStyle( this, StyleSet.CurrentStyle );

            Icon = ResourceStore.LoadIcon( "Icons/Application.ico" );

            mRootNode = new ReferenceNode( rootNode );
            mRootNode.Populate();

            foreach ( var node in mRootNode.Nodes )
            {
                if ( node.DataType != typeof( T ) )
                    continue;

                if ( filter != null && !filter( ( T ) node.Data ) )
                    continue;

                mNodeTreeView.Nodes.Add( new NodeAsTreeNode( new ReferenceNode( node ), true )
                    { HideContextMenuStrip = true } );
            }

            StyleHelpers.ApplySystemFont( this );
        }
    }
}