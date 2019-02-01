using System.ComponentModel;
using System.Numerics;
using MikuMikuLibrary.Models;

namespace MikuMikuModel.Nodes.Models
{
    public class BoneNode : Node<Bone>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [DisplayName( "Parent ID" )]
        public int ParentID
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        public int ID
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        public Matrix4x4 Matrix
        {
            get => GetProperty<Matrix4x4>();
            set => SetProperty( value );
        }

        [DisplayName( "Is ex" )]
        public bool IsEx => GetProperty<bool>();

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public BoneNode( string name, Bone data ) : base( name, data )
        {
        }
    }
}