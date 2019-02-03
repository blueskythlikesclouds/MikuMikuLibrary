using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MikuMikuLibrary.Archives.Farc;
using MikuMikuLibrary.IO;
using MikuMikuModel.Configurations;
using MikuMikuModel.GUI.Forms;
using MikuMikuModel.Modules;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.IO
{
    public abstract class BinaryFileNode<T> : Node<T>, IDirtyNode where T : class, IBinaryFile, new()
    {
        private static readonly IFormatModule sModule = FormatModuleRegistry.ModulesByType[ typeof( T ) ];

        private readonly Func<Stream> mStreamGetter;
        private bool mLoaded;
        private bool mDirty;

        public event EventHandler DirtyStateChanged;

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

        public Endianness Endianness
        {
            get => GetProperty<Endianness>();
            set => SetProperty( value );
        }

        public BinaryFormat Format
        {
            get => GetProperty<BinaryFormat>();
            set => SetProperty( value );
        }

        public virtual Stream GetStream() => new DynamicStream( this );

        protected virtual void Load( T data, Stream source ) => 
            data.Load( source ); 

        private void InitializeSubscription( INode node, bool unsubscribe )
        {
            if ( node.IsPopulated || unsubscribe && node.IsPopulated )
                IsDirty = true;

            if ( unsubscribe )
            {
                if ( node is IDirtyNode dirtyNode )
                    dirtyNode.DirtyStateChanged -= OnNodeDirtyStateChanged;
                else
                {
                    node.PropertyChanged -= OnNodePropertyChanged;
                    node.Added -= OnNodeAdded;
                    node.Removed -= OnNodeRemoved;
                    node.Replaced -= OnNodeReplaced;
                    node.Moved -= OnNodeMoved;

                    foreach ( var childNode in node.Nodes )
                        InitializeSubscription( childNode, true );
                }
            }
            else
            {
                if ( node is IDirtyNode dirtyNode )
                    dirtyNode.DirtyStateChanged += OnNodeDirtyStateChanged;
                else
                {
                    node.PropertyChanged += OnNodePropertyChanged;
                    node.Added += OnNodeAdded;
                    node.Removed += OnNodeRemoved;
                    node.Replaced += OnNodeReplaced;
                    node.Moved += OnNodeMoved;
                }
            }

            void OnNodeDirtyStateChanged( object sender, EventArgs args ) =>
                IsDirty = ( ( IDirtyNode ) sender ).IsDirty || IsDirty;

            void OnNodePropertyChanged( object sender, PropertyChangedEventArgs args ) => IsDirty = true;
            void OnNodeAdded( object sender, NodeAddEventArgs args ) => InitializeSubscription( args.AddedNode, false );
            void OnNodeRemoved( object sender, NodeRemoveEventArgs args ) => IsDirty = true;
            void OnNodeReplaced( object sender, NodeReplaceEventArgs args ) => IsDirty = true;
            void OnNodeMoved( object sender, NodeMoveEventArgs args ) => IsDirty = true;
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

        protected override void OnAdd( INode addedNode )
        {
            InitializeSubscription( addedNode, false );
            base.OnAdd( addedNode );
        }

        protected override void OnRemove( INode removedNode )
        {
            InitializeSubscription( removedNode, true );
            base.OnRemove( removedNode );
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

        protected override void Initialize()
        {
            RegisterReplaceHandler<FarcArchive>( filePath =>
            {
                var farcArchive = BinaryFile.Load<FarcArchive>( filePath );
                using ( var farcArchiveViewForm = new FarcArchiveViewForm<T>( farcArchive ) )
                {
                    farcArchiveViewForm.Text = "Select a node to replace with.";

                    if ( farcArchiveViewForm.NodeCount == 0 )
                        MessageBox.Show( "This archive has no entries that you could replace the node with.", "Miku Miku Model",
                            MessageBoxButtons.OK, MessageBoxIcon.Information );
                        
                    else if ( farcArchiveViewForm.NodeCount == 1 )
                        return ( T ) farcArchiveViewForm.TopNode.Data;

                    else if ( farcArchiveViewForm.ShowDialog() == DialogResult.OK )
                        return ( T ) farcArchiveViewForm.SelectedNode.Data;
                }

                return null;
            } );
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
                    mStream?.Dispose();

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