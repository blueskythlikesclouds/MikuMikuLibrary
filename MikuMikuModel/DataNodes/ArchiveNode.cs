using MikuMikuLibrary.Archives;
using MikuMikuLibrary.IO;
using MikuMikuModel.FormatModules;
using MikuMikuModel.Resources;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MikuMikuModel.DataNodes
{
    // Only string archives for now
    public abstract class ArchiveNode<TArchive> : DataNode<TArchive> where TArchive : IArchive<string>, new()
    {
        private DataNodeActionFlags mFlags;

        public override DataNodeFlags Flags => DataNodeFlags.Branch;
        public override DataNodeActionFlags ActionFlags => mFlags;
        public override Bitmap Icon => ResourceStore.LoadBitmap( "Icons/Archive.png" );

        protected override void InitializeCore()
        {
            if ( Data.Flags.HasFlag( BinaryFileFlags.Load ) )
            {
                mFlags |= DataNodeActionFlags.Replace;
                RegisterReplaceHandler<TArchive>( ( path ) => BinaryFile.Load<TArchive>( path ) );
            }
            if ( Data.Flags.HasFlag( BinaryFileFlags.Save ) )
            {
                mFlags |= DataNodeActionFlags.Export;
                RegisterExportHandler<TArchive>( ( path ) => Data.Save( path ) );
            }
            if ( Data.CanAdd )
            {
                mFlags |= DataNodeActionFlags.Import | DataNodeActionFlags.Move;
                RegisterImportHandler<Stream>( ( path ) => DataNodeFactory.Create( path ) );
            }
            if ( Data.CanRemove )
            {
                mFlags |= DataNodeActionFlags.Remove;
            }

            RegisterCustomHandler( "Export All", () =>
            {
                using ( var saveFileDialog = new SaveFileDialog() )
                {
                    saveFileDialog.AutoUpgradeEnabled = true;
                    saveFileDialog.CheckPathExists = true;
                    saveFileDialog.Title = "Select a folder to export textures to.";
                    saveFileDialog.FileName = "Enter into a directory and press Save";

                    if ( saveFileDialog.ShowDialog() == DialogResult.OK )
                    {
                    }
                }
            }, Keys.Control | Keys.Shift | Keys.E );

            RegisterDataUpdateHandler( () =>
            {
                // We're gonna work with the original data always
                foreach ( var node in Nodes )
                {
                    if ( node.HadAnyChanges )
                    {
                        node.HadAnyChanges = false;

                        Data.Add( node.Name, new FormatModuleStream( node.Data, node.Name ), false,
                            ConflictPolicy.Replace );
                    }
                }

                return Data;
            } );
        }

        protected override void InitializeViewCore()
        {
            foreach ( var handle in Data )
                Add( DataNodeFactory.Create( Data.Open( handle, EntryStreamMode.MemoryStream ), handle ) );
        }

        public ArchiveNode( string name, TArchive data ) : base( name, data )
        {
        }

        // A simple format module class that wraps around a format module's stream
        private class FormatModuleStream : Stream
        {
            private readonly object mObject;
            private readonly string mName;

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

            public void EnsureNotNull()
            {
                if ( mStream == null )
                {
                    mStream = FormatModuleUtilities.ExportToStream( mName, mObject );
                    mStream.Position = 0;
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
                {
                    mStream?.Dispose();
                }

                base.Dispose( disposing );
            }

            public FormatModuleStream( object obj, string name )
            {
                mObject = obj;
                mName = name;
            }
        }
    }
}
