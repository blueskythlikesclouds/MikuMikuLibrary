using MikuMikuLibrary.Models;
using System.ComponentModel;

namespace MikuMikuModel.DataNodes
{
    [DataNodePrettyName( "Mesh Skin" )]
    public class SkinNode : DataNode<Skin>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;
        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        [Browsable( false )]
        public ListNode<Bone> Bones { get; set; }

        [Browsable( false )]
        public ExDataNode ExData { get; set; }

        protected override void InitializeCore()
        {
            RegisterDataUpdateHandler( () =>
            {
                var data = new Skin();
                data.Bones.AddRange( Bones.Data );
                data.ExData = ExData?.Data;
                return data;
            } );
        }

        protected override void InitializeViewCore()
        {
            Add( Bones = new ListNode<Bone>( nameof( Data.Bones ), Data.Bones ) );

            if ( Data.ExData != null )
                Add( ExData = new ExDataNode( "Ex data", Data.ExData ) );
        }

        public SkinNode( string name, Skin data ) : base( name, data )
        {
        }
    }
}
