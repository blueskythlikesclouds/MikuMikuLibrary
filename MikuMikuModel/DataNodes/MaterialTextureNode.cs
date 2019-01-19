using MikuMikuLibrary.Materials;
using MikuMikuModel.DataNodes.TypeConverters;
using MikuMikuModel.Resources;
using System.ComponentModel;
using System.Drawing;

namespace MikuMikuModel.DataNodes
{
    [DataNodePrettyName( "Material Texture" )]
    public class MaterialTextureNode : DataNode<MaterialTexture>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Leaf;
        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        public override Bitmap Icon => ResourceStore.LoadBitmap( "Icons/MaterialTexture.png" );

        [TypeConverter( typeof( Int32HexTypeConverter ) )]
        public int Field00
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }
        [TypeConverter( typeof( Int32HexTypeConverter ) )]
        public int Field01
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }
        [DisplayName( "Texture ID" )]
        public int TextureID
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }
        [TypeConverter( typeof( Int32HexTypeConverter ) )]
        public int Field02
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }
        public float Field03
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field04
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field05
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field06
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field07
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field08
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field09
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field10
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field11
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field12
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field13
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field14
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field15
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field16
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field17
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field18
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field19
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field20
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field21
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field22
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field23
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field24
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field25
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field26
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field27
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field28
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

        public MaterialTextureNode( string name, MaterialTexture data ) : base( name, data )
        {
        }
    }
}
