using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using MikuMikuModel.Configurations;
using MikuMikuModel.Mementos;
using MikuMikuModel.Modules;
using MikuMikuModel.Resources;
using Ookii.Dialogs.WinForms;

namespace MikuMikuModel.Nodes
{
    public delegate void NodeImportHandler( string filePath );

    public delegate void NodeExportHandler( string filePath );

    public delegate T NodeReplaceHandler<out T>( string filePath );

    public abstract partial class Node<T> : INode where T : class
    {
        private static readonly PropertyInfo sNameProperty =
            typeof( T ).GetProperty( "Name", typeof( string ) );

        private static readonly Dictionary<string, PropertyInfo> sPropertyInfos =
            new Dictionary<string, PropertyInfo>( StringComparer.OrdinalIgnoreCase );

        private static InputDialog sInputDialog;
        private readonly NodeList mNodes;

        private readonly Dictionary<Type, NodeImportHandler> mImportHandlers;
        private readonly Dictionary<Type, NodeExportHandler> mExportHandlers;
        private readonly Dictionary<Type, NodeReplaceHandler<T>> mReplaceHandlers;
        private readonly List<ToolStripItem> mCustomHandlers;

        private T mData;
        private INode mParent;
        private string mName;

        private ContextMenuStrip mContextMenuStrip;

        private bool mIsPendingSynchronization;
        private bool mIsPopulated;

        private OpenFileDialog mImportDialog;
        private SaveFileDialog mExportDialog;
        private OpenFileDialog mReplaceDialog;

        protected virtual T InternalData => mData;

        [Category( "Debug" )]
        [Browsable( false )]
        public T Data
        {
            get
            {
                Synchronize();
                return InternalData;
            }
        }

        protected bool IsPopulating { get; private set; }

        protected bool IsSynchronizing { get; private set; }

        public IEnumerator<INode> GetEnumerator() => 
            mNodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            mNodes.GetEnumerator();

        [Category( "Debug" )]
        [Browsable( false )]
        public abstract NodeFlags Flags { get; }

        object INode.Data => Data;

        [Category( "Debug" )]
        [Browsable( false )]
        public Type DataType => typeof( T );

        [Category( "Debug" )]
        [Browsable( false )]
        public INode Parent
        {
            get => mParent;
            set
            {
                if ( mParent != null && value != null )
                    throw new InvalidOperationException( "Cannot set parent, node is already owned by one" );

                if ( SourceConfiguration == null && value != null )
                    SourceConfiguration = value.SourceConfiguration;

                mParent = value;
            }
        }

        [Category( "Debug" )]
        [Browsable( false )]
        public IList<INode> Nodes => mNodes;

        [Category( "General" )]
        public string Name
        {
            get => mName;
            set => Rename( value );
        }

        [Category( "Debug" )]
        [Browsable( false )]
        public virtual Bitmap Image => 
            ResourceStore.LoadBitmap( "Icons/Node.png" );

        [Category( "Debug" )]
        [Browsable( false )]
        public ContextMenuStrip ContextMenuStrip =>
            mContextMenuStrip ?? ( mContextMenuStrip = CreateContextMenuStrip() );

        [Category( "Debug" )]
        [Browsable( false )]
        public virtual Control Control { get; }

        [Category( "Debug" )]
        [Browsable( false )] 
        public object Tag { get; set; }

        [Category( "Debug" )]
        [Browsable( false )] 
        public Configuration SourceConfiguration { get; set; }

        [Category( "Debug" )]
        [Browsable( false )]
        public bool IsPopulated
        {
            get => mIsPopulated && !IsPopulating;
            protected set
            {
                if ( !IsPopulating )
                    mIsPopulated = value;
            }
        }

        [Category( "Debug" )]
        [Browsable( false )]
        public bool IsPendingSynchronization
        {
            get => mIsPendingSynchronization && !IsSynchronizing;
            protected set
            {
                if ( !IsPopulating )
                    mIsPendingSynchronization = value;
            }
        }

        public event EventHandler<NodeRenameEventArgs> Renamed;
        public event EventHandler<NodeAddEventArgs> Added;
        public event EventHandler<NodeRemoveEventArgs> Removed;
        public event EventHandler<NodeImportEventArgs> Imported;
        public event EventHandler<NodeExportEventArgs> Exported;
        public event EventHandler<NodeReplaceEventArgs> Replaced;
        public event EventHandler<NodeMoveEventArgs> Moved;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Populate()
        {
            if ( IsPopulated && !Flags.HasFlag( NodeFlags.Add ) )
                return;

            mIgnoreMementos++;
            IsPopulating = true;
            {
                mNodes.Clear( true );
                PopulateCore();
            }
            IsPopulating = false;
            mIsPopulated = true;
            mIgnoreMementos--;
        }

        public void Synchronize()
        {
            if ( !IsSynchronizing && Flags.HasFlag( NodeFlags.Add ) )
            {
                foreach ( var node in mNodes )
                    node.Synchronize();
            }

            if ( IsSynchronizing || !mIsPendingSynchronization )
                return;

            IsSynchronizing = true;
            {
                SynchronizeCore();
            }
            IsSynchronizing = false;
            mIsPendingSynchronization = false;
        }

        public TNode FindParent<TNode>() where TNode : INode
        {
            for ( var parent = mParent; parent != null; parent = parent.Parent )
            {
                if ( parent is TNode node )
                    return node;
            }

            return default;
        }

        public TNode FindNode<TNode>( string nodeName, bool searchChildren ) where TNode : INode
        {
            foreach ( var node in mNodes )
            {
                if ( node is TNode genericNode && node.Name.Equals( nodeName, StringComparison.OrdinalIgnoreCase ) )
                    return genericNode;
            }

            if ( searchChildren )
            {
                foreach ( var node in mNodes )
                {
                    TNode subNode;
                    if ( ( subNode = node.FindNode<TNode>( nodeName, true ) ) != null )
                        return subNode;
                }
            }

            return default;
        }

        public void Rename( string name )
        {
            if ( !Flags.HasFlag( NodeFlags.Rename ) && !string.IsNullOrEmpty( mName ) || mName == name )
                return;

            string previousName = mName;
            {
                mName = name;

                if ( sNameProperty != null && sNameProperty.CanWrite && mData != null )
                    sNameProperty.SetValue( mData, mName );

                if ( string.IsNullOrEmpty( previousName ) )
                    return;

                OnPropertyChanged( nameof( Name ) );
                OnRename( previousName );

                PushMemento( new RenameMemento( this, previousName, name ) );
            }
        }

        public void Rename()
        {
            if ( !Flags.HasFlag( NodeFlags.Rename ) )
                return;

            if ( sInputDialog == null )
                sInputDialog = new InputDialog { WindowTitle = "Rename node" };

            sInputDialog.Input = Name;

            while ( sInputDialog.ShowDialog() == DialogResult.OK )
            {
                if ( string.IsNullOrEmpty( sInputDialog.Input ) )
                {
                    MessageBox.Show( "Please enter a valid name.", Program.Name, MessageBoxButtons.OK,
                        MessageBoxIcon.Error );

                    sInputDialog.Input = Name;
                }
                else
                {
                    Rename( sInputDialog.Input );
                    break;
                }
            }
        }

        public void Remove()
        {
            if ( mParent != null && mParent.Flags.HasFlag( NodeFlags.Remove ) )
                mParent.Nodes.Remove( this );
        }

        public void Import( string filePath )
        {
            if ( !Flags.HasFlag( NodeFlags.Import ) )
                return;

            var module = ModuleImportUtilities.GetModule( mImportHandlers.Keys, filePath );

            if ( module == null )
            {
                MessageBox.Show( "File could not be imported.", Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error );
            }

            else
            {
                ConfigurationList.Instance.DetermineCurrentConfiguration( filePath );

                mImportHandlers[ module.ModelType ]( filePath );
                OnImport( filePath );

                if ( !IsPopulated ) 
                    return;

                IsPopulated = false;
                Populate();

                MementoStack.Clear();
            }
        }

        public string[] Import()
        {
            if ( !Flags.HasFlag( NodeFlags.Import ) )
                return null;

            if ( mImportDialog == null )
            {
                mImportDialog = new OpenFileDialog
                {
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    Filter = ModuleFilterGenerator.GenerateFilter( mImportHandlers.Keys, FormatExtensionFlags.Import ),
                    Title = "Select file(s) to import from.",
                    ValidateNames = true,
                    AddExtension = true,
                    Multiselect = true
                };
            }

            mImportDialog.FileName = Name;

            if ( mImportDialog.ShowDialog() != DialogResult.OK ) 
                return null;

            foreach ( string filePath in mImportDialog.FileNames )
                Import( filePath );

            return mImportDialog.FileNames;
        }

        public void Export( string filePath )
        {
            if ( !Flags.HasFlag( NodeFlags.Export ) )
                return;

            var module = ModuleExportUtilities.GetModule( mExportHandlers.Keys, filePath );

            if ( module == null )
            {
                MessageBox.Show( "Node could not be exported.", Program.Name, MessageBoxButtons.OK,
                    MessageBoxIcon.Error );
            }
            else
            {
                ConfigurationList.Instance.DetermineCurrentConfiguration( filePath );

                mExportHandlers[ module.ModelType ]( filePath );
                OnExport( filePath );
            }
        }

        public string Export()
        {
            if ( !Flags.HasFlag( NodeFlags.Export ) )
                return null;

            if ( mExportDialog == null )
            {
                mExportDialog = new SaveFileDialog
                {
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    Filter = ModuleFilterGenerator.GenerateFilter( mExportHandlers.Keys, FormatExtensionFlags.Export ),
                    OverwritePrompt = true,
                    Title = "Select a file to export to.",
                    ValidateNames = true,
                    AddExtension = true
                };
            }

            mExportDialog.FileName = Name;

            if ( mExportDialog.ShowDialog() != DialogResult.OK ) 
                return null;

            Export( mExportDialog.FileName );
            return mExportDialog.FileName;
        }

        void INode.Replace( object data )
        {
            Replace( data as T );
        }

        public void Replace( string filePath )
        {
            if ( !Flags.HasFlag( NodeFlags.Replace ) )
                return;

            var module = ModuleImportUtilities.GetModule( mReplaceHandlers.Keys, filePath );
            if ( module == null )
            {
                MessageBox.Show( "Node could not be replaced.", Program.Name, MessageBoxButtons.OK,
                    MessageBoxIcon.Error );
            }

            else
            {
                ConfigurationList.Instance.DetermineCurrentConfiguration( filePath );
                var configuration = ConfigurationList.Instance.CurrentConfiguration;

                var obj = mReplaceHandlers[ module.ModelType ]( filePath );

                if ( obj == null )
                    return;

                Replace( obj );
                SourceConfiguration = configuration;
            }
        }

        public string Replace()
        {
            if ( !Flags.HasFlag( NodeFlags.Replace ) )
                return null;

            if ( mReplaceDialog == null )
            {
                mReplaceDialog = new OpenFileDialog
                {
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    CheckFileExists = true,
                    Filter = ModuleFilterGenerator.GenerateFilter( mReplaceHandlers.Keys, FormatExtensionFlags.Import ),
                    Title = "Select a file to replace with.",
                    ValidateNames = true,
                    AddExtension = true
                };
            }

            mReplaceDialog.FileName = Name;

            if ( mReplaceDialog.ShowDialog() != DialogResult.OK )
                return null;

            Replace( mReplaceDialog.FileName );
            return mReplaceDialog.FileName;
        }

        public void Move( int index, int targetIndex )
        {
            mNodes.Move( index, targetIndex );
        }

        public void MoveUp()
        {
            if ( mParent == null || !mParent.Flags.HasFlag( NodeFlags.Move ) )
                return;

            int index = mParent.Nodes.IndexOf( this );
            mParent.Move( index, index - 1 );
        }

        public void MoveDown()
        {
            if ( mParent == null || !mParent.Flags.HasFlag( NodeFlags.Move ) )
                return;

            int index = mParent.Nodes.IndexOf( this );
            mParent.Move( index, index + 1 );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        public void DisposeData()
        {
            if ( mData is IDisposable disposable )
                disposable.Dispose();
        }

        public override string ToString()
        {
            return Name;
        }

        public void Replace( T data )
        {
            if ( !Flags.HasFlag( NodeFlags.Replace ) || data == null || data.Equals( Data ) )
                return;

            var previousData = Data;
            {
                mData = data;
                mIsPopulated = false;
                mIsPendingSynchronization = false;

                OnReplace( previousData );

                // TODO: Implement a memento for replacing somehow
                MementoStack.Clear();
            }
        }

        private static PropertyInfo GetPropertyInfo( string propertyName )
        {
            if ( sPropertyInfos.TryGetValue( propertyName, out var propertyInfo ) )
                return propertyInfo;

            propertyInfo = typeof( T ).GetProperty( propertyName );

            if ( propertyInfo == null )
                Debug.WriteLine( $"Could not find property info: {propertyName}" );

            sPropertyInfos[ propertyName ] = propertyInfo;
            return propertyInfo;
        }

        protected TProperty GetProperty<TProperty>( [CallerMemberName] string propertyName = null ) => 
            ( TProperty ) GetPropertyInfo( propertyName )?.GetValue( InternalData );

        protected void SetProperty<TProperty>( TProperty value, [CallerMemberName] string propertyName = null )
        {
            var propertyInfo = GetPropertyInfo( propertyName );

            if ( propertyInfo.PropertyType.IsValueType && propertyInfo.GetValue( InternalData ).Equals( value ) )
                return;

            PushMemento( new PropertyMemento<TProperty>( this, propertyName, ( TProperty ) propertyInfo.GetValue( InternalData ), value ) );

            propertyInfo.SetValue( InternalData, value );
            OnPropertyChanged( propertyName );
        }

        protected static EventHandler CreateEventHandler( Action action ) => 
            ( sender, args ) => action();

        protected void AddImportHandler<TModule>( NodeImportHandler handler ) => 
            mImportHandlers[ typeof( TModule ) ] = handler;

        protected void AddExportHandler<TModule>( NodeExportHandler handler ) => 
            mExportHandlers[ typeof( TModule ) ] = handler;

        protected void AddReplaceHandler<TModule>( NodeReplaceHandler<T> handler ) => 
            mReplaceHandlers[ typeof( TModule ) ] = handler;

        protected void AddCustomHandler( string name, Action action, 
            Keys shortcutKeys = Keys.None, CustomHandlerFlags flags = CustomHandlerFlags.None )
        {
            mCustomHandlers.Add( new ToolStripMenuItem( name, null, ( s, e ) =>
            {
                if ( flags.HasFlag( CustomHandlerFlags.ClearMementos ) )
                {
                    action();
                    MementoStack.Clear();
                }

                else
                {
                    MementoStack.BeginCompoundMemento();
                    action();
                    MementoStack.EndCompoundMemento();
                }

                if ( flags.HasFlag( CustomHandlerFlags.Repopulate ) && IsPopulated )
                {
                    IsPopulated = false;
                    Populate();
                }
            }, shortcutKeys ) );
        }

        protected void AddCustomHandlerSeparator() => 
            mCustomHandlers.Add( new ToolStripSeparator() );

        protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null ) => 
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );

        protected virtual void OnRename( string previousName ) => 
            Renamed?.Invoke( this, new NodeRenameEventArgs( previousName ) );

        protected virtual void OnAdd( INode addedNode, int index ) => 
            Added?.Invoke( this, new NodeAddEventArgs( addedNode, index ) );

        protected virtual void OnRemove( INode removedNode ) => 
            Removed?.Invoke( this, new NodeRemoveEventArgs( removedNode ) );

        protected virtual void OnImport( string filePath ) => 
            Imported?.Invoke( this, new NodeImportEventArgs( filePath ) );

        protected virtual void OnExport( string filePath ) => 
            Exported?.Invoke( this, new NodeExportEventArgs( filePath ) );

        protected virtual void OnReplace( T previousData ) => 
            Replaced?.Invoke( this, new NodeReplaceEventArgs( previousData ) );

        protected virtual void OnMove( INode movedNode, int previousIndex, int newIndex ) => 
            Moved?.Invoke( this, new NodeMoveEventArgs( movedNode, previousIndex, newIndex ) );

        private void OnChildRenamed( object sender, NodeRenameEventArgs args ) => 
            IsPendingSynchronization = true;

        private void OnChildReplaced( object sender, NodeReplaceEventArgs args ) => 
            IsPendingSynchronization = true;

        protected abstract void Initialize();
        protected abstract void PopulateCore();
        protected abstract void SynchronizeCore();

        private ContextMenuStrip CreateContextMenuStrip()
        {
            if ( mContextMenuStrip != null )
                return mContextMenuStrip;

            mContextMenuStrip = new ContextMenuStrip();

            if ( mCustomHandlers.Count > 0 )
            {
                foreach ( var toolStripItem in mCustomHandlers )
                    ContextMenuStrip.Items.Add( toolStripItem );

                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( Flags.HasFlag( NodeFlags.Import ) && mImportHandlers.Count > 0 )
            {
                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Import", null, CreateEventHandler( () => Import() ), Keys.Control | Keys.I ) );
            }

            if ( Flags.HasFlag( NodeFlags.Export ) && mExportHandlers.Count > 0 )
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Export", null, CreateEventHandler( () => Export() ), Keys.Control | Keys.E ) );

            if ( Flags.HasFlag( NodeFlags.Replace ) && mReplaceHandlers.Count > 0 )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Replace", null, CreateEventHandler( () => Replace() ), Keys.Control | Keys.R ) );
                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( mParent != null && mParent.Flags.HasFlag( NodeFlags.Move ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Move Up", null, CreateEventHandler( MoveUp ), Keys.Control | Keys.Up ) );
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Move Down", null, CreateEventHandler( MoveDown ), Keys.Control | Keys.Down ) );
            }

            if ( mParent != null && mParent.Flags.HasFlag( NodeFlags.Remove ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Remove", null, CreateEventHandler( Remove ), Keys.Control | Keys.Delete ) );
                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( Flags.HasFlag( NodeFlags.Rename ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Rename", null, CreateEventHandler( Rename ), Keys.Control | Keys.N ) );
            }

            // Remove start/end/duplicate separators
            for ( int i = 0; i < ContextMenuStrip.Items.Count; )
            {
                if ( ( ( i != 0 && i != ContextMenuStrip.Items.Count - 1 ) || !( ContextMenuStrip.Items[ i ] is ToolStripSeparator ) ) &&
                     ( !( ContextMenuStrip.Items[ i++ ] is ToolStripSeparator ) || !( ContextMenuStrip.Items[ i ] is ToolStripSeparator ) ) ) continue;

                ContextMenuStrip.Items.RemoveAt( i++ );
            }

            return mContextMenuStrip;
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( !disposing )
                return;

            foreach ( var node in mNodes )
                node.Dispose();

            mContextMenuStrip?.Dispose();
            mImportDialog?.Dispose();
            mExportDialog?.Dispose();
            mReplaceDialog?.Dispose();
        }

        ~Node()
        {
            Dispose( false );
        }

        protected Node( string name, T data )
        {
            mData = data;
            Name = name;

            mNodes = new NodeList( this );
            mImportHandlers = new Dictionary<Type, NodeImportHandler>();
            mExportHandlers = new Dictionary<Type, NodeExportHandler>();
            mReplaceHandlers = new Dictionary<Type, NodeReplaceHandler<T>>();
            mCustomHandlers = new List<ToolStripItem>();

            SourceConfiguration = ConfigurationList.Instance.CurrentConfiguration;

            Initialize();
        }

        [Flags]
        protected enum CustomHandlerFlags
        {
            None = 0,
            Repopulate = 1 << 0,
            ClearMementos = 1 << 1
        }
    }   
}