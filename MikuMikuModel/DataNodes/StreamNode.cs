using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace MikuMikuModel.DataNodes
{
    public class StreamNode : DataNode<Stream>
    {
        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Leaf; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get
            {
                return
                  DataNodeActionFlags.Export | DataNodeActionFlags.Move |
                  DataNodeActionFlags.Remove | DataNodeActionFlags.Rename | DataNodeActionFlags.Replace;
            }
        }

        public override Bitmap Icon
        {
            get { return Properties.Resources.File; }
        }

        [DisplayName( "File path" )]
        public string FilePath
        {
            get { return ( Data as FileStream )?.Name; }
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( ( path ) =>
            {
                using ( var stream = File.Create( path ) )
                {
                    Data.Seek( 0, SeekOrigin.Begin );
                    Data.CopyTo( stream );
                }
            } );
            RegisterReplaceHandler<Stream>( File.OpenRead );
        }

        protected override void InitializeViewCore()
        {
        }

        public StreamNode( string name, Stream data ) : base( name, data )
        {
        }
    }
}
