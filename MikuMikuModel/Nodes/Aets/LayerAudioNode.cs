using MikuMikuLibrary.Aets;

namespace MikuMikuModel.Nodes.Aets
{
    public class LayerAudioNode : Node<LayerAudio>
    {
        public override NodeFlags Flags => NodeFlags.Add;

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new FCurveNode( "Volume L", Data.VolumeL ) );
            Nodes.Add( new FCurveNode( "Volume R", Data.VolumeR ) );
            Nodes.Add( new FCurveNode( "Pan L", Data.PanL ) );
            Nodes.Add( new FCurveNode( "Pan R", Data.PanR ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public LayerAudioNode( string name, LayerAudio data ) : base( name, data )
        {
        }
    }
}