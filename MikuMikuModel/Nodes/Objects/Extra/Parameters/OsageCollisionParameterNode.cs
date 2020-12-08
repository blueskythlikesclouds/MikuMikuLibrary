using System.ComponentModel;
using MikuMikuLibrary.Objects.Extra.Parameters;

namespace MikuMikuModel.Nodes.Objects.Extra.Parameters
{
    public class OsageCollisionParameterNode : Node<OsageCollisionParameter>
    {
        public override NodeFlags Flags => NodeFlags.Add;

        [Category( "General" )]
        public uint Type
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public float Radius
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new OsageCollisionBoneParameterNode( Data.Bone0.Name, Data.Bone0 ) );
            Nodes.Add( new OsageCollisionBoneParameterNode( Data.Bone1.Name, Data.Bone1 ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public OsageCollisionParameterNode( string name, OsageCollisionParameter data ) : base( name, data )
        {
        }
    }
}