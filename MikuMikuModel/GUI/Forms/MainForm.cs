using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Motions;
using MikuMikuModel.Configurations;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Modules;
using MikuMikuModel.Nodes;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Misc;
using MikuMikuModel.Nodes.Wrappers;
using MikuMikuModel.Resources.Styles;

namespace MikuMikuModel.GUI.Forms
{
    public partial class MainForm : Form
    {
        private readonly StringBuilder mStringBuilder = new StringBuilder();

        private Control mControl;

        private string mCurrentlyOpenFilePath;

        private OriginalStyle mOriginalStyle;

        /// <summary>
        ///     Returns true when cancel is selected
        /// </summary>
        private bool AskForSavingChanges()
        {
            if ( mNodeTreeView.TopDataNode == null || !( ( IDirtyNode ) mNodeTreeView.TopDataNode ).IsDirty )
                return false;

            var result = MessageBox.Show(
                "You have unsaved changes. Do you want to save them?", "Miku Miku Model", MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question );

            if ( result == DialogResult.Cancel )
                return true;

            if ( result == DialogResult.OK )
                SaveFile();

            return false;
        }

        private static bool CheckKeyPressRecursively( object item, Keys keys )
        {
            if ( !( item is ToolStripMenuItem menuItem ) )
                return false;

            if ( menuItem.ShortcutKeys == keys )
            {
                menuItem.PerformClick();
                return true;
            }

            foreach ( var subItem in menuItem.DropDownItems )
                if ( CheckKeyPressRecursively( subItem, keys ) )
                    return true;

            return false;
        }

        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            if ( mControl != null && mControl.Focused )
                return false;

            foreach ( var menuItem in mMenuStrip.Items )
                if ( CheckKeyPressRecursively( menuItem, keyData ) )
                    return true;

            var strip = mNodeTreeView.SelectedNode?.ContextMenuStrip;
            if ( strip != null )
                foreach ( var menuItem in strip.Items )
                    if ( CheckKeyPressRecursively( menuItem, keyData ) )
                        return true;

            return base.ProcessCmdKey( ref msg, keyData );
        }

        /// <summary>
        ///     Returns true if file was closed
        /// </summary>
        private bool CloseFile()
        {
            if ( AskForSavingChanges() )
                return false;

            Reset();
            return true;
        }

        private void OnAbout( object sender, EventArgs e )
        {
            mStringBuilder.Clear();
            mStringBuilder.AppendLine( Program.Name );
            mStringBuilder.AppendFormat( "Version: {0}", Program.Version );
#if DEBUG
            mStringBuilder.Append( " - Debug" );
#endif
            mStringBuilder.AppendLine();
            mStringBuilder.AppendLine( "This program was created by Skyth." );

            MessageBox.Show( mStringBuilder.ToString(), "About", MessageBoxButtons.OK );
        }

        private void OnAfterSelect( object sender, TreeViewEventArgs e )
        {
            if ( mNodeTreeView.SelectedDataNode is ReferenceNode referenceNode )
                mPropertyGrid.SelectedObject = referenceNode.Node;
            else
                mPropertyGrid.SelectedObject = mNodeTreeView.SelectedDataNode;

            // Set the control on the left to the node's control
            mMainSplitContainer.Panel1.Controls.Clear();

            if ( ( mControl = mNodeTreeView.ControlOfSelectedDataNode ) != null )
            {
                mControl.Dock = DockStyle.Fill;
                mMainSplitContainer.Panel1.Controls.Add( mControl );
            }
        }

        private void OnConfigurations( object sender, EventArgs e )
        {
            using ( var configurationsForm = new ConfigurationForm() )
            {
                configurationsForm.ShowDialog( this );
            }
        }

        protected override void OnDragDrop( DragEventArgs drgevent )
        {
            var filePaths = ( string[] ) drgevent.Data.GetData( DataFormats.FileDrop, false );
            if ( filePaths.Length >= 1 && !AskForSavingChanges() )
                OpenFile( filePaths[ 0 ] );

            base.OnDragDrop( drgevent );
        }

        protected override void OnDragEnter( DragEventArgs drgevent )
        {
            drgevent.Effect = drgevent.Data.GetDataPresent( DataFormats.FileDrop )
                ? DragDropEffects.Copy
                : DragDropEffects.None;

            base.OnDragEnter( drgevent );
        }

        private void OnExit( object sender, EventArgs e )
        {
            Close();
        }

        protected override void OnFormClosing( FormClosingEventArgs e )
        {
            e.Cancel = AskForSavingChanges();
            base.OnFormClosing( e );
        }

        private void OnHelp( object sender, EventArgs e )
        {
            Process.Start( "https://github.com/blueskythlikesclouds/MikuMikuLibrary/wiki/Miku-Miku-Model" );
        }

        private void OnNodeClose( object sender, EventArgs e )
        {
            CloseFile();
        }

        private void OnOpen( object sender, EventArgs e )
        {
            OpenFile();
        }

        private void OnSave( object sender, EventArgs e )
        {
            SaveFile();
        }

        private void OnSaveAs( object sender, EventArgs e )
        {
            SaveFileAs();
        }

        public void OpenFile()
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = ModuleFilterGenerator.GenerateFilter(
                    FormatModuleRegistry.ModelTypes.Where( x =>
                        typeof( IBinaryFile ).IsAssignableFrom( x ) && x.IsClass && !x.IsAbstract &&
                        NodeFactory.NodeTypes.ContainsKey( x ) ),
                    FormatModuleFlags.Import );
                dialog.Title = "Select a file to open.";
                dialog.ValidateNames = true;
                dialog.AddExtension = true;

                if ( dialog.ShowDialog() == DialogResult.OK )
                    OpenFile( dialog.FileName );
            }
        }

        public void OpenFile( string filePath )
        {
            Reset();

            bool errorsOccured = false;

            INode node = null;
            try
            {
                node = NodeFactory.Create( filePath );
            }
            catch
            {
                errorsOccured = true;
            }

            if ( errorsOccured || !typeof( IBinaryFile ).IsAssignableFrom( node.DataType ) )
            {
                MessageBox.Show( "File could not be opened.", "Miku Miku Model", MessageBoxButtons.OK,
                    MessageBoxIcon.Error );

                node?.Dispose();
                return;
            }

            node.Exported += OnNodeExported;

            var treeNode = new NodeAsTreeNode( node );
            {
                mNodeTreeView.Nodes.Add( treeNode );
            }
            treeNode.Expand();

            mCurrentlyOpenFilePath = filePath;
            mSaveToolStripMenuItem.Enabled = true;
            mSaveAsToolStripMenuItem.Enabled = true;
            mCloseToolStripMenuItem.Enabled = true;

            SetTitle( Path.GetFileName( filePath ) );
        }

        private static void OnNodeExported( object sender, NodeExportEventArgs e ) =>
            ConfigurationList.Instance.CurrentConfiguration?.SaveTextureDatabase();

        public void Reset()
        {
            if ( mNodeTreeView.TopNode != null )
            {
                mNodeTreeView.TopNode.Dispose();
                mNodeTreeView.TopDataNode.Exported -= OnNodeExported;
                mNodeTreeView.TopDataNode.DisposeData();
                mNodeTreeView.TopDataNode.Dispose();
            }

            mNodeTreeView.Nodes.Clear();

            mPropertyGrid.SelectedObject = null;
            mMainSplitContainer.Panel1.Controls.Clear();
            mSaveToolStripMenuItem.Enabled = false;
            mSaveAsToolStripMenuItem.Enabled = false;
            mCloseToolStripMenuItem.Enabled = false;
            mCurrentlyOpenFilePath = null;

            SetTitle();
        }

        private void SaveFile( string filePath )
        {
            if ( mNodeTreeView.TopDataNode == null )
                return;

            mNodeTreeView.TopDataNode.Export( filePath );
            mCurrentlyOpenFilePath = filePath;
        }

        private bool SaveFile()
        {
            if ( mNodeTreeView.TopDataNode == null )
                return false;

            if ( string.IsNullOrEmpty( mCurrentlyOpenFilePath ) )
                return SaveFileAs();

            SaveFile( mCurrentlyOpenFilePath );
            return true;
        }

        private bool SaveFileAs()
        {
            string path = mNodeTreeView.TopDataNode?.Export();
            if ( string.IsNullOrEmpty( path ) )
                return false;

            mCurrentlyOpenFilePath = path;
            return true;
        }

        private void SetTitle( string fileName = null )
        {
            mStringBuilder.Clear();
            mStringBuilder.Append( Program.Name );
            mStringBuilder.AppendFormat( " ({0})", Program.Version );

#if DEBUG
            mStringBuilder.Append( " (Debug)" );
#endif
            if ( !string.IsNullOrEmpty( fileName ) )
                mStringBuilder.AppendFormat( " - {0}", fileName );

            Text = mStringBuilder.ToString();
        }


        private void OnCombineMotions( object sender, EventArgs e )
        {
            string filePath =
                ModuleImportUtilities.SelectModuleImport<Motion>( "Select the root .mot file." );

            if ( filePath == null )
                return;

            var configuration = ConfigurationList.Instance.FindConfiguration( filePath );
            if ( configuration?.BoneDatabase == null )
            {
                MessageBox.Show( "Could not find suitable configuration for the file.", "Miku Miku Model",
                    MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            else
            {
                string baseFilePath = Path.ChangeExtension( filePath, null );

                string outputFilePath = ModuleExportUtilities.SelectModuleExport<Motion>(
                    "Select a file to export to.",
                    Path.GetFileName( $"{baseFilePath}_combined.mot" ) );

                if ( string.IsNullOrEmpty( outputFilePath ) )
                    return;

                var skeleton = configuration.BoneDatabase.Skeletons[ 0 ];

                var rootMotion = new Motion();
                {
                    rootMotion.Load( filePath, skeleton );
                }

                var rootController = rootMotion.Bind();
                for ( int i = 1;; i++ )
                {
                    string divFilePath = $"{baseFilePath}_div_{i}.mot";
                    if ( !File.Exists( divFilePath ) )
                        break;

                    var divMotion = new Motion();
                    {
                        divMotion.Load( divFilePath, skeleton );
                    }

                    var divController = divMotion.Bind();
                    rootController.Merge( divController );
                }

                rootMotion.Save( outputFilePath, skeleton );
            }
        }

        private void StoreOriginalStyle()
        {
            if ( mOriginalStyle != null )
                return;

            mOriginalStyle = new OriginalStyle
            {
                ForeColor = base.ForeColor,
                BackColor = base.BackColor,

                TreeViewBackColor = mNodeTreeView.BackColor,
                TreeViewForeColor = mNodeTreeView.ForeColor,
                TreeViewLineColor = mNodeTreeView.LineColor,

                PropertyGridLineColor = mPropertyGrid.LineColor,
                PropertyGridSelectedItemWithFocusBackColor = mPropertyGrid.SelectedItemWithFocusBackColor,
                PropertyGridViewBackColor = mPropertyGrid.ViewBackColor,
                PropertyGridViewForeColor = mPropertyGrid.ViewForeColor
            };
        }

        private void RestoreOriginalStyle()
        {
            if ( mOriginalStyle == null )
                return;

            mMenuStrip.Renderer = null;

            base.BackColor = mOriginalStyle.BackColor;
            base.ForeColor = mOriginalStyle.ForeColor;

            mNodeTreeView.BackColor = mOriginalStyle.TreeViewBackColor;
            mNodeTreeView.ForeColor = mOriginalStyle.TreeViewForeColor;
            mNodeTreeView.LineColor = mOriginalStyle.TreeViewLineColor;

            mPropertyGrid.LineColor = mOriginalStyle.PropertyGridLineColor;
            mPropertyGrid.SelectedItemWithFocusBackColor = mOriginalStyle.PropertyGridSelectedItemWithFocusBackColor;
            mPropertyGrid.ViewBackColor = mOriginalStyle.PropertyGridViewBackColor;
            mPropertyGrid.ViewForeColor = mOriginalStyle.PropertyGridViewForeColor;

            Refresh();
        }

        protected override void OnLoad( EventArgs eventArgs )
        {
            StoreOriginalStyle();

            if ( StyleSet.CurrentStyle != null )
                ApplyStyle( StyleSet.CurrentStyle );

            StyleSet.StyleChanged += OnStyleChanged;

            InitializeStylesToolStripMenuItem();

            base.OnLoad( eventArgs );
        }

        private void OnStyleChanged( object sender, StyleChangedEventArgs eventArgs )
        {
            ApplyStyle( eventArgs.Style );
        }

        private void ApplyStyle( Style style )
        {
            if ( style == null )
            {
                RestoreOriginalStyle();
                return;
            }

            mMenuStrip.Renderer = style.ToolStripRenderer;

            base.BackColor = style.Background;
            base.ForeColor = style.Foreground;

            mNodeTreeView.BackColor = style.Background;
            mNodeTreeView.ForeColor = style.Foreground;
            mNodeTreeView.LineColor = style.SeparatorLight;

            mPropertyGrid.LineColor = style.Border;
            mPropertyGrid.SelectedItemWithFocusBackColor = style.Border;
            mPropertyGrid.ViewBackColor = style.Background;
            mPropertyGrid.ViewForeColor = style.Foreground;

            Refresh();
        }

        private void InitializeStylesToolStripMenuItem()
        {
            mStylesToolStripMenuItem.DropDownItems.Add( "Default", null, ( s, e ) => StyleSet.CurrentStyle = null );

            if ( StyleSet.Styles.Count != 0 )
                mStylesToolStripMenuItem.DropDownItems.Add( new ToolStripSeparator() );

            foreach ( var style in StyleSet.Styles )
                mStylesToolStripMenuItem.DropDownItems.Add( style.Name, null,
                    ( s, e ) => StyleSet.CurrentStyle = style );
        }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                mComponents?.Dispose();
                ModelViewControl.DisposeInstance();
                TextureViewControl.DisposeInstance();
                StyleSet.StyleChanged -= OnStyleChanged;
            }

            base.Dispose( disposing );
        }

        public MainForm()
        {
            InitializeComponent();
            SetTitle();
            Select();

#if DEBUG
            mPropertyGrid.BrowsableAttributes = new AttributeCollection();
#endif
        }

        private class OriginalStyle
        {
            public Color BackColor;
            public Color ForeColor;
            public Color TreeViewBackColor;
            public Color TreeViewForeColor;
            public Color TreeViewLineColor;
            public Color PropertyGridLineColor;
            public Color PropertyGridSelectedItemWithFocusBackColor;
            public Color PropertyGridViewBackColor;
            public Color PropertyGridViewForeColor;
        }
    }
}