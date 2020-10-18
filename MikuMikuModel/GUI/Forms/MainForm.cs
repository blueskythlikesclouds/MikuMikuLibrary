using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MikuMikuLibrary.Hashes;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Motions;
using MikuMikuModel.Configurations;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Mementos;
using MikuMikuModel.Modules;
using MikuMikuModel.Nodes;
using MikuMikuModel.Nodes.Archives;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Wrappers;
using MikuMikuModel.Resources;
using MikuMikuModel.Resources.Styles;
using Ookii.Dialogs.WinForms;

namespace MikuMikuModel.GUI.Forms
{
    public partial class MainForm : Form
    {
        private readonly StringBuilder mStringBuilder = new StringBuilder();

        private Control mControl;

        private string mCurrentlyOpenFilePath;

        private void SetTitle()
        {
            mStringBuilder.Clear();
            mStringBuilder.Append( Program.Name );
            mStringBuilder.AppendFormat( " (v{0})", Program.Version );

#if DEBUG
            mStringBuilder.Append( " (Debug)" );
#endif
            if ( !string.IsNullOrEmpty( mCurrentlyOpenFilePath ) )
            {
                mStringBuilder.AppendFormat( " - {0}", Path.GetFileName( mCurrentlyOpenFilePath ) );

                if ( mNodeTreeView.TopDataNode is IDirtyNode dirtyNode && dirtyNode.IsDirty )
                    mStringBuilder.Append( '*' );
            }

            mStringBuilder.AppendFormat( " - {0}",
                ConfigurationList.Instance.CurrentConfiguration?.Name ?? "No configuration" );

            Text = mStringBuilder.ToString();
        }

        private void SetSplitContainerControl( Control control )
        {
            mMainSplitContainer.Panel1.Controls.Clear();

            if ( control == null )
                return;

            mControl = control;
            mControl.Dock = DockStyle.Fill;
            mMainSplitContainer.Panel1.Controls.Add( mControl );
        }

        private void RefreshNodeControls()
        {
            // Refresh the node control every time a change is made
            SetSplitContainerControl( mNodeTreeView.ControlOfSelectedDataNode );

            // Refresh the property grid
            mPropertyGrid.Refresh();
        }

        private void SetSubscription( INode node, bool unsubscribe = false )
        {
            if ( unsubscribe )
            {
                node.Added -= OnNodeAdded;
                node.Removed -= OnNodeRemoved;
                node.Replaced -= OnNodeReplaced;

                if ( node is IDirtyNode dirtyNode )
                    dirtyNode.DirtyStateChanged -= OnDirtyStateChanged;

                foreach ( var childNode in node.Nodes )
                    SetSubscription( childNode, true );
            }
            else
            {
                node.Added += OnNodeAdded;
                node.Removed += OnNodeRemoved;
                node.Replaced += OnNodeReplaced;

                if ( node is IDirtyNode dirtyNode )
                    dirtyNode.DirtyStateChanged += OnDirtyStateChanged;
            }

            void OnNodeAdded( object sender, NodeAddEventArgs args ) =>
                SetSubscription( args.AddedNode );

            void OnNodeRemoved( object sender, NodeRemoveEventArgs args ) =>
                SetSubscription( args.RemovedNode, true );

            void OnNodeReplaced( object sender, NodeReplaceEventArgs args ) =>
                RefreshNodeControls();

            void OnDirtyStateChanged( object sender, EventArgs args ) =>
                SetTitle();
        }

        private void OnNodeExported( object sender, NodeExportEventArgs e )
        {
            ConfigurationList.Instance.CurrentConfiguration?.SaveTextureDatabase();
            mCurrentlyOpenFilePath = e.FilePath;
            FileHistory.Add( e.FilePath );
            SetTitle();
        }

        public void Reset()
        {
            if ( mNodeTreeView.TopNode != null )
            {
                SetSubscription( mNodeTreeView.TopDataNode, true );

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

            MementoStack.Clear();
            ModelViewControl.ResetInstance();
            TextureViewControl.ResetInstance();

            SetTitle();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void OpenFile( string filePath )
        {
            Enabled = false;

            try
            {
#if DEBUG
                var node = NodeFactory.Create( filePath );
#else
                INode node = null;
                string exceptionMessage = null;

                try
                {
                    node = NodeFactory.Create( filePath );
                }
                catch ( Exception exception )
                {
                    exceptionMessage = exception.Message;
                }

                if ( node != null && !typeof( IBinaryFile ).IsAssignableFrom( node.DataType ) )
                    exceptionMessage = "File type could not be determined.";

                if ( node == null || !string.IsNullOrEmpty( exceptionMessage ) )
                {
                    MessageBox.Show( $"Failed to open {Path.GetFileName( filePath )}.\nReason: {exceptionMessage}", Program.Name, MessageBoxButtons.OK,
                        MessageBoxIcon.Error );

                    Enabled = true;
                    return;
                }
#endif

                Reset();

                SetSubscription( node );

                node.Exported += OnNodeExported;

                var treeNode = new NodeAsTreeNode( node );
                {
                    mNodeTreeView.Nodes.Add( treeNode );
                }

                treeNode.Expand();

                if ( node is FarcArchiveNode && node.Nodes.Count > 0 )
                    mNodeTreeView.SelectedNode = treeNode.Nodes[ 0 ] as NodeAsTreeNode;

                mCurrentlyOpenFilePath = filePath;
                mSaveToolStripMenuItem.Enabled = true;
                mSaveAsToolStripMenuItem.Enabled = true;
                mCloseToolStripMenuItem.Enabled = true;

                FileHistory.Add( filePath );

                SetTitle();
            }

            finally
            {
                Enabled = true;
            }
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
                        typeof( IBinaryFile ).IsAssignableFrom( x ) && x.IsClass && !x.IsAbstract && NodeFactory.NodeTypes.ContainsKey( x ) ),
                    FormatExtensionFlags.Import );
                dialog.Title = "Select a file to open.";
                dialog.ValidateNames = true;
                dialog.AddExtension = true;

                if ( dialog.ShowDialog() == DialogResult.OK )
                    OpenFile( dialog.FileName );
            }
        }

        private void OnOpen( object sender, EventArgs e )
        {
            OpenFile();
        }

        private bool SaveFileAs()
        {
            string path = mNodeTreeView.TopDataNode?.Export();
            if ( string.IsNullOrEmpty( path ) )
                return false;

            mCurrentlyOpenFilePath = path;
            return true;
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

        private void OnSave( object sender, EventArgs e )
        {
            SaveFile();
        }

        private void OnSaveAs( object sender, EventArgs e )
        {
            SaveFileAs();
        }

        /// <summary>
        ///     Returns true when cancel is selected
        /// </summary>
        private bool AskForSavingChanges()
        {
            if ( mNodeTreeView.TopDataNode == null || !( ( IDirtyNode ) mNodeTreeView.TopDataNode ).IsDirty )
                return false;

            var result = MessageBox.Show( "You have unsaved changes. Do you want to save them?", Program.Name, MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question );

            if ( result == DialogResult.Cancel )
                return true;

            if ( result == DialogResult.Yes )
                SaveFile();

            return false;
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

        private void OnFileDropDownOpening( object sender, EventArgs e )
        {
            // Populate the Open Recent menu according to the file history.

            mOpenRecentToolStripMenuItem.DropDownItems.Clear();

            if ( !( mOpenRecentToolStripMenuItem.Enabled = FileHistory.Files.Count > 0 ) )
                return;

            int index = 0;

            foreach ( string filePath in FileHistory.Files.Reverse() )
            {
                var menuItem = new ToolStripMenuItem
                {
                    Text = $"{++index}. {filePath}",
                    Tag = filePath
                };

                menuItem.Click += OnOpenRecentDropDownItemClicked;

                mOpenRecentToolStripMenuItem.DropDownItems.Add( menuItem );
            }
        }

        private void OnOpenRecentDropDownItemClicked( object sender, EventArgs e )
        {
            if ( !( sender is ToolStripMenuItem menuItem ) || !( menuItem.Tag is string filePath ) )
                return;

            OpenFile( filePath );
        }

        private void OnNodeClose( object sender, EventArgs e )
        {
            CloseFile();
        }

        private void OnExit( object sender, EventArgs e )
        {
            Close();
        }

        private void OnEditDropDownOpening( object sender, EventArgs e )
        {
            mUndoToolStripMenuItem.Enabled = MementoStack.IsPendingUndo;
            mRedoToolStripMenuItem.Enabled = MementoStack.IsPendingRedo;
        }

        private void OnEditDropDownClosed( object sender, EventArgs e )
        {
            // Have them enabled for shortcut keys to work
            mUndoToolStripMenuItem.Enabled = true;
            mRedoToolStripMenuItem.Enabled = true;
        }

        private void OnUndo( object sender, EventArgs e )
        {
            MementoStack.Undo();
        }

        private void OnRedo( object sender, EventArgs e )
        {
            MementoStack.Redo();
        }

        private void OnConfigurations( object sender, EventArgs e )
        {
            using ( var configurationsForm = new ConfigurationForm() )
                configurationsForm.ShowDialog( this );
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
                MessageBox.Show( "Could not find suitable configuration for the file.", Program.Name,
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

        private void OnGenerateMurmurHashes( object sender, EventArgs e )
        {
            while ( true )
            {
                using ( var inputDialog = new InputDialog() { WindowTitle = "Enter input" } )
                {
                    if ( inputDialog.ShowDialog() != DialogResult.OK )
                        break;

                    uint hash = MurmurHash.Calculate( inputDialog.Input );

                    MessageBox.Show( $"{inputDialog.Input}: 0x{hash:X8} ({hash})", Program.Name,
                        MessageBoxButtons.OK, MessageBoxIcon.None );
                }
            }
        }

        private void OnUserGuide( object sender, EventArgs e )
        {
            Process.Start( "https://github.com/blueskythlikesclouds/MikuMikuLibrary/wiki/Miku-Miku-Model" );
        }

        private void CheckForUpdates( bool notifyOnFail )
        {
#if DEBUG
            return;
#endif

            try
            {
                using ( var client = new WebClient() )
                {
                    client.Headers.Add( "user-agent", Program.Name );
                    string response = client.DownloadString( "https://api.github.com/repos/blueskythlikesclouds/MikuMikuLibrary/releases/latest" );

                    // yes this is pure crackheadery
                    // no I won't use a json library

                    int index = response.IndexOf( "tag_name", StringComparison.OrdinalIgnoreCase );

                    int firstIndex = response.IndexOf( ':', index + 8 );
                    int lastIndex = response.IndexOf( ',', firstIndex + 1 );

                    string tagName = Program.FixUpVersionString( response.Substring( firstIndex + 1, lastIndex - firstIndex - 1 ).Trim( '"', ',', 'v', ' ' ) );

                    if ( tagName != Program.Version )
                    {
                        Invoke( new Action( () =>
                        {
                            if ( MessageBox.Show( "There's an update available! Do you want to go to the releases page?", Program.Name, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question ) != DialogResult.Yes )
                                return;

                            Process.Start( "https://github.com/blueskythlikesclouds/MikuMikuLibrary/releases" );
                        } ) );
                    }

                    else if ( notifyOnFail )
                    {
                        Invoke( new Action( () =>
                        {
                            MessageBox.Show( "There are no updates available.", Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Information );
                        } ) );
                    }
                }
            }

            catch
            {
                if ( !notifyOnFail )
                    return;

                Invoke( new Action( () =>
                {
                    MessageBox.Show( "Failed to check for updates.", Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error );
                } ) );
            }
        }

        private void OnCheckForUpdates( object sender, EventArgs e )
        {
            CheckForUpdates( true );
        }

        private void OnAbout( object sender, EventArgs e )
        {
            mStringBuilder.Clear();
            mStringBuilder.AppendLine( Program.Name );
            mStringBuilder.AppendFormat( "Version: v{0}", Program.Version );
#if DEBUG
            mStringBuilder.Append( " - Debug" );
#endif
            mStringBuilder.AppendLine();
            mStringBuilder.AppendLine( "This program was created by Skyth." );
            mStringBuilder.Append( "Please see README.md, LICENSE.md and COPYRIGHT.md for additional information." );

            MessageBox.Show( mStringBuilder.ToString(), "About", MessageBoxButtons.OK );
        }

        private void OnAfterSelect( object sender, TreeViewEventArgs e )
        {
            MementoStack.EndCompoundMemento();

            var type = mNodeTreeView.SelectedDataNode.GetType();

            if ( mNodeTreeView.SelectedDataNode is ReferenceNode referenceNode )
                mPropertyGrid.SelectedObject = referenceNode.Node;

#if !DEBUG
            else if ( type.IsGenericType && typeof( ListNode<> ).IsAssignableFrom( type.GetGenericTypeDefinition() ) )
            {
                mPropertyGrid.SelectedObjects = mNodeTreeView.SelectedDataNode.Nodes.ToArray();

                MementoStack.BeginCompoundMemento();
            }
#endif
            else
                mPropertyGrid.SelectedObject = mNodeTreeView.SelectedDataNode;

            // Set the control on the left to the node's control
            SetSplitContainerControl( mNodeTreeView.ControlOfSelectedDataNode );
        }

        private void OnPropertyValueChanged( object s, PropertyValueChangedEventArgs e )
        {
            MementoStack.EndCompoundMemento();
            MementoStack.BeginCompoundMemento();
        }

        private void OnAutoCheckUpdates( object sender, EventArgs e )
        {
            ValueCache.Set( "AutoCheckUpdates", mAutoCheckUpdatesToolStripMenuItem.Checked );
        }

        private void OnStyleChanged( object sender, StyleChangedEventArgs eventArgs )
        {
            StyleHelpers.ApplyStyle( this, eventArgs.Style );

            Refresh();
        }

        private void InitializeStylesToolStripMenuItem()
        {
            AddMenuItem( "Default", () => StyleSet.CurrentStyle = null, StyleSet.CurrentStyle == null );

            if ( StyleSet.Styles.Count != 0 )
                mStylesToolStripMenuItem.DropDownItems.Add( new ToolStripSeparator() );

            foreach ( var style in StyleSet.Styles )
                AddMenuItem( style.Name, () => StyleSet.CurrentStyle = style, StyleSet.CurrentStyle == style );

            void AddMenuItem( string text, Action onClick, bool isChecked )
            {
                var toolStripMenuItem = new ToolStripMenuItem { Text = text, CheckOnClick = true, Checked = isChecked };

                toolStripMenuItem.Click += ( sender, args ) =>
                {
                    onClick();

                    foreach ( var item in mStylesToolStripMenuItem.DropDownItems )
                    {
                        if ( item is ToolStripMenuItem menuItem )
                            menuItem.Checked = menuItem == toolStripMenuItem;
                    }
                };

                mStylesToolStripMenuItem.DropDownItems.Add( toolStripMenuItem );
            }
        }

        private void UpdateCameraModeFlags()
        {
            mOrbitCameraToolStripMenuItem.Checked = ModelViewControl.UseOrbitCamera;
            mFreeCameraToolStripMenuItem.Checked = !ModelViewControl.UseOrbitCamera;
        }

        private void OnCameraModeClicked( object sender, EventArgs e )
        {
            ModelViewControl.UseOrbitCamera = sender == mOrbitCameraToolStripMenuItem;
            ValueCache.Set( "UseOrbitCamera", ModelViewControl.UseOrbitCamera );
            UpdateCameraModeFlags();
        }

        protected override void OnLoad( EventArgs eventArgs )
        {
            if ( Type.GetType( "Mono.Runtime" ) == null )
            {
                StyleHelpers.StoreDefaultStyle( this );

                if ( StyleSet.CurrentStyle != null )
                    StyleHelpers.ApplyStyle( this, StyleSet.CurrentStyle );

                StyleSet.StyleChanged += OnStyleChanged;

                InitializeStylesToolStripMenuItem();
            }

            if ( !ValueCache.Get<bool>( "IsNotFirstLaunch" ) )
            {
                using ( var firstLaunchForm = new FirstLaunchForm() )
                    firstLaunchForm.ShowDialog( this );

                ValueCache.Set( "IsNotFirstLaunch", true );
            }

            ModelViewControl.UseOrbitCamera = ValueCache.Get( "UseOrbitCamera", true );
            UpdateCameraModeFlags();

            mAutoCheckUpdatesToolStripMenuItem.Checked = ValueCache.Get( "AutoCheckUpdates", true );

            if ( mAutoCheckUpdatesToolStripMenuItem.Checked )
                new Thread( () => CheckForUpdates( false ) ).Start();

            base.OnLoad( eventArgs );
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
            {
                if ( CheckKeyPressRecursively( subItem, keys ) )
                    return true;
            }

            return false;
        }

        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            if ( mControl != null && mControl.Focused )
                return false;

            foreach ( var menuItem in mMenuStrip.Items )
            {
                if ( CheckKeyPressRecursively( menuItem, keyData ) )
                    return true;
            }

            var strip = mNodeTreeView.SelectedNode?.ContextMenuStrip;

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

            Icon = ResourceStore.LoadIcon( "Icons/Application.ico" );

            SetTitle();
            Select();

#if DEBUG
            mPropertyGrid.BrowsableAttributes = new AttributeCollection();
#endif
        }
    }
}