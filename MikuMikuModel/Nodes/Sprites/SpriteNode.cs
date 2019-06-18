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
            set
            {
                SetProperty( value );
                CalculateNdcValues();
            }
        }

        public float Y
        {
            get => GetProperty<float>();
            set
            {
                SetProperty( value );
                CalculateNdcValues();
            }
        }

        public float Width
        {
            get => GetProperty<float>();
            set
            {
                SetProperty( value );
                CalculateNdcValues();
            }
        }

        public float Height
        {
            get => GetProperty<float>();
            set
            {
                SetProperty( value );
                CalculateNdcValues();
            }
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

        private void CalculateNdcValues()
        {
            var spriteSetNode = FindParent<SpriteSetNode>();
            var spriteSet = spriteSetNode.Data;

            if ( Data.TextureIndex >= spriteSet.TextureSet.Textures.Count )
                return;

            var texture = spriteSet.TextureSet.Textures[ Data.TextureIndex ];

            Data.NdcX = Data.X / texture.Width;
            Data.NdcY = Data.Y / texture.Height;
            Data.NdcWidth = Data.Width / texture.Width;
            Data.NdcHeight = Data.Height / texture.Height;
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