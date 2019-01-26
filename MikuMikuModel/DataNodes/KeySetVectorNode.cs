using System.ComponentModel;
using MikuMikuLibrary.Motions;

namespace MikuMikuModel.DataNodes
{
    public class KeySetVectorNode : DataNode<KeySetVector>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        [Browsable( false )]
        public KeySetNode XNode { get; set; }

        [Browsable( false )]
        public KeySetNode YNode { get; set; }

        [Browsable( false )]
        public KeySetNode ZNode { get; set; }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
            Add( XNode = new KeySetNode( "X", Data.X ) );
            Add( YNode = new KeySetNode( "Y", Data.Y ) );
            Add( ZNode = new KeySetNode( "Z", Data.Z ) );
        }

        public KeySetVectorNode( string name, KeySetVector data ) : base( name, data )
        {
        }

    }
}