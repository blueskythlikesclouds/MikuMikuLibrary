using MikuMikuLibrary.Motions;

namespace MikuMikuModel.Nodes.Motions
{
    public class KeySetVectorNode : Node<KeySetVector>
    {
        public override NodeFlags Flags => NodeFlags.Add;

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            if ( Data.X != null )
                Nodes.Add( new KeySetNode( "X", Data.X ) );
            if ( Data.Y != null )
                Nodes.Add( new KeySetNode( "Y", Data.Y ) );
            if ( Data.Z != null )
                Nodes.Add( new KeySetNode( "Z", Data.Z ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public KeySetVectorNode( string name, KeySetVector data ) : base( name, data )
        {
        }
    }
}