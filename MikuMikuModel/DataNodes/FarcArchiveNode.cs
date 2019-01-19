using MikuMikuLibrary.Archives.Farc;

namespace MikuMikuModel.DataNodes
{
    [DataNodePrettyName( "FARC Archive" )]
    public class FarcArchiveNode : ArchiveNode<FarcArchive>
    {
        public FarcArchiveNode( string name, FarcArchive data ) : base( name, data )
        {
        }
    }
}
