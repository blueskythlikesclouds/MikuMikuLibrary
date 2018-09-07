using MikuMikuLibrary.Archives;
using MikuMikuLibrary.IO;
using MikuMikuModel.FormatModules;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MikuMikuModel.DataNodes
{
    // Only string archives for now
    public abstract class ArchiveNode<TArchive> : DataNode<TArchive> where TArchive : IArchive<string>
    {
        private DataNodeActionFlags flags;
        private Dictionary<DataNode, object> valueMap;

        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Branch; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get { return flags; }
        }

        public override Bitmap Icon
        {
            get { return Properties.Resources.Archive; }
        }

        protected override void InitializeCore()
        {
            valueMap = new Dictionary<DataNode, object>();

            if ( Data.Flags.HasFlag( BinaryFileFlags.Load ) )
            {
                flags |= DataNodeActionFlags.Replace;
                RegisterReplaceHandler<TArchive>( ( path ) => BinaryFile.Load<TArchive>( path ) );
            }
            if ( Data.Flags.HasFlag( BinaryFileFlags.Save ) )
            {
                flags |= DataNodeActionFlags.Export;
                RegisterExportHandler<TArchive>( ( path ) => Data.Save( path ) );
            }
            if ( Data.CanAdd )
            {
                flags |= DataNodeActionFlags.Import | DataNodeActionFlags.Move;
                RegisterImportHandler<Stream>( ( path ) => DataNodeFactory.Create( path ) );
            }
            if ( Data.CanRemove )
            {
                flags |= DataNodeActionFlags.Remove;
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
                    Stream stream = null;

                    bool exists;
                    if ( ( exists = valueMap.TryGetValue( node, out object value ) && !value.Equals( node.Data ) ) || !exists )
                        stream = new FormatModuleStream( node.Data, node.Name );

                    if ( stream != null )
                    {
                        Data.Add( node.Name, stream, false, ConflictPolicy.Replace );
                        valueMap[ node ] = node.Data;
                    }
                }

                return Data;
            } );
        }

        protected override void InitializeViewCore()
        {
            foreach ( var handle in Data )
            {
                var node = DataNodeFactory.Create( Data.Open( handle, EntryStreamMode.MemoryStream ), handle );
                valueMap[ node ] = node.Data;
                Add( node );
            }
        }

        public ArchiveNode( string name, TArchive data ) : base( name, data )
        {
        }

        // A simple format module class that wraps around a format module's stream
        private class FormatModuleStream : Stream
        {
            private readonly object obj;
            private readonly string name;

            private Stream stream;

            public override bool CanRead
            {
                get
                {
                    EnsureNotNull();
                    return stream.CanRead;
                }
            }

            public override bool CanSeek
            {
                get
                {
                    EnsureNotNull();
                    return stream.CanSeek;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    EnsureNotNull();
                    return stream.CanWrite;
                }
            }

            public override long Length
            {
                get
                {
                    EnsureNotNull();
                    return stream.Length;
                }
            }

            public override long Position
            {
                get
                {
                    EnsureNotNull();
                    return stream.Position;
                }

                set
                {
                    EnsureNotNull();
                    stream.Position = value;
                }
            }

            public void EnsureNotNull()
            {
                if ( stream == null )
                {
                    stream = FormatModuleUtilities.ExportToStream( name, obj );
                    stream.Position = 0;
                }
            }

            public override void Flush()
            {
                EnsureNotNull();
                stream.Flush();
            }

            public override int Read( byte[] buffer, int offset, int count )
            {
                EnsureNotNull();
                return stream.Read( buffer, 0, count );
            }

            public override long Seek( long offset, SeekOrigin origin )
            {
                EnsureNotNull();
                return stream.Seek( offset, origin );
            }

            public override void SetLength( long value )
            {
                EnsureNotNull();
                stream.SetLength( value );
            }

            public override void Write( byte[] buffer, int offset, int count )
            {
                EnsureNotNull();
                stream.Write( buffer, 0, count );
            }

            protected override void Dispose( bool disposing )
            {
                if ( disposing )
                {
                    stream?.Dispose();
                }

                base.Dispose( disposing );
            }

            public FormatModuleStream( object obj, string name )
            {
                this.obj = obj;
                this.name = name;
            }
        }
    }
}
