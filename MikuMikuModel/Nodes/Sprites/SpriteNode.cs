using System.ComponentModel;
using MikuMikuLibrary.Sprites;
using MikuMikuModel.Nodes.TypeConverters;

namespace MikuMikuModel.Nodes.Sprites
{
    public class SpriteNode : Node<Sprite>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [DisplayName( "Texture index" )]
        public int TextureIndex
        {
            get => GetProperty<int>();
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

        public float Field02
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [DisplayName( "Normalized X" )]
        public float NdcX
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [DisplayName( "Normalized Y" )]
        public float NdcY
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [DisplayName( "Normalized width" )]
        public float NdcWidth
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [DisplayName( "Normalized height" )]
        public float NdcHeight
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