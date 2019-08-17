using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Objects;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Nodes.Misc;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.Models
{
    public class ObjectNode : Node<Object>
    {
        public override NodeFlags Flags =>
            NodeFlags.Import | NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        public override Bitmap Image =>
            ResourceStore.LoadBitmap( "Icons/Object.png" );

        public override Control Control
        {
            get
            {
                var objectSetParent = FindParent<ObjectSetNode>();
                if ( objectSetParent == null )
                    return null;

                ModelViewControl.Instance.SetModel( Data, objectSetParent.Data.TextureSet );
                return ModelViewControl.Instance;
            }
        }

        public int Id
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
            Nodes.Add( new ListNode<Mesh>( "Meshes", Data.Meshes, x => x.Name ) );
            Nodes.Add( new ListNode<Material>( "Materials", Data.Materials, x => x.Name ) );

            if ( Data.Skin != null )
                Nodes.Add( new SkinNode( "Skin", Data.Skin ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public ObjectNode( string name, Object data ) : base( name, data )
        {
        }
    }
}