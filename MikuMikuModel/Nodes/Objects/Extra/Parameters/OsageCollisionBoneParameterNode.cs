using System.ComponentModel;
using System.Numerics;
using MikuMikuLibrary.Objects.Extra.Parameters;
using MikuMikuModel.Nodes.TypeConverters;

namespace MikuMikuModel.Nodes.Objects.Extra.Parameters
{
    public class OsageCollisionBoneParameterNode : Node<OsageCollisionBoneParameter>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [Category( "General" )]
        [TypeConverter( typeof( Vector3TypeConverter ) )]
        public Vector3 Position
        {
            get => GetProperty<Vector3>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public OsageCollisionBoneParameterNode( string name, OsageCollisionBoneParameter data ) : base( name, data )
        {
        }
    }
}