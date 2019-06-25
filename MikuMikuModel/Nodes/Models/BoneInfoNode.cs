using System.ComponentModel;
using System.Numerics;
using MikuMikuLibrary.Objects;

namespace MikuMikuModel.Nodes.Models
{
    public class BoneInfoNode : Node<BoneInfo>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [DisplayName( "Parent id" )]
        public int ParentId
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        public int Id
        {
            get => GetProperty<int>();
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