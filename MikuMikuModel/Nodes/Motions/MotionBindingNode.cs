using MikuMikuLibrary.Motions;
using MikuMikuModel.Nodes.Collections;

namespace MikuMikuModel.Nodes.Motions
{
    public class MotionBindingNode : Node<MotionBinding>
    {
        public override NodeFlags Flags => NodeFlags.Add;

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<BoneBinding>( "Bone bindings", Data.BoneBindings, x => x.Name ) );
            Nodes.Add( new BoneBindingNode( "Global transformation", Data.GlobalTransformation ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public MotionBindingNode( string name, MotionBinding data ) : base( name, data )
        {
        }
    }
}