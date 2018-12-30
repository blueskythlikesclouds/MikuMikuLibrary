using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Models;
using MikuMikuModel.GUI.Controls;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Forms;

namespace MikuMikuModel.DataNodes
{
    [DataNodeSpecialName( "Submesh" )]
    public class SubMeshNode : DataNode<SubMesh>
    {
        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Branch; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get { return DataNodeActionFlags.Move | DataNodeActionFlags.Remove | DataNodeActionFlags.Rename; }
        }

        public override Control Control
        {
            get
            {
                ModelViewControl.Instance.SetModel( Data, FindParent<Mesh>().Data, TextureNode.GlobalTextureSet );
                return ModelViewControl.Instance;
            }
        }

        [DisplayName( "Bounding sphere" )]
        public BoundingSphere BoundingSphere
        {
            get { return GetProperty<BoundingSphere>(); }
            set { SetProperty( value ); }
        }

        public Vector3[] Vertices
        {
            get { return GetProperty<Vector3[]>(); }
            set { SetProperty( value ); }
        }

        public Vector3[] Normals
        {
            get { return GetProperty<Vector3[]>(); }
            set { SetProperty( value ); }
        }

        public Vector4[] Tangents
        {
            get { return GetProperty<Vector4[]>(); }
            set { SetProperty( value ); }
        }

        [DisplayName( "UV channel 1" )]
        public Vector2[] UVChannel1
        {
            get { return GetProperty<Vector2[]>(); }
            set { SetProperty( value ); }
        }

        [DisplayName( "UV channel 2" )]
        public Vector2[] UVChannel2
        {
            get { return GetProperty<Vector2[]>(); }
            set { SetProperty( value ); }
        }

        public Color[] Colors
        {
            get { return GetProperty<Color[]>(); }
            set { SetProperty( value ); }
        }

        [DisplayName( "Bone weights" )]
        public BoneWeight[] BoneWeights
        {
            get { return GetProperty<BoneWeight[]>(); }
            set { SetProperty( value ); }
        }

        [Browsable( false )]
        public ListNode<IndexTable> IndexTables { get; set; }

        protected override void InitializeCore()
        {
            RegisterDataUpdateHandler( () =>
            {
                var data = new SubMesh();
                data.BoundingSphere = BoundingSphere;
                data.IndexTables.AddRange( IndexTables.Data );
                data.Vertices = Vertices;
                data.Normals = Normals;
                data.Tangents = Tangents;
                data.UVChannel1 = UVChannel1;
                data.UVChannel2 = UVChannel2;
                data.Colors = Colors;
                data.BoneWeights = BoneWeights;
                data.Name = Name;
                return data;
            } );
        }

        protected override void InitializeViewCore()
        {
            Add( IndexTables = new ListNode<IndexTable>( "Index tables", Data.IndexTables ) );
        }

        protected override void OnRename( string oldName )
        {
            SetProperty( Name, nameof( Name ) );
            base.OnRename( oldName );
        }

        public SubMeshNode( string name, SubMesh data ) :
            base( string.IsNullOrEmpty( data.Name ) ? name : data.Name, data )
        {
        }
    }
}
