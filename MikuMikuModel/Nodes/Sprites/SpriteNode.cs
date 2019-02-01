using System.ComponentModel;
using MikuMikuLibrary.Sprites;
using MikuMikuModel.Nodes.TypeConverters;

namespace MikuMikuModel.Nodes.Sprites
{
    public class SpriteNode : Node<Sprite>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

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

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public SpriteNode( string name, Sprite data ) : base( name, data )
        {
        }
    }
}