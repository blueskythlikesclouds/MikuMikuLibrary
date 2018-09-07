using MikuMikuLibrary.Materials;
using System.ComponentModel;
using System.Drawing;

namespace MikuMikuModel.DataNodes
{
    [DataNodeSpecialName( "Material Texture" )]
    public class MaterialTextureNode : DataNode<MaterialTexture>
    {
        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Leaf; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get { return DataNodeActionFlags.None; }
        }

        public override Bitmap Icon
        {
            get { return Properties.Resources.MaterialTexture; }
        }

        public int Field00
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }
        public int Field01
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }
        [DisplayName( "Texture ID" )]
        public int TextureID
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }
        public int Field02
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }
        public float Field03
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field04
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field05
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field06
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field07
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field08
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field09
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field10
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field11
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field12
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field13
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field14
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field15
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field16
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field17
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field18
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field19
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field20
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field21
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field22
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field23
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field24
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field25
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field26
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field27
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field28
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

        public MaterialTextureNode( string name, MaterialTexture data ) : base( name, data )
        {
        }
    }
}
