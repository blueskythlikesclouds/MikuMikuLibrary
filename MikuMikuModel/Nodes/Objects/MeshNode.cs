using System.ComponentModel;
using System.Numerics;
using System.Windows.Forms;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Objects;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Nodes.Collections;

namespace MikuMikuModel.Nodes.Objects
{
    public class MeshNode : Node<Mesh>
    {
        public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Rename;

        public override Control Control
        {
            get
            {
                var objectSetParent = FindParent<ObjectSetNode>();
                var objectParent = FindParent<ObjectNode>();

                if ( objectSetParent == null || objectParent == null )
                    return null;

                ModelViewControl.Instance.SetModel( Data, objectParent.Data, objectSetParent.Data.TextureSet );
                return ModelViewControl.Instance;
            }
        }

        [Category( "General" )]
        [DisplayName( "Bounding sphere" )]
        public BoundingSphere BoundingSphere
        {
            get => GetProperty<BoundingSphere>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public Vector3[] Positions
        {
            get => GetProperty<Vector3[]>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public Vector3[] Normals
        {
            get => GetProperty<Vector3[]>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public Vector4[] Tangents
        {
            get => GetProperty<Vector4[]>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Texture coordinates 1" )]
        public Vector2[] TexCoords0
        {
            get => GetProperty<Vector2[]>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Texture coordinates 2" )]
        public Vector2[] TexCoords1
        {
            get => GetProperty<Vector2[]>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Texture coordinates 3" )]
        public Vector2[] TexCoords2
        {
            get => GetProperty<Vector2[]>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Texture coordinates 4" )]
        public Vector2[] TexCoords3
        {
            get => GetProperty<Vector2[]>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Colors 1" )]
        public Color[] Colors0
        {
            get => GetProperty<Color[]>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Colors 2" )]
        public Color[] Colors1
        {
            get => GetProperty<Color[]>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Bone weights" )]
        public BoneWeight[] BoneWeights
        {
            get => GetProperty<BoneWeight[]>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Flags" )]
        public MeshFlags MeshFlags
        {
            get => GetProperty<MeshFlags>( nameof( Mesh.Flags ) );
            set => SetProperty( value, nameof( Mesh.Flags ) );
        }

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<SubMesh>( "Submeshes", Data.SubMeshes ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public MeshNode( string name, Mesh data ) : base( name, data )
        {
        }
    }
}