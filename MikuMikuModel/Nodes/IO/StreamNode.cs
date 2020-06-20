using System.Drawing;
using System.IO;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.IO
{
    public class StreamNode : Node<Stream>
    {
        public override NodeFlags Flags =>
            NodeFlags.Export | NodeFlags.Move | NodeFlags.Remove | NodeFlags.Rename | NodeFlags.Replace;

        public override Bitmap Image =>
            ResourceStore.LoadBitmap( "Icons/File.png" );

        protected override void Initialize()
        {
            AddExportHandler<Stream>( filePath =>
            {
                using ( var stream = File.Create( filePath ) )
                {
                    if ( Data.CanSeek )
                        Data.Seek( 0, SeekOrigin.Begin );

                    Data.CopyTo( stream );
                }
            } );
            AddReplaceHandler<Stream>( File.OpenRead );
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public StreamNode( string name, Stream data ) : base( name, data )
        {
        }
    }
}