using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MikuMikuLibrary.Archives;
using MikuMikuLibrary.IO;
using MikuMikuModel.Configurations;
using MikuMikuModel.GUI.Forms;
using MikuMikuModel.Modules;
using MikuMikuModel.Nodes.Archives;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.IO
{
    public abstract class BinaryFileNode<T> : Node<T>, IDirtyNode where T : class, IBinaryFile, new()
    {
        private static readonly IFormatModule sModule = FormatModuleRegistry.ModulesByType[ typeof( T ) ];

        private readonly Func<Stream> mStreamGetter;
        private bool mLoaded;
        private bool mDirty;

        protected override T InternalData
        {
            get
            {
                var internalData = base.InternalData;

                if ( mLoaded || mStreamGetter == null )
                    return internalData;

                ConfigurationList.Instance.CurrentConfiguration = SourceConfiguration;
                {
                    Load( internalData, mStreamGetter() );
                }

                mLoaded = true;
                return internalData;
            }
        }

        [Category( "Binary serialization" )]
        public Endianness Endianness
        {
            get => GetProperty<Endianness>();
            set => SetProperty( value );
        }

        [Category( "Binary serialization" )]
        public BinaryFormat Format
        {
            get => GetProperty<BinaryFormat>();
            set => SetProperty( value );
        }

        public event EventHandler DirtyStateChanged;

        [Category( "Debug" )]
        [Browsable( false )]
        public virtual bool IsDirty
        {
            get => mDirty;
            protected set
            {
                if ( IsPopulating && value )
                    return;

                bool previousValue = mDirty;
                mDirty = value;

                if ( previousValue == mDirty )
                    return;

                OnDirtyStateChanged();

                if ( mDirty )
                    IsPendingSynchronization = true;
            }
        }

        public override Bitmap Image =>
            ResourceStore.LoadBitmap( "Icons/File.png" );

        public virtual Stream GetStream()
        {
            return new DynamicStream( this );
        }

        protected virtual void Load( T data, Stream source )
        {
            data.Load( source );
        }

        protected void SetSubscription( INode node, bool unsubscribe = false )
        {
            if ( node.IsPopulated || unsubscribe && node.IsPopulated )
                IsDirty = true;

            if ( unsubscribe )
            {
                if ( node is IDirtyNode dirtyNode )
                {
                    dirtyNode.DirtyStateChanged -= OnNodeDirtyStateChanged;
                }

                else
                {
                    node.PropertyChanged -= OnNodePropertyChanged;
                    node.Added -= OnNodeAdded;
                    node.Removed -= OnNodeRemoved;
                    node.Imported -= OnNodeImported;
                    node.Replaced -= OnNodeReplaced;
                    node.Moved -= OnNodeMoved;

                    foreach ( var childNode in node.Nodes )
                        SetSubscription( childNode, true );
                }
            }
            else
            {
                if ( node is IDirtyNode dirtyNode )
                {
                    dirtyNode.DirtyStateChanged += OnNodeDirtyStateChanged;
                }

                else
                {
                    node.PropertyChanged += OnNodePropertyChanged;
                    node.Added += OnNodeAdded;
                    node.Removed += OnNodeRemoved;
                    node.Imported += OnNodeImported;
                    node.Replaced += OnNodeReplaced;
                    node.Moved += OnNodeMoved;
                }
            }

            void OnNodeDirtyStateChanged( object sender, EventArgs args ) => 
                IsDirty = ( ( IDirtyNode ) sender ).IsDirty || IsDirty;

            void OnNodePropertyChanged( object sender, PropertyChangedEventArgs args ) => 
                IsDirty = true;

            void OnNodeAdded( object sender, NodeAddEventArgs args )
            {
                IsDirty |= args.AddedNode.Parent.IsPopulated;
                SetSubscription( args.AddedNode );
            }

            void OnNodeRemoved( object sender, NodeRemoveEventArgs args )
            {
                IsDirty = true;
                SetSubscription( args.RemovedNode, true );
            }

            void OnNodeImported( object sender, NodeImportEventArgs args ) =>
                IsDirty = true;

            void OnNodeReplaced( object sender, NodeReplaceEventArgs args ) => 
                IsDirty = true;

            void OnNodeMoved( object sender, NodeMoveEventArgs args ) => 
                IsDirty = true;
        }

        protected void AddDirtyCustomHandler( string name, Func<bool> func,
            Keys shortcutKeys = Keys.None, CustomHandlerFlags flags = CustomHandlerFlags.None )
        {
            AddCustomHandler( name, () =>
            {
                IsDirty |= func();
            }, shortcutKeys, flags );
        }

        protected override void OnPropertyChanged( string propertyName = null )
        {
            IsDirty = true;
            base.OnPropertyChanged( propertyName );
        }

        protected override void OnRename( string previousName )
        {
            IsDirty = true;
            base.OnRename( previousName );
        }

        protected override void OnAdd( INode addedNode, int index )
        {
            SetSubscription( addedNode );
            base.OnAdd( addedNode, index );
        }

        protected override void OnRemove( INode removedNode )
        {
            SetSubscription( removedNode, true );
            base.OnRemove( removedNode );
        }

        protected override void OnImport( string filePath )
        {
            IsDirty = true;
            base.OnImport( filePath );
        }

        protected override void OnExport( string filePath )
        {
            IsDirty = false;
            base.OnExport( filePath );
        }

        protected override void OnReplace( T previousData )
        {
            IsDirty = true;

            Data.Format = previousData.Format;
            Data.Endianness = previousData.Endianness;

            base.OnReplace( previousData );
        }

        protected override void OnMove( INode movedNode, int previousIndex, int newIndex )
        {
            IsDirty = true;
            base.OnMove( movedNode, previousIndex, newIndex );
        }

        protected virtual void OnDirtyStateChanged() => 
            DirtyStateChanged?.Invoke( this, EventArgs.Empty );

        public static T PromptFarcArchiveViewForm( string filePath, 
            string title = "Select a file to replace with.", 
            string errorOnEmpty = "This archive has no files that you could replace the node with." )
        {
            using ( var farcArchive = BinaryFile.Load<FarcArchive>( filePath ) )
            using ( var farcArchiveNode = new FarcArchiveNode( Path.GetFileName( filePath ), farcArchive ) )
            using ( var nodeSelectForm = new NodeSelectForm<T>( farcArchiveNode ) )
            {
                nodeSelectForm.Text = title;

                if ( nodeSelectForm.NodeCount == 0 )
                    MessageBox.Show( errorOnEmpty, Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error );

                else if ( nodeSelectForm.NodeCount == 1 )
                    return ( T ) nodeSelectForm.TopNode.Data;

                else if ( nodeSelectForm.ShowDialog() == DialogResult.OK )
                    return ( T ) nodeSelectForm.SelectedNode.Data;
            }

            return null;
        }

        protected override void Initialize()
        {
            AddReplaceHandler<FarcArchive>( filePath => PromptFarcArchiveViewForm( filePath ) );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                foreach ( var node in Nodes )
                    SetSubscription( node, true );
            }

            base.Dispose( disposing );
        }

        protected BinaryFileNode( string name, T data ) : base( name, data )
        {
        }

        protected BinaryFileNode( string name, Func<Stream> streamGetter ) : base( name, new T() )
        {
            mStreamGetter = streamGetter;
        }

        private class DynamicStream : Stream
        {
            private readonly BinaryFileNode<T> mNode;
            private Stream mStream;

            public override bool CanRead
            {
                get
                {
                    EnsureNotNull();
                    return mStream.CanRead;
                }
            }

            public override bool CanSeek
            {
                get
                {
                    EnsureNotNull();
                    return mStream.CanSeek;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    EnsureNotNull();
                    return mStream.CanWrite;
                }
            }

            public override long Length
            {
                get
                {
                    EnsureNotNull();
                    return mStream.Length;
                }
            }

            public override long Position
            {
                get
                {
                    EnsureNotNull();
                    return mStream.Position;
                }

                set
                {
                    EnsureNotNull();
                    mStream.Position = value;
                }
            }

            public override void Flush()
            {
                EnsureNotNull();
                mStream.Flush();
            }

            public override int Read( byte[] buffer, int offset, int count )
            {
                EnsureNotNull();
                return mStream.Read( buffer, 0, count );
            }

            public override long Seek( long offset, SeekOrigin origin )
            {
                EnsureNotNull();
                return mStream.Seek( offset, origin );
            }

            public override void SetLength( long value )
            {
                EnsureNotNull();
                mStream.SetLength( value );
            }

            public override void Write( byte[] buffer, int offset, int count )
            {
                EnsureNotNull();
                mStream.Write( buffer, 0, count );
            }

            protected override void Dispose( bool disposing )
            {
                if ( disposing )
                    mStream?.Close();

                base.Dispose( disposing );
            }

            private void EnsureNotNull()
            {
                if ( mStream != null )
                    return;

                mStream = new MemoryStream();
                {
                    sModule.Export( mNode.Data, mStream, mNode.Name );
                    mNode.IsDirty = false;
                }
                mStream.Position = 0;
            }

            public DynamicStream( BinaryFileNode<T> node )
            {
                mNode = node;
            }
        }
    }
}