using MikuMikuLibrary.IO;
using MikuMikuModel.Configurations;
using MikuMikuModel.DataNodes;
using MikuMikuModel.DataNodes.Wrappers;
using MikuMikuModel.FormatModules;
using MikuMikuModel.GUI.Controls;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace MikuMikuModel.GUI.Forms
{
    public partial class MainForm : Form
    {
        private string currentlyOpenFilePath;

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

        private void OnExit( object sender, EventArgs e )
        {
            Close();
        }

        private void OnAfterSelect( object sender, TreeViewEventArgs e )
        {
            // Set the property grid's selected object to the tag, which is a IDataNode
            propertyGrid.SelectedObject = treeView.SelectedDataNode.Data;

            // Set the control on the left to the node's control
            mainSplitContainer.Panel1.Controls.Clear();

            Control control;
            if ( ( control = treeView.ControlOfSelectedDataNode ) != null )
            {
                control.Dock = DockStyle.Fill;
                mainSplitContainer.Panel1.Controls.Add( control );
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
            foreach ( var menuItem in menuStrip.Items )
            {
                if ( CheckKeyPressRecursively( menuItem, keyData ) )
                    return true;
            }

            var strip = treeView.SelectedNode?.ContextMenuStrip;
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

        private void OnNodeClose( object sender, EventArgs e )
        {
            CloseFile();
        }

        public void Reset()
        {
            treeView.TopDataNode?.Dispose();
            treeView.Nodes.Clear();

            propertyGrid.SelectedObject = null;

            mainSplitContainer.Panel1.Controls.Clear();

            saveToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            closeToolStripMenuItem.Enabled = false;

            Text = $"Miku Miku Model";

            currentlyOpenFilePath = null;
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
            treeView.Nodes.Add( wrappedNode );

            // Restore menu items
            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            closeToolStripMenuItem.Enabled = true;

            // Update the title to have the name of node
            Text = $"Miku Miku Model - {node.Name}";

            // Update the file path for the save method
            currentlyOpenFilePath = filePath;
        }

        private void SaveFile( string filePath )
        {
            if ( treeView.TopDataNode == null )
                return;

            treeView.TopDataNode.Export( filePath );

            // Save the texture database
            ConfigurationList.Instance.CurrentConfiguration?.TextureDatabase?.Save(
                ConfigurationList.Instance.CurrentConfiguration?.TextureDatabaseFilePath );

            currentlyOpenFilePath = filePath;
        }

        private bool SaveFile()
        {
            if ( treeView.TopDataNode == null )
                return false;

            if ( !string.IsNullOrEmpty( currentlyOpenFilePath ) )
            {
                SaveFile( currentlyOpenFilePath );
                return true;
            }

            return SaveFileAs();
        }

        private bool SaveFileAs()
        {
            var path = treeView.TopDataNode?.Export();
            if ( string.IsNullOrEmpty( path ) )
                return false;

            currentlyOpenFilePath = path;
            return true;
        }

        /// <summary>
        /// Returns true when cancel is selected
        /// </summary>
        private bool AskForSavingChanges()
        {
            if ( treeView.TopDataNode == null || !treeView.TopDataNode.HasPendingChanges )
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
                components?.Dispose();
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

        private void OnHelp(object sender, EventArgs e)
        {
            Process.Start("https://github.com/blueskythlikesclouds/MikuMikuLibrary/wiki/Miku-Miku-Model");
        }

        private void OnPropertyValueChanged( object s, PropertyValueChangedEventArgs e )
        {
            treeView.SelectedDataNode?.NotifyPropertyChanged( e.ChangedItem.Label );
        }
    }
}
