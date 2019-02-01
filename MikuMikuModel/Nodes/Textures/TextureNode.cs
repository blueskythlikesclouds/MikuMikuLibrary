using System.Drawing;
using System.Windows.Forms;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Sprites;
using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.DDS;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Nodes.Sprites;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.Textures
{
    public class TextureNode : Node<Texture>
    {
        public override NodeFlags Flags => NodeFlags.Rename | NodeFlags.Export | NodeFlags.Replace;
        public override Bitmap Image => ResourceStore.LoadBitmap( "Icons/Texture.png" );

        public override Control Control
        {
            get
            {
                TextureViewControl.Instance.SetTexture( Data );
                return TextureViewControl.Instance;
            }
        }

        public int ID
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        public int Width => GetProperty<int>();
        public int Height => GetProperty<int>();
        public TextureFormat Format => GetProperty<TextureFormat>();

        protected override void Initialize()
        {
            RegisterExportHandler<Bitmap>( filePath => TextureDecoder.DecodeToPNG( Data, filePath ) );
            RegisterExportHandler<Texture>( filePath => TextureDecoder.DecodeToDDS( Data, filePath ) );
            RegisterReplaceHandler<Texture>( TextureEncoder.Encode );
            RegisterReplaceHandler<Bitmap>( filePath =>
            {
                // TODO: REMOVE THIS HACK
                var parent = FindParent<SpriteSetNode>();
                if ( parent != null && BinaryFormatUtilities.IsModern( parent.Format ) )
                    return TextureEncoder.Encode( filePath );

                using ( var bitmap = new Bitmap( filePath ) )
                {
                    if ( Data.IsYCbCr )
                    {
                        var format = TextureFormat.RGB;
                        if ( DDSCodec.HasTransparency( bitmap ) )
                            format = TextureFormat.RGBA;

                        return TextureEncoder.Encode( bitmap, format, false );
                    }

                    return TextureEncoder.Encode( bitmap, Data.Format, Data.MipMapCount != 0 );
                }
            } );
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public TextureNode( string name, Texture data ) : base( name, data )
        {
        }
    }
}