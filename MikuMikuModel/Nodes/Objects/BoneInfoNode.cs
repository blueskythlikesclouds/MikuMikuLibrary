using System.ComponentModel;
using System.Numerics;
using MikuMikuLibrary.Objects;
using MikuMikuModel.Nodes.TypeConverters;

namespace MikuMikuModel.Nodes.Objects
{
    public class BoneInfoNode : Node<BoneInfo>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [DisplayName( "Parent name" )] public string ParentBone => 
            GetProperty<BoneInfo>( nameof( BoneInfo.Parent ) )?.Name;

        [TypeConverter( typeof( IdTypeConverter ) )]
        public uint Id
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [DisplayName( "Inverse bind pose matrix" )]
        public Matrix4x4 InverseBindPoseMatrix
        {
            get => GetProperty<Matrix4x4>();
            set => SetProperty( value );
        }

        [DisplayName( "Belongs in ex data" )] public bool IsEx => GetProperty<bool>();

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public BoneInfoNode( string name, BoneInfo data ) : base( name, data )
        {
        }
    }
}