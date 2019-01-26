using MikuMikuLibrary.IO;
using MikuMikuModel.Configurations;
using MikuMikuModel.DataNodes;
using MikuMikuModel.DataNodes.Wrappers;
using MikuMikuModel.FormatModules;
using MikuMikuModel.GUI.Controls;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace MikuMikuModel.GUI.Forms
{
    public partial class MainForm : Form
    {
        private string mCurrentlyOpenFilePath;

        private void OnOpen( object sender, EventArgs e ) => OpenFile();
        private void OnSave( object sender, EventArgs e ) => SaveFile();
        private void OnSaveAs( object sender, EventArgs e ) => SaveFileAs();
        private void OnExit( object sender, EventArgs e ) => Close();

        private void OnAfterSelect( object sender, TreeViewEventArgs e )
        {
            if ( mTreeView.SelectedDataNode is ReferenceNode referenceNode )
                mPropertyGrid.SelectedObject = referenceNode.Reference;
            else
                mPropertyGrid.SelectedObject = mTreeView.SelectedDataNode;

            // Set the control on the left to the node's control
            mMainSplitContainer.Panel1.Controls.Clear();

            Control control;
            if ( ( control = mTreeView.ControlOfSelectedDataNode ) != null )
            {
                control.Dock = DockStyle.Fill;
                mMainSplitContainer.Panel1.Controls.Add( control );
            }
        }

        private bool CheckKeyPressRecursively( object item, Keys keys )
        {
            if ( item is ToolStripMenuItem menuItem )
            {
                if ( menuItem.ShortcutKeys == keys )
                {
                    menuItem.PerformClick();
                    return true;
                }
                else
                {
                    foreach ( var subItem in menuItem.DropDownItems )
                    {
                        if ( CheckKeyPressRecursively( subItem, keys ) )
                            return true;
                    }
                }
            }

            return false;
        }

        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            foreach ( var menuItem in mMenuStrip.Items )
            {
                if ( CheckKeyPressRecursively( menuItem, keyData ) )
                    return true;
            }

            var strip = mTreeView.SelectedNode?.ContextMenuStrip;
            if ( strip != null )
            {
                foreach ( var menuItem in strip.Items )
                {
                    if ( CheckKeyPressRecursively( menuItem, keyData ) )
                        return true;
                }
            }

            return base.ProcessCmdKey( ref msg, keyData );
        }

        protected override void OnFormClosing( FormClosingEventArgs e )
        {
            e.Cancel = AskForSavingChanges();
            base.OnFormClosing( e );
        }

        protected override void OnFormClosed( FormClosedEventArgs e )
        {
            ConfigurationList.Instance.Save();
            base.OnFormClosed( e );
        }

        protected override void OnDragEnter( DragEventArgs drgevent )
        {
            if ( drgevent.Data.GetDataPresent( DataFormats.FileDrop ) )
                drgevent.Effect = DragDropEffects.Copy;
            else
                drgevent.Effect = DragDropEffects.None;

            base.OnDragEnter( drgevent );
        }

        protected override void OnDragDrop( DragEventArgs drgevent )
        {
            string[] filePaths = ( string[] )drgevent.Data.GetData( DataFormats.FileDrop, false );
            if ( filePaths.Length >= 1 && !AskForSavingChanges() )
                OpenFile( filePaths[ 0 ] );

            base.OnDragDrop( drgevent );
        }

        private void OnNodeClose( object sender, EventArgs e ) => CloseFile();

        public void Reset()
        {
            mTreeView.TopDataNode?.Dispose();
            mTreeView.Nodes.Clear();

            mPropertyGrid.SelectedObject = null;

            mMainSplitContainer.Panel1.Controls.Clear();

            mSaveToolStripMenuItem.Enabled = false;
            mSaveAsToolStripMenuItem.Enabled = false;
            mCloseToolStripMenuItem.Enabled = false;

            Text = $"Miku Miku Model";

            mCurrentlyOpenFilePath = null;
        }

        public void OpenFile()
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = FormatModuleUtilities.GetFilter(
                    FormatModuleRegistry.ModelTypes.Where( x => typeof( IBinaryFile ).IsAssignableFrom( x ) && x.IsClass && !x.IsAbstract ), FormatModuleFlags.Import );
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

            // Determine current configuration
            ConfigurationList.Instance.DetermineCurrentConfiguration( filePath );

            // Create the node
            var node = DataNodeFactory.Create( filePath );

            // Wrap the node and add it to the tree view
            var wrappedNode = new DataTreeNode( node );
            mTreeView.Nodes.Add( wrappedNode );

            // Restore menu items
            mSaveToolStripMenuItem.Enabled = true;
            mSaveAsToolStripMenuItem.Enabled = true;
            mCloseToolStripMenuItem.Enabled = true;

            // Update the title to have the name of node
            Text = $"Miku Miku Model - {node.Name}";

            // Update the file path for the save method
            mCurrentlyOpenFilePath = filePath;

            // Expand the node
            wrappedNode.Expand();
        }

        private void SaveFile( string filePath )
        {
            if ( mTreeView.TopDataNode == null )
                return;

            mTreeView.TopDataNode.Export( filePath );

            // Save the texture database
            ConfigurationList.Instance.CurrentConfiguration?.TextureDatabase?.Save(
                ConfigurationList.Instance.CurrentConfiguration?.TextureDatabaseFilePath );

            mCurrentlyOpenFilePath = filePath;
        }

        private bool SaveFile()
        {
            if ( mTreeView.TopDataNode == null )
                return false;

            if ( !string.IsNullOrEmpty( mCurrentlyOpenFilePath ) )
            {
                SaveFile( mCurrentlyOpenFilePath );
                return true;
            }

            return SaveFileAs();
        }

        private bool SaveFileAs()
        {
            var path = mTreeView.TopDataNode?.Export();
            if ( string.IsNullOrEmpty( path ) )
                return false;

            mCurrentlyOpenFilePath = path;
            return true;
        }

        /// <summary>
        /// Returns true when cancel is selected
        /// </summary>
        private bool AskForSavingChanges()
        {
            if ( mTreeView.TopDataNode == null || !mTreeView.TopDataNode.HadAnyChanges )
                return false;

            var result = MessageBox.Show(
                "You have unsaved changes. Do you want to save them?", "Miku Miku Model", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question );

            if ( result == DialogResult.Cancel )
                return true;

            if ( result == DialogResult.OK )
                SaveFile();

            return false;
        }

        /// <summary>
        /// Returns true if file was closed
        /// </summary>
        private bool CloseFile()
        {
            if ( AskForSavingChanges() )
                return false;

            Reset();
            return true;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                mComponents?.Dispose();
                ModelViewControl.DisposeInstance();
                TextureViewControl.DisposeInstance();
            }
            base.Dispose( disposing );
        }

        public MainForm()
        {
            InitializeComponent();

            // To prevent the seperator from getting selected
            // It's something that really annoys me lol
            Select();
        }

        private void OnConfigurations( object sender, EventArgs e )
        {
            using ( var configurationsForm = new ConfigurationForm() )
                configurationsForm.ShowDialog( this );
        }

        private void OnAbout( object sender, EventArgs e )
        {
            MessageBox.Show( "MikuMikuModel by Skyth\nThis program is a work in progress." );
        }

        private void OnHelp( object sender, EventArgs e )
        {
            Process.Start( "https://github.com/blueskythlikesclouds/MikuMikuLibrary/wiki/Miku-Miku-Model" );
        }

        private void OnPropertyValueChanged( object s, PropertyValueChangedEventArgs e )
        {
            mTreeView.SelectedDataNode?.NotifyPropertyChanged( e.ChangedItem.Label );
        }
    }
}
