using MikuMikuLibrary.Models;
using System.ComponentModel;

namespace MikuMikuModel.DataNodes
{
    [DataNodeSpecialName( "Index Table" )]
    public class IndexTableNode : DataNode<IndexTable>
    {
        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Leaf; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get { return DataNodeActionFlags.Move | DataNodeActionFlags.Remove | DataNodeActionFlags.Rename; }
        }

        [DisplayName( "Bounding Sphere" )]
        public BoundingSphere BoundingSphere
        {
            get { return GetProperty<BoundingSphere>(); }
            set { SetProperty( value ); }
        }
        public ushort[] Indices
        {
            get { return GetProperty<ushort[]>(); }
            set { SetProperty( value ); }
        }
        [DisplayName( "Bone Indices" )]
        public ushort[] BoneIndices
        {
            get { return GetProperty<ushort[]>(); }
            set { SetProperty( value ); }
        }
        public int MaterialIndex
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }
        [DisplayName( "Primitive Type" )]
        public IndexTablePrimitiveType PrimitiveType
        {
            get { return GetProperty<IndexTablePrimitiveType>(); }
            set { SetProperty( value ); }
        }
        [DisplayName( "Bounding Box" )]
        public BoundingBox BoundingBox
        {
            get { return GetProperty<BoundingBox>(); }
            set { SetProperty( value ); }
        }
        public float Field00
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
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
