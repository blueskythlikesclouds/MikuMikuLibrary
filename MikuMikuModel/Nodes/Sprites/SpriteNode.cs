using System.ComponentModel;
using MikuMikuLibrary.Sprites;

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

        [DisplayName( "Resolution mode" )]
        public ResolutionMode ResolutionMode
        {
            get => GetProperty<ResolutionMode>();
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