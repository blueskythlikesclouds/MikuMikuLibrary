using MikuMikuLibrary.Aets;

namespace MikuMikuModel.Nodes.Aets
{
    public class LayerVideoNode : Node<LayerVideo>
    {
        public override NodeFlags Flags => NodeFlags.Add;

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new TransferModeNode( "Transfer mode", Data.TransferMode ) );
            Nodes.Add( new FCurveNode( "Anchor X", Data.AnchorX ) );
            Nodes.Add( new FCurveNode( "Anchor Y", Data.AnchorY ) );
            Nodes.Add( new FCurveNode( "Position X", Data.PositionX ) );
            Nodes.Add( new FCurveNode( "Position Y", Data.PositionY ) );
            Nodes.Add( new FCurveNode( "Rotation", Data.Rotation ) );
            Nodes.Add( new FCurveNode( "Scale X", Data.ScaleX ) );
            Nodes.Add( new FCurveNode( "Scale Y", Data.ScaleY ) );
            Nodes.Add( new FCurveNode( "Opacity", Data.Opacity ) );

            if ( Data.Video3D != null )
                Nodes.Add( new LayerVideo3DNode( "3D", Data.Video3D ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public LayerVideoNode( string name, LayerVideo data ) : base( name, data )
        {
        }
    }
}