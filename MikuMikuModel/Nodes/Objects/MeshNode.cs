using System.ComponentModel;
using System.Numerics;
using System.Windows.Forms;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Objects;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Nodes.Misc;

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

        [DisplayName( "Bounding sphere" )]
        public BoundingSphere BoundingSphere
        {
            get => GetProperty<BoundingSphere>();
            set => SetProperty( value );
        }

        public Vector3[] Vertices
        {
            get => GetProperty<Vector3[]>();
            set => SetProperty( value );
        }

        public Vector3[] Normals
        {
            get => GetProperty<Vector3[]>();
            set => SetProperty( value );
        }

        public Vector4[] Tangents
        {
            get => GetProperty<Vector4[]>();
            set => SetProperty( value );
        }

        [DisplayName( "UV channel 1" )]
        public Vector2[] UVChannel1
        {
            get => GetProperty<Vector2[]>();
            set => SetProperty( value );
        }

        [DisplayName( "UV channel 2" )]
        public Vector2[] UVChannel2
        {
            get => GetProperty<Vector2[]>();
            set => SetProperty( value );
        }

        public Color[] Colors
        {
            get => GetProperty<Color[]>();
            set => SetProperty( value );
        }

        [DisplayName( "Bone weights" )]
        public BoneWeight[] BoneWeights
        {
            get => GetProperty<BoneWeight[]>();
            set => SetProperty( value );
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