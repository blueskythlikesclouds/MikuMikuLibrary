using MikuMikuLibrary.Motions;

namespace MikuMikuModel.Nodes.Motions
{
    public class KeyControllerNode : Node<KeyController>
    {
        public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Rename;

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            if ( Data.Position != null )
                Nodes.Add( new KeySetVectorNode( "Position", Data.Position ) );

            if ( Data.Rotation != null )
                Nodes.Add( new KeySetVectorNode( "Rotation", Data.Rotation ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public KeyControllerNode( string name, KeyController data ) : base( name, data )
        {
        }
    }
}