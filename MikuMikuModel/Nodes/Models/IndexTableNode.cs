using System.ComponentModel;
using MikuMikuLibrary.Maths;
using MikuMikuLibrary.Models;

namespace MikuMikuModel.Nodes.Models
{
    public class IndexTableNode : Node<IndexTable>
    {
        public override NodeFlags Flags => NodeFlags.None;

        [DisplayName( "Bounding sphere" )]
        public BoundingSphere BoundingSphere
        {
            get => GetProperty<BoundingSphere>();
            set => SetProperty( value );
        }

        public ushort[] Indices
        {
            get => GetProperty<ushort[]>();
            set => SetProperty( value );
        }

        [DisplayName( "Bone indices" )]
        public ushort[] BoneIndices
        {
            get => GetProperty<ushort[]>();
            set => SetProperty( value );
        }

        [DisplayName( "Material index" )]
        public int MaterialIndex
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        [DisplayName( "Primitive type" )]
        public PrimitiveType PrimitiveType
        {
            get => GetProperty<PrimitiveType>();
            set => SetProperty( value );
        }

        [DisplayName( "Bounding box" )]
        public BoundingBox BoundingBox
        {
            get => GetProperty<BoundingBox>();
            set => SetProperty( value );
        }

        public int Field00
        {
            get => GetProperty<int>();
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

        public IndexTableNode( string name, IndexTable data ) : base( name, data )
        {
        }
    }
}