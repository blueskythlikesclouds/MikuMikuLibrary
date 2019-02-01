using System.ComponentModel;
using System.Drawing;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Maths;
using MikuMikuLibrary.Models;
using MikuMikuModel.Nodes.Misc;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.Models
{
    public class MeshNode : Node<Mesh>
    {
        public override NodeFlags Flags =>
            NodeFlags.Import | NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        public override Bitmap Image => 
            ResourceStore.LoadBitmap( "Icons/Mesh.png" );

        public int ID
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        [DisplayName( "Bounding sphere" )]
        public BoundingSphere BoundingSphere
        {
            get => GetProperty<BoundingSphere>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<SubMesh>( "Submeshes", Data.SubMeshes, x => x.Name ) );
            Nodes.Add( new ListNode<Material>( "Materials", Data.Materials, x => x.Name ) );

            if ( Data.Skin != null )
                Nodes.Add( new SkinNode( "Skin", Data.Skin ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public MeshNode( string name, Mesh data ) : base( name, data )
        {
        }
    }   
}