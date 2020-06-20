using MikuMikuLibrary.Motions;

namespace MikuMikuModel.Nodes.Motions
{
    public class KeyBindingNode : Node<KeyBinding>
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

        public KeyBindingNode( string name, KeyBinding data ) : base( name, data )
        {
        }
    }
}