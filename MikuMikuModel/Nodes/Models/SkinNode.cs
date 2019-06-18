using MikuMikuLibrary.Objects;
using MikuMikuModel.Nodes.Misc;

namespace MikuMikuModel.Nodes.Models
{
    public class SkinNode : Node<Skin>
    {
        public override NodeFlags Flags => NodeFlags.Add;

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<BoneInfo>( "Bones", Data.Bones, x => x.Name ) );

            if ( Data.ExData != null )
                Nodes.Add( new ExDataNode( "Ex data", Data.ExData ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public SkinNode( string name, Skin data ) : base( name, data )
        {
        }
    }
}