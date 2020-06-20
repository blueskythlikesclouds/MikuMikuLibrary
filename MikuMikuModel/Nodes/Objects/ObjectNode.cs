using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Objects;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.TypeConverters;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.Objects
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

        [Category( "General" )]
        [TypeConverter( typeof( IdTypeConverter ) )]
        public uint Id
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Bounding sphere" )]
        public BoundingSphere BoundingSphere
        {
            get => GetProperty<BoundingSphere>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Flags" )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint ObjectFlags
        {
            get => GetProperty<uint>( nameof( Object.Flags ) );
            set => SetProperty( value, nameof( Object.Flags ) );
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