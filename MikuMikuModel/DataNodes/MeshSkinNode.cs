using MikuMikuLibrary.Models;
using System.ComponentModel;

namespace MikuMikuModel.DataNodes
{
    [DataNodeSpecialName( "Mesh Skin" )]
    public class MeshSkinNode : DataNode<MeshSkin>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;
        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        [Browsable( false )]
        public ListNode<Bone> Bones { get; set; }

        [Browsable( false )]
        public MeshExDataNode ExData { get; set; }

        protected override void InitializeCore()
        {
            RegisterDataUpdateHandler( () =>
            {
                var data = new MeshSkin();
                data.Bones.AddRange( Bones.Data );
                data.ExData = ExData?.Data;
                return data;
            } );
        }

        protected override void InitializeViewCore()
        {
            Add( Bones = new ListNode<Bone>( nameof( Data.Bones ), Data.Bones ) );

            if ( Data.ExData != null )
                Add( ExData = new MeshExDataNode( "Ex data", Data.ExData ) );
        }

        public MeshSkinNode( string name, MeshSkin data ) : base( name, data )
        {
        }
    }
}
