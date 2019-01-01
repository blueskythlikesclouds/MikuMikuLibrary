using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Misc;
using System.ComponentModel;

namespace MikuMikuModel.DataNodes
{
    public class MaterialNode : DataNode<Material>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags => 
            DataNodeActionFlags.Move | DataNodeActionFlags.Remove | DataNodeActionFlags.Rename;

        public override System.Drawing.Bitmap Icon => Properties.Resources.Material;

        public int Field00
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }
        public string Shader
        {
            get => GetProperty<string>();
            set => SetProperty( value );
        }
        public int Field01
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }
        public int Field02
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }
        [DisplayName( "Diffuse color" )]
        public Color DiffuseColor
        {
            get => GetProperty<Color>();
            set => SetProperty( value );
        }
        [DisplayName( "Ambient color" )]
        public Color AmbientColor
        {
            get => GetProperty<Color>();
            set => SetProperty( value );
        }
        [DisplayName( "Specular color" )]
        public Color SpecularColor
        {
            get => GetProperty<Color>();
            set => SetProperty( value );
        }
        [DisplayName( "Emission color" )]
        public Color EmissionColor
        {
            get => GetProperty<Color>();
            set => SetProperty( value );
        }
        public float Shininess
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
        public float Field29
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field30
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field31
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field32
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field33
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field34
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field35
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field36
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field37
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field38
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field39
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Field40
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
            Add( new MaterialTextureNode( "Diffuse", Data.Diffuse ) );
            Add( new MaterialTextureNode( "Ambient", Data.Ambient ) );
            Add( new MaterialTextureNode( "Normal", Data.Normal ) );
            Add( new MaterialTextureNode( "Specular", Data.Specular ) );
            Add( new MaterialTextureNode( "Toon curve", Data.ToonCurve ) );
            Add( new MaterialTextureNode( "Reflection", Data.Reflection ) );
            Add( new MaterialTextureNode( "Specular power", Data.SpecularPower ) );
            Add( new MaterialTextureNode( "Unknown slot", Data.Texture08 ) );
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
