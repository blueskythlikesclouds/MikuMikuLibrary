using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Models;
using MikuMikuModel.GUI.Controls;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MikuMikuModel.DataNodes
{
    public class MeshNode : DataNode<Mesh>
    {
        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Branch; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get
            {
                return
                  DataNodeActionFlags.Export | DataNodeActionFlags.Move |
                  DataNodeActionFlags.Remove | DataNodeActionFlags.Rename | DataNodeActionFlags.Replace;
            }
        }

        public override Control Control
        {
            get
            {
                ModelViewControl.Instance.SetModel( Data, TextureNode.GlobalTextureSet );
                return ModelViewControl.Instance;
            }
        }

        public override Bitmap Icon
        {
            get { return Properties.Resources.Mesh; }
        }

        public int ID
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }

        [DisplayName( "Bounding Sphere" )]
        public BoundingSphere BoundingSphere
        {
            get { return GetProperty<BoundingSphere>(); }
            set { SetProperty( value ); }
        }

        [Browsable( false )]
        public ListNode<SubMesh> SubMeshes { get; set; }

        [Browsable( false )]
        public ListNode<Material> Materials { get; set; }

        [Browsable( false )]
        public MeshSkinNode Skin { get; set; }

        protected override void InitializeCore()
        {
            RegisterDataUpdateHandler( () =>
            {
                var data = new Mesh();
                data.SubMeshes.AddRange( SubMeshes.Data );
                data.Materials.AddRange( Materials.Data );
                data.Skin = Skin?.Data;
                data.Name = Name;
                data.ID = ID;
                data.BoundingSphere = BoundingSphere;
                return data;
            } );
        }

        protected override void InitializeViewCore()
        {
            Add( SubMeshes = new ListNode<SubMesh>( "Submeshes", Data.SubMeshes ) );
            Add( Materials = new ListNode<Material>( "Materials", Data.Materials ) );

            if ( Data.Skin != null )
                Add( Skin = new MeshSkinNode( nameof( Data.Skin ), Data.Skin ) );
        }

        protected override void OnRename( string oldName )
        {
            SetProperty( Name, nameof( Name ) );
            base.OnRename( oldName );
        }

        public MeshNode( string name, Mesh data ) :
            base( string.IsNullOrEmpty( data.Name ) ? name : data.Name, data )
        {
        }
    }
}
