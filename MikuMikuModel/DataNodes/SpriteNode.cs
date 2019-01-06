using MikuMikuLibrary.Sprites;
using System.ComponentModel;

namespace MikuMikuModel.DataNodes
{
    public class SpriteNode : DataNode<Sprite>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Leaf;

        public override DataNodeActionFlags ActionFlags =>
            DataNodeActionFlags.Move | DataNodeActionFlags.Remove | DataNodeActionFlags.Rename;

        public int Field00
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }
        public int Field01
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }
        [DisplayName( "Texture index" )]
        public int TextureIndex
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }
        public float Field02
        {
            get => GetProperty<float>();
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
        public float X
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Y
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Width
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }
        public float Height
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

        protected override void OnRename( string oldName )
        {
            SetProperty( Name, nameof( Data.Name ) );
            base.OnRename( oldName );
        }

        public SpriteNode( string name, Sprite data ) :
            base( string.IsNullOrEmpty( data.Name ) ? name : data.Name, data )
        {
        }
    }
}
