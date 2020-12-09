using MikuMikuLibrary.Aets;

namespace MikuMikuModel.Nodes.Aets
{
    public class LayerVideo3DNode : Node<LayerVideo3D>
    {
        public override NodeFlags Flags => NodeFlags.Add;

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new FCurveNode( "Anchor Z", Data.AnchorZ ) );
            Nodes.Add( new FCurveNode( "Position Z", Data.PositionZ ) );
            Nodes.Add( new FCurveNode( "Direction X", Data.DirectionX ) );
            Nodes.Add( new FCurveNode( "Direction Y", Data.DirectionY ) );
            Nodes.Add( new FCurveNode( "Direction Z", Data.DirectionZ ) );
            Nodes.Add( new FCurveNode( "Rotation X", Data.RotationX ) );
            Nodes.Add( new FCurveNode( "Rotation Y", Data.RotationY ) );
            Nodes.Add( new FCurveNode( "Scale Z", Data.ScaleZ ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public LayerVideo3DNode( string name, LayerVideo3D data ) : base( name, data )
        {
        }
    }
}