using MikuMikuLibrary.Models;
using System.ComponentModel;

namespace MikuMikuModel.DataNodes
{
    [DataNodeSpecialName( "Mesh Skin" )]
    public class MeshSkinNode : DataNode<MeshSkin>
    {
        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Branch; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get { return DataNodeActionFlags.None; }
        }

        [Browsable( false )]
        public ListNode<Bone> Bones { get; set; }

        protected override void InitializeCore()
        {
            RegisterDataUpdateHandler( () =>
            {
                var data = new MeshSkin();
                data.Bones.AddRange( Bones.Data );
                data.ExData = Data.ExData;
                return data;
            } );
        }

        protected override void InitializeViewCore()
        {
            Add( Bones = new ListNode<Bone>( nameof( Data.Bones ), Data.Bones ) );
        }

        public MeshSkinNode( string name, MeshSkin data ) : base( name, data )
        {
        }
    }
}
