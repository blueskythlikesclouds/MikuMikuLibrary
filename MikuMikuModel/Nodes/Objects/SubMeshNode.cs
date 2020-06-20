using System.ComponentModel;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Objects;

namespace MikuMikuModel.Nodes.Objects
{
    public class SubMeshNode : Node<SubMesh>
    {
        public override NodeFlags Flags => NodeFlags.None;

        [Category( "General" )]
        [DisplayName( "Bounding sphere" )]
        public BoundingSphere BoundingSphere
        {
            get => GetProperty<BoundingSphere>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Material index" )]
        public uint MaterialIndex
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Material texture coordinate indices" )]
        public byte[] TexCoordIndices
        {
            get => GetProperty<byte[]>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Bone indices" )]
        public ushort[] BoneIndices
        {
            get => GetProperty<ushort[]>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Bones per vertex" )]
        public uint BonesPerVertex
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Primitive type" )]
        public PrimitiveType PrimitiveType
        {
            get => GetProperty<PrimitiveType>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Index format" )]
        public IndexFormat IndexFormat
        {
            get => GetProperty<IndexFormat>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public uint[] Indices
        {
            get => GetProperty<uint[]>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Flags" )]
        public SubMeshFlags SubMeshFlags
        {
            get => GetProperty<SubMeshFlags>( nameof( SubMesh.Flags ) );
            set => SetProperty( value, nameof( SubMesh.Flags ) );
        }

        [Category( "General" )]
        [DisplayName( "Index offset" )]
        public uint IndexOffset
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Bounding box" )]
        public BoundingBox BoundingBox
        {
            get => GetProperty<BoundingBox>();
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

        public SubMeshNode( string name, SubMesh data ) : base( name, data )
        {
        }
    }
}