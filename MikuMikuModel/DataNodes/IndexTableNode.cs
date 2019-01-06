using MikuMikuLibrary.Maths;
using MikuMikuLibrary.Models;
using System.ComponentModel;

namespace MikuMikuModel.DataNodes
{
    [DataNodeSpecialName( "Index Table" )]
    public class IndexTableNode : DataNode<IndexTable>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Leaf;

        public override DataNodeActionFlags ActionFlags =>
            DataNodeActionFlags.Move | DataNodeActionFlags.Remove | DataNodeActionFlags.Rename;

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
        public int MaterialIndex
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }
        [DisplayName( "Primitive type" )]
        public IndexTablePrimitiveType PrimitiveType
        {
            get => GetProperty<IndexTablePrimitiveType>();
            set => SetProperty( value );
        }
        [DisplayName( "Bounding box" )]
        public BoundingBox BoundingBox
        {
            get => GetProperty<BoundingBox>();
            set => SetProperty( value );
        }
        public float Field00
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        public IndexTableNode( string name, IndexTable data ) : base( name, data )
        {
        }
    }
}
