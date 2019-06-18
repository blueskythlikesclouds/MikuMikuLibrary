using MikuMikuLibrary.Motions;

namespace MikuMikuModel.Nodes.Motions
{
    public class BoneBindingNode : Node<BoneBinding>
    {
        public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Rename;

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            if ( Data.Position != null )
                Nodes.Add( new KeyBindingNode( "Position", Data.Position ) );

            if ( Data.Rotation != null )
                Nodes.Add( new KeyBindingNode( "Rotation", Data.Rotation ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public BoneBindingNode( string name, BoneBinding data ) : base( name, data )
        {
        }
    }
}