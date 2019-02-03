using System.Drawing;
using System.Windows.Forms;
using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.DDS;
using MikuMikuModel.GUI.Controls;
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

        public int Id
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

        protected override void OnReplace( Texture previousData )
        {
            Data.Name = previousData.Name;
            Data.Id = previousData.Id;

            base.OnReplace( previousData );
        }

        public TextureNode( string name, Texture data ) : base( name, data )
        {
        }
    }
}