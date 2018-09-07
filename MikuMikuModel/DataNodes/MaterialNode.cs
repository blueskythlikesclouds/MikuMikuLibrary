using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Misc;

namespace MikuMikuModel.DataNodes
{
    public class MaterialNode : DataNode<Material>
    {
        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Branch; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get { return DataNodeActionFlags.Move | DataNodeActionFlags.Remove | DataNodeActionFlags.Rename; }
        }

        public override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.Material; }
        }

        public int Field00
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }
        public string Shader
        {
            get { return GetProperty<string>(); }
            set { SetProperty( value ); }
        }
        public int Field01
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }
        public int Field02
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }
        public Color DiffuseColor
        {
            get { return GetProperty<Color>(); }
            set { SetProperty( value ); }
        }
        public Color AmbientColor
        {
            get { return GetProperty<Color>(); }
            set { SetProperty( value ); }
        }
        public Color SpecularColor
        {
            get { return GetProperty<Color>(); }
            set { SetProperty( value ); }
        }
        public Color EmissionColor
        {
            get { return GetProperty<Color>(); }
            set { SetProperty( value ); }
        }
        public float Shininess
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
        public float Field29
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field30
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field31
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field32
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field33
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field34
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field35
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field36
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field37
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field38
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field39
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field40
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
            Add( new MaterialTextureNode( nameof( Data.Diffuse ), Data.Diffuse ) );
            Add( new MaterialTextureNode( nameof( Data.Ambient ), Data.Ambient ) );
            Add( new MaterialTextureNode( nameof( Data.Normal ), Data.Normal ) );
            Add( new MaterialTextureNode( nameof( Data.Specular ), Data.Specular ) );
            Add( new MaterialTextureNode( nameof( Data.ToonCurve ), Data.ToonCurve ) );
            Add( new MaterialTextureNode( nameof( Data.Reflection ), Data.Reflection ) );
            Add( new MaterialTextureNode( nameof( Data.SpecularPower ), Data.SpecularPower ) );
            Add( new MaterialTextureNode( nameof( Data.Texture08 ), Data.Texture08 ) );
        }

        protected override void OnRename( string oldName )
        {
            SetProperty( Name, nameof( Name ) );
            base.OnRename( oldName );
        }

        public MaterialNode( string name, Material data ) :
            base( string.IsNullOrEmpty( data.Name ) ? name : data.Name, data )
        {
        }
    }
}
