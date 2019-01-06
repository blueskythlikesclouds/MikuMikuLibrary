using MikuMikuModel.Configurations;
using MikuMikuModel.FormatModules;
using MikuMikuModel.GUI.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MikuMikuModel.DataNodes
{
    public delegate object DataNodeReplaceHandler( string filePath );
    public delegate void DataNodeExportHandler( string filePath );
    public delegate void DataNodeImportHandler( string filePath );
    public delegate object DataNodeDataUpdateHandler();

    public abstract class DataNode : IDisposable, IEnumerable<DataNode>, INotifyPropertyChanged
    {
        //
        // Fields
        //
        protected readonly List<DataNode> mNodes;
        private readonly Dictionary<Type, DataNodeImportHandler> mImportHandlers;
        private readonly Dictionary<Type, DataNodeExportHandler> mExportHandlers;
        private readonly Dictionary<Type, DataNodeReplaceHandler> mReplaceHandlers;
        private readonly List<ToolStripItem> mCustomHandlers;
        private DataNodeDataUpdateHandler mDataUpdateHandler;

        private object mData;
        protected string mName;
        protected DataNode mParent;
        private bool mHasPendingChanges;

        //
        // Event Handlers
        //
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<DataNodeNameChangedEventArgs> NameChanged;
        public event EventHandler<DataNodeNodeEventArgs> NodeAdded;
        public event EventHandler<DataNodeNodeEventArgs> NodeRemoved;
        public event EventHandler<DataNodeNodeMovedEventArgs> NodeMoved;
        public event EventHandler<DataNodeDataReplacedEventArgs> DataReplaced;

        //
        // Properties
        //
        [Browsable( false )]
        public virtual object Data
        {
            get
            {
                if ( HasPendingChanges && !IsUpdatingData && IsViewInitialized )
                {
                    IsUpdatingData = true;

                    if ( mDataUpdateHandler != null )
                    {
                        var newData = mDataUpdateHandler();
                        mData = newData;
                    }

                    HasPendingChanges = false;
                    IsUpdatingData = false;
                }

                return mData;
            }
            protected set
            {
                if ( IsInitialized )
                    HasPendingChanges = true;

                mData = value;
            }
        }

        [Browsable( false )]
        public abstract Type DataType { get; }

        [Browsable( false )]
        public virtual string SpecialName => DataNodeFactory.GetSpecialName( GetType() );

        public virtual string Name
        {
            get => mName;
            protected set => Rename( value );
        }

        [Browsable( false )]
        public virtual DataNode Parent => mParent;

        [Browsable( false )]
        public abstract DataNodeFlags Flags { get; }

        [Browsable( false )]
        public abstract DataNodeActionFlags ActionFlags { get; }

        [Browsable( false )]
        public virtual ContextMenuStrip ContextMenuStrip { get; private set; }

        [Browsable( false )]
        public virtual Control Control { get; }

        [Browsable( false )]
        public virtual object Tag { get; set; }

        [Browsable( false )]
        public virtual bool IsInitialized { get; private set; }

        [Browsable( false )]
        public virtual bool HasPendingChanges
        {
            get => mHasPendingChanges;
            protected set
            {
                // Don't want node constructions
                // to affect this flag
                if ( IsInitialized )
                {
                    mHasPendingChanges = value;
                    if ( mParent != null )
                        mParent.HasPendingChanges = true;
                }
            }
        }

        [Browsable( false )]
        public virtual bool IsUpdatingData { get; private set; }

        [Browsable( false )]
        public virtual bool IsViewInitialized { get; private set; }

        [Browsable( false )]
        public virtual IEnumerable<DataNode> Nodes => mNodes;

        [Browsable( false )]
        public virtual Bitmap Icon => Properties.Resources.Node;

        //
        // Methods
        //
        public virtual void Add( DataNode node )
        {
            // Don't add if this node can't contain nodes
            if ( !Flags.HasFlag( DataNodeFlags.Branch ) )
                return;

            if ( node.Parent != null )
                node.Remove();

            node.mParent = this;
            mNodes.Add( node );
            OnAdd( node );
            HasPendingChanges = true;
        }

        public virtual IEnumerable<DataNode> EnumerateNodes() => mNodes;

        public virtual IEnumerable<DataNode> EnumerateNodes( Type dataType, bool searchChildren )
        {
            if ( searchChildren )
            {
                var nodesRoot = mNodes.Where( x => x.DataType == dataType );
                return nodesRoot.Concat( nodesRoot.SelectMany( x => x.EnumerateNodes( dataType, true ) ) );
            }

            return mNodes.Where( x => x.DataType == dataType );
        }

        public virtual IEnumerable<DataNode> EnumerateNodes<T>( bool searchChildren )
        {
            return EnumerateNodes( typeof( T ), searchChildren );
        }

        public virtual IEnumerable<DataNode> EnumerateNodes( string name, StringComparison comparison, bool searchChildren )
        {
            if ( searchChildren )
            {
                var nodesRoot = mNodes.Where( x => x.Name.Equals( name, comparison ) );
                return nodesRoot.Concat( nodesRoot.SelectMany( x => x.EnumerateNodes( name, comparison, true ) ) );
            }

            return mNodes.Where( x => x.Name == Name );
        }

        public virtual DataNode FindNode( string name, StringComparison comparison )
        {
            return mNodes.FirstOrDefault( x => x.Name.Equals( name, comparison ) );
        }

        public virtual DataNode FindNode( Type dataType, string name, StringComparison comparison )
        {
            return mNodes.FirstOrDefault( x => x.Name.Equals( name, comparison ) && x.DataType.Equals( dataType ) );
        }

        public virtual DataNode<T> FindNode<T>( string name, StringComparison comparison )
        {
            return ( DataNode<T> )FindNode( typeof( T ), name, comparison );
        }

        public virtual DataNode FindParent( Type dataType )
        {
            if ( dataType.IsInstanceOfType( Parent?.mData ) )
                return Parent;

            return Parent?.FindParent( dataType );
        }

        public virtual DataNode<T> FindParent<T>()
        {
            return ( DataNode<T> )FindParent( typeof( T ) );
        }

        public virtual void Export( string filePath )
        {
            if ( !ActionFlags.HasFlag( DataNodeActionFlags.Export ) )
                return;

            var module = FormatModuleUtilities.GetModuleForExport( Path.GetFileName( filePath ), mExportHandlers.Keys );
            if ( module != null )
            {
                ConfigurationList.Instance.DetermineCurrentConfiguration( filePath );
                mExportHandlers[ module.ModelType ].Invoke( filePath );
            }
            else
            {
                MessageBox.Show( "File could not be exported.", "Miku Miku Model", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        public virtual string Export()
        {
            if ( !ActionFlags.HasFlag( DataNodeActionFlags.Export ) )
                return null;

            using ( var dialog = new SaveFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.FileName = Name;
                dialog.Filter = FormatModuleUtilities.GetFilter( mExportHandlers.Keys, FormatModuleFlags.Export );
                dialog.OverwritePrompt = true;
                dialog.Title = "Select a file to export to.";
                dialog.ValidateNames = true;
                dialog.AddExtension = true;

                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    Export( dialog.FileName );
                    return dialog.FileName;
                }
            }

            return null;
        }

        public virtual void Import( string filePath )
        {
            if ( !ActionFlags.HasFlag( DataNodeActionFlags.Import ) )
                return;

            var module = FormatModuleUtilities.GetModuleForImport( filePath, mImportHandlers.Keys );
            if ( module != null )
            {
                ConfigurationList.Instance.DetermineCurrentConfiguration( filePath );
                mImportHandlers[ module.ModelType ].Invoke( filePath );
            }
            else
            {
                MessageBox.Show( "File could not be imported.", "Miku Miku Model", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        public virtual void Import()
        {
            if ( !ActionFlags.HasFlag( DataNodeActionFlags.Import ) )
                return;

            using ( var dialog = new OpenFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.FileName = Name;
                dialog.Filter = FormatModuleUtilities.GetFilter( mImportHandlers.Keys, FormatModuleFlags.Import );
                dialog.Title = "Select file(s) to import from.";
                dialog.ValidateNames = true;
                dialog.AddExtension = true;
                dialog.Multiselect = true;

                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    foreach ( var filePath in dialog.FileNames )
                        Import( filePath );
                }
            }
        }

        public virtual void MoveDown()
        {
            if ( mParent != null )
            {
                var index = mParent.mNodes.IndexOf( this );
                if ( index < mParent.mNodes.Count - 1 )
                {
                    mParent.mNodes.RemoveAt( index );
                    mParent.mNodes.Insert( index + 1, this );
                    mParent.OnMove( index, index + 1, this );
                    HasPendingChanges = true;
                }
            }
        }

        public virtual void MoveUp()
        {
            if ( mParent != null )
            {
                var index = mParent.mNodes.IndexOf( this );
                if ( index > 0 )
                {
                    mParent.mNodes.RemoveAt( index );
                    mParent.mNodes.Insert( index - 1, this );
                    mParent.OnMove( index, index - 1, this );
                    HasPendingChanges = true;
                }
            }
        }

        public virtual void Remove()
        {
            mParent?.Remove( this );
        }

        public virtual void Remove( DataNode node )
        {
            if ( mNodes.Contains( node ) )
            {
                node.mParent = null;
                mNodes.Remove( node );
                OnRemove( node );
                HasPendingChanges = true;
            }
        }

        public virtual void Replace( object data )
        {
            if ( data == null )
                throw new ArgumentNullException( nameof( data ) );

            if ( data.GetType() != DataType && !data.GetType().IsSubclassOf( DataType ) )
                throw new ArgumentException( "Data does not equal node's data type", nameof( data ) );

            var oldData = Data;

            HasPendingChanges = false;
            IsInitialized = false;
            IsViewInitialized = false;
            IsUpdatingData = true;

            {
                mData = data;
                OnReplace( oldData );
            }

            IsInitialized = true;
            IsViewInitialized = false;
            IsUpdatingData = false;
            HasPendingChanges = true;
        }

        public virtual void Replace( string filePath )
        {
            if ( !ActionFlags.HasFlag( DataNodeActionFlags.Replace ) )
                return;

            var module = FormatModuleUtilities.GetModuleForImport( filePath, mReplaceHandlers.Keys );
            if ( module != null )
            {
                ConfigurationList.Instance.DetermineCurrentConfiguration( filePath );
                Replace( mReplaceHandlers[ module.ModelType ].Invoke( filePath ) );
            }
            else
            {
                MessageBox.Show( "Node could not be replaced.", "Miku Miku Model", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        public virtual void Replace()
        {
            if ( !ActionFlags.HasFlag( DataNodeActionFlags.Replace ) )
                return;

            using ( var dialog = new OpenFileDialog() )
            {
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists = true;
                dialog.CheckFileExists = true;
                dialog.FileName = Name;
                dialog.Filter = FormatModuleUtilities.GetFilter( mReplaceHandlers.Keys, FormatModuleFlags.Import );
                dialog.Title = "Select a file to replace with.";
                dialog.ValidateNames = true;
                dialog.AddExtension = true;

                if ( dialog.ShowDialog() == DialogResult.OK )
                    Replace( dialog.FileName );
            }
        }

        public virtual void Rename( string name )
        {
            string oldName = Name;
            mName = name;

            OnRename( oldName );
            HasPendingChanges = true;
        }

        public virtual void Rename()
        {
            using ( var dialog = new RenameForm( Name ) )
            {
                if ( dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty( dialog.TextBoxText ) )
                    Rename( dialog.TextBoxText );
            }
        }

        public virtual void Clear()
        {
            while ( mNodes.Count > 0 )
                mNodes[ 0 ].Remove();
        }

        public void NotifyPropertyChanged( [CallerMemberName]string propertyName = null )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
            HasPendingChanges = true;
        }

        protected T GetProperty<T>( [CallerMemberName]string propertyName = null )
        {
            if ( string.IsNullOrEmpty( propertyName ) )
                throw new ArgumentNullException( nameof( propertyName ) );

            var property = DataType.GetProperty( propertyName, typeof( T ) );
            if ( property != null && property.CanRead )
                return ( T )property.GetValue( Data );

            var field = DataType.GetField( propertyName );
            if ( field != null )
                return ( T )field.GetValue( Data );

            Debug.WriteLine( $"Could not find property: {propertyName}" );
            return default( T );
        }

        protected void SetProperty<T>( T value, [CallerMemberName]string propertyName = null )
        {
            if ( string.IsNullOrEmpty( propertyName ) )
                throw new ArgumentNullException( nameof( propertyName ) );

            var property = DataType.GetProperty( propertyName, typeof( T ) );
            if ( property != null && property.CanWrite )
            {
                property.SetValue( Data, value );
                NotifyPropertyChanged( propertyName );
                return;
            }

            var field = DataType.GetField( propertyName );
            if ( field != null )
            {
                field.SetValue( Data, value );
                NotifyPropertyChanged( propertyName );
                return;
            }

            Debug.WriteLine( $"Could not find property: {propertyName}" );
        }

        protected void RegisterImportHandler<T>( DataNodeImportHandler handler ) =>
            mImportHandlers[ typeof( T ) ] = handler;

        protected void RegisterExportHandler<T>( DataNodeExportHandler handler ) =>
            mExportHandlers[ typeof( T ) ] = handler;

        protected void RegisterReplaceHandler<T>( DataNodeReplaceHandler handler ) =>
            mReplaceHandlers[ typeof( T ) ] = handler;

        protected void RegisterCustomHandler( string name, Action action, Keys shortcutKeys = Keys.None ) =>
            mCustomHandlers.Add( new ToolStripMenuItem( name, null, CreateEventHandler( action ), shortcutKeys ) );

        protected void RegisterDataUpdateHandler( DataNodeDataUpdateHandler handler ) =>
            mDataUpdateHandler = handler;

        public void InitializeContextMenuStrip()
        {
            if ( ContextMenuStrip != null )
                return;

            ContextMenuStrip = new ContextMenuStrip();

            if ( mCustomHandlers.Any() )
            {
                foreach ( var toolStripItem in mCustomHandlers )
                    ContextMenuStrip.Items.Add( toolStripItem );

                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( ActionFlags.HasFlag( DataNodeActionFlags.Import ) && mImportHandlers.Any() )
            {
                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Import", null, CreateEventHandler( Import ), Keys.Control | Keys.I ) );
            }

            if ( ActionFlags.HasFlag( DataNodeActionFlags.Export ) && mExportHandlers.Any() )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Export", null, CreateEventHandler( () => Export() ), Keys.Control | Keys.E ) );
            }

            if ( ActionFlags.HasFlag( DataNodeActionFlags.Replace ) && mReplaceHandlers.Any() )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Replace", null, CreateEventHandler( Replace ), Keys.Control | Keys.R ) );
                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( ActionFlags.HasFlag( DataNodeActionFlags.Move ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Move Up", null, CreateEventHandler( MoveUp ), Keys.Control | Keys.Up ) );
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Move Down", null, CreateEventHandler( MoveDown ), Keys.Control | Keys.Down ) );
            }

            if ( ActionFlags.HasFlag( DataNodeActionFlags.Remove ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Remove", null, CreateEventHandler( Remove ), Keys.Control | Keys.Delete ) );
                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
            }

            if ( ActionFlags.HasFlag( DataNodeActionFlags.Rename ) )
            {
                ContextMenuStrip.Items.Add( new ToolStripSeparator() );
                ContextMenuStrip.Items.Add( new ToolStripMenuItem( "Rename", null, CreateEventHandler( Rename ), Keys.Control | Keys.N ) );
            }

            // Remove start/end/duplicate separators
            for ( int i = 0; i < ContextMenuStrip.Items.Count; )
            {
                if ( ( ( i == 0 || ( i == ContextMenuStrip.Items.Count - 1 ) ) && ContextMenuStrip.Items[ i ] is ToolStripSeparator ) ||
                    ( ContextMenuStrip.Items[ i++ ] is ToolStripSeparator && ContextMenuStrip.Items[ i ] is ToolStripSeparator ) )
                {
                    ContextMenuStrip.Items.RemoveAt( i++ );
                }
            }
        }

        protected EventHandler CreateEventHandler( Action action ) => ( s, e ) => action();

        public void InitializeView( bool force = false )
        {
            if ( Flags.HasFlag( DataNodeFlags.Branch ) && ( force || !IsViewInitialized ) )
            {
                IsInitialized = false;
                {
                    Clear();
                    InitializeViewCore();
                }
                IsInitialized = true;
                IsViewInitialized = true;
            }

            if ( ContextMenuStrip == null )
                InitializeContextMenuStrip();
        }

        public void Initialize()
        {
            if ( IsInitialized )
                return;

            InitializeCore();
            IsInitialized = true;
        }

        // 
        // Abstract Methods
        //
        protected abstract void InitializeCore();
        protected abstract void InitializeViewCore();

        protected virtual void OnPropertyChanged( string propertyName ) =>
            NotifyPropertyChanged( propertyName );

        protected virtual void OnAdd( DataNode addedNode ) =>
            NodeAdded?.Invoke( this, new DataNodeNodeEventArgs( this, addedNode ) );

        protected virtual void OnExport() { }

        protected virtual void OnImport( DataNode importedNode ) { }

        protected virtual void OnMove( int oldIndex, int newIndex, DataNode movedNode ) =>
            NodeMoved?.Invoke( this, new DataNodeNodeMovedEventArgs( this, movedNode, oldIndex, newIndex ) );

        protected virtual void OnRemove( DataNode removedNode ) =>
            NodeRemoved?.Invoke( this, new DataNodeNodeEventArgs( this, removedNode ) );

        protected virtual void OnRename( string oldName ) =>
            NameChanged?.Invoke( this, new DataNodeNameChangedEventArgs( this, oldName ) );

        protected virtual void OnReplace( object oldData ) =>
            DataReplaced?.Invoke( this, new DataNodeDataReplacedEventArgs( this, oldData ) );

        public IEnumerator<DataNode> GetEnumerator() =>
            ( ( IEnumerable<DataNode> )mNodes ).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ( ( IEnumerable<DataNode> )mNodes ).GetEnumerator();

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // If the object can be disposed, dispose it
                if ( Data is IDisposable disposable )
                    disposable.Dispose();

                // Do the same for children
                foreach ( var node in mNodes )
                    node.Dispose();
            }
        }

        ~DataNode()
        {
            Dispose( false );
        }

        public DataNode( string name, object data )
        {
            mData = data;
            Rename( name );

            mNodes = new List<DataNode>();
            mImportHandlers = new Dictionary<Type, DataNodeImportHandler>();
            mExportHandlers = new Dictionary<Type, DataNodeExportHandler>();
            mReplaceHandlers = new Dictionary<Type, DataNodeReplaceHandler>();
            mCustomHandlers = new List<ToolStripItem>();

            Initialize();
        }
    }

    [Flags]
    public enum DataNodeFlags
    {
        Leaf = 1,
        Branch = 2,
    }

    [Flags]
    public enum DataNodeActionFlags
    {
        None = 0,
        Add = 1,
        Export = 2,
        Import = 4,
        Move = 8,
        Remove = 16,
        Rename = 32,
        Replace = 64,
    }

    //
    // Name Changed
    //

    public class DataNodeNameChangedEventArgs : EventArgs
    {
        public DataNode Node { get; }
        public string OldName { get; }

        public DataNodeNameChangedEventArgs( DataNode node, string oldName )
        {
            Node = node;
            OldName = oldName;
        }
    }

    //
    // Child Added/Removed
    //

    public class DataNodeNodeEventArgs : EventArgs
    {
        public DataNode ParentNode { get; }
        public DataNode ChildNode { get; }

        public DataNodeNodeEventArgs( DataNode parentNode, DataNode childNode )
        {
            ParentNode = parentNode;
            ChildNode = childNode;
        }
    }

    //
    // Data Replaced
    //

    public class DataNodeDataReplacedEventArgs : EventArgs
    {
        public DataNode Node { get; }
        public object OldData { get; }

        public DataNodeDataReplacedEventArgs( DataNode node, object oldData )
        {
            Node = node;
            OldData = oldData;
        }
    }

    //
    // Node Moved
    //

    public class DataNodeNodeMovedEventArgs : EventArgs
    {
        public DataNode ParentNode { get; }
        public DataNode MovedNode { get; }
        public int OldIndex { get; }
        public int NewIndex { get; }

        public DataNodeNodeMovedEventArgs( DataNode parentNode, DataNode movedNode, int oldIndex, int newIndex )
        {
            ParentNode = parentNode;
            MovedNode = movedNode;
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }
    }

    //
    // Attributes
    //

    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false )]
    public class DataNodeSpecialNameAttribute : Attribute
    {
        public string Name { get; }

        public DataNodeSpecialNameAttribute( string name )
        {
            Name = name;
        }
    }
}
