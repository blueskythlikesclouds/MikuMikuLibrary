using System.ComponentModel;
using MikuMikuLibrary.Motions;

namespace MikuMikuModel.DataNodes
{
    public class KeyControllerNode : DataNode<KeyController>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        public string Target => GetProperty<string>();

        [Browsable( false )]
        public KeySetVectorNode PositionNode { get; set; }

        [Browsable( false )]
        public KeySetVectorNode RotationNode { get; set; }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
            if ( Data.Position != null )
                Add( PositionNode = new KeySetVectorNode( "Position", Data.Position ) );

            if ( Data.Rotation != null )
                Add( RotationNode = new KeySetVectorNode( "Rotation", Data.Rotation ) );
        }

        public KeyControllerNode( string name, KeyController data ) : base( name, data )
        {
        }
    }
}