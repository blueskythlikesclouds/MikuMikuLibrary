using MikuMikuLibrary.Aets;
using MikuMikuModel.Nodes.Collections;

namespace MikuMikuModel.Nodes.Aets
{
    public class CompositionNode : Node<Composition>
    {
        public override NodeFlags Flags => NodeFlags.Add;

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<Layer>( "Layers", Data.Layers, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public CompositionNode( string name, Composition data ) : base( name, data )
        {
        }
    }
}