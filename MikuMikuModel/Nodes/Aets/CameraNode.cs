using MikuMikuLibrary.Aets;

namespace MikuMikuModel.Nodes.Aets
{
    public class CameraNode : Node<Camera>
    {
        public override NodeFlags Flags => NodeFlags.Add;

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new FCurveNode( "Eye X", Data.EyeX ) );
            Nodes.Add( new FCurveNode( "Eye Y", Data.EyeY ) );
            Nodes.Add( new FCurveNode( "Eye Z", Data.EyeZ ) );
            Nodes.Add( new FCurveNode( "Position X", Data.PositionX ) );
            Nodes.Add( new FCurveNode( "Position Y", Data.PositionY ) );
            Nodes.Add( new FCurveNode( "Position Z", Data.PositionZ ) );
            Nodes.Add( new FCurveNode( "Direction X", Data.DirectionX ) );
            Nodes.Add( new FCurveNode( "Direction Y", Data.DirectionY ) );
            Nodes.Add( new FCurveNode( "Direction Z", Data.DirectionZ ) );
            Nodes.Add( new FCurveNode( "Rotation X", Data.RotationX ) );
            Nodes.Add( new FCurveNode( "Rotation Y", Data.RotationY ) );
            Nodes.Add( new FCurveNode( "Rotation Z", Data.RotationZ ) );
            Nodes.Add( new FCurveNode( "Zoom", Data.Zoom ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public CameraNode( string name, Camera data ) : base( name, data )
        {
        }
    }
}