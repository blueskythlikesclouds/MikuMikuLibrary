using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MikuMikuModel.Nodes.Collections;

namespace MikuMikuModel.Nodes.Wrappers
{
    public class NodeTreeView : TreeView
    {
        private bool mIsExpanding;

        protected override CreateParams CreateParams
        {
            get
            {
                var createParams = base.CreateParams;
                createParams.ExStyle |= 0x02000000;
                return createParams;
            }
        }

        public new NodeAsTreeNode SelectedNode
        {
            get => base.SelectedNode as NodeAsTreeNode;
            set => base.SelectedNode = value;
        }

        public NodeAsTreeNode RootNode => Nodes.Count != 0 ? ( NodeAsTreeNode ) Nodes[ 0 ] : null;
        public INode RootDataNode => RootNode?.Node;
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
            if ( !mIsExpanding )
                PopulateNode( e.Node as NodeAsTreeNode );
            base.OnAfterSelect( e );
        }

        protected override void OnAfterExpand( TreeViewEventArgs e )
        {
            mIsExpanding = true;

            var topNode = TopNode;
            var node = e.Node as NodeAsTreeNode;

            BeginUpdate();
            PopulateNode( node );

            for ( int i = 0; i < e.Node.Nodes.Count; )
            {
                if ( e.Node.Nodes[ i ] is NodeAsTreeNode )
                    i++;

                else
                    e.Node.Nodes.RemoveAt( i );
            }

            if ( node != null && e.Node.Nodes.Count == 1 )
            {
                if ( e.Node.Nodes[ 0 ] is NodeAsTreeNode firstNode )
                {
                    var type = firstNode.Node.GetType();

                    if ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof( ListNode<> ) )
                        firstNode.Expand();
                }
            }

            SelectedNode = node;
            TopNode = topNode;

            EndUpdate();

            base.OnAfterExpand( e );

            mIsExpanding = false;
        }

        protected override void OnHandleCreated( EventArgs e )
        {
            SendMessage( Handle, TVM_SETEXTENDEDSTYLE, ( IntPtr ) TVS_EX_DOUBLEBUFFER, ( IntPtr ) TVS_EX_DOUBLEBUFFER );
            base.OnHandleCreated( e );
        }

        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;

        [DllImport( "user32.dll" )]
        private static extern IntPtr SendMessage( IntPtr hWnd, int msg, IntPtr wp, IntPtr lp );

        public NodeTreeView()
        {
            ImageList = NodeAsTreeNode.ImageList;
            HideSelection = false;
        }
    }
}