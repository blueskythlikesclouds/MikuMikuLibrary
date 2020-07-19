using System;
using System.ComponentModel;
using System.Windows.Forms;
using MikuMikuLibrary.Materials;
using MikuMikuModel.Nodes;
using MikuMikuModel.Nodes.Textures;
using MikuMikuModel.Nodes.Wrappers;
using MikuMikuModel.Resources;
using MikuMikuModel.Resources.Styles;

namespace MikuMikuModel.GUI.Forms
{
    public partial class TextureSelectForm : Form
    {
        public TextureNode SelectedTextureNode =>
            mNodeTreeView.SelectedDataNode as TextureNode;

        public MaterialTextureType MaterialTextureType =>
            ( MaterialTextureType ) mMaterialTextureTypeComboBox.SelectedIndex;

        private void OnNodeMouseDoubleClick( object sender, TreeNodeMouseClickEventArgs e )
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OnAfterSelect( object sender, TreeViewEventArgs e )
        {
            mMainSplitContainer.Panel1.Controls.Clear();

            var control = mNodeTreeView.ControlOfSelectedDataNode;
            if ( control == null )
                return;

            control.Dock = DockStyle.Fill;
            mMainSplitContainer.Panel1.Controls.Add( control );
        }

        protected override void OnClosing( CancelEventArgs e )
        {
            if ( DialogResult == DialogResult.OK && !( mNodeTreeView.SelectedNode?.Node is TextureNode ) )
            {
                MessageBox.Show( "Please select a texture.", Program.Name, MessageBoxButtons.OK,
                    MessageBoxIcon.Error );

                e.Cancel = true;
            }

            base.OnClosing( e );
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                mMainSplitContainer.Panel1.Controls.Clear(); // why tf you disposing texture view control bruh
                mNodeTreeView.TopNode?.Dispose();
                mNodeTreeView.TopDataNode?.Dispose();
                components?.Dispose();
            }

            base.Dispose( disposing );
        }

        public TextureSelectForm( INode textureSetNode )
        {
            InitializeComponent();

            if ( StyleSet.CurrentStyle != null )
                StyleHelpers.ApplyStyle( this, StyleSet.CurrentStyle );

            Icon = ResourceStore.LoadIcon( "Icons/Application.ico" );

            var rootNode = new ReferenceNode( textureSetNode );

            var nodeAsTreeNode = new NodeAsTreeNode( rootNode );
            mNodeTreeView.Nodes.Add( nodeAsTreeNode );

            nodeAsTreeNode.Expand();
            nodeAsTreeNode.Nodes[ 0 ].Expand();
        }

        public TextureSelectForm( INode textureSetNode, MaterialTextureType type ) : this( textureSetNode )
        {
            mMaterialTextureTypeLabel.Visible = true;
            mMaterialTextureTypeComboBox.Visible = true;

            foreach ( string typeName in Enum.GetNames( typeof( MaterialTextureType ) ) )
                mMaterialTextureTypeComboBox.Items.Add( typeName );

            mMaterialTextureTypeComboBox.SelectedIndex = ( int ) type;
        }
    }
}