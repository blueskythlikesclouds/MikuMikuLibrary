using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.DDS;
using MikuMikuModel.GUI.Controls;
using System.Drawing;
using System.Windows.Forms;

namespace MikuMikuModel.DataNodes
{
    public class TextureNode : DataNode<Texture>
    {
        public static TextureSet GlobalTextureSet { get; } = new TextureSet();

        public override DataNodeFlags Flags => DataNodeFlags.Leaf;

        public override DataNodeActionFlags ActionFlags => 
            DataNodeActionFlags.Export | DataNodeActionFlags.Move | DataNodeActionFlags.Remove | DataNodeActionFlags.Replace;

        public override Bitmap Icon => Properties.Resources.Texture;

        public int Width => GetProperty<int>();
        public int Height => GetProperty<int>();
        public TextureFormat Format => GetProperty<TextureFormat>();

        public override Control Control
        {
            get
            {
                TextureViewControl.Instance.SetTexture( Data );
                return TextureViewControl.Instance;
            }
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Texture>( ( path ) => TextureDecoder.DecodeToDDS( Data, path ) );
            RegisterExportHandler<Bitmap>( ( path ) => TextureDecoder.Decode( Data ).Save( path ) );
            RegisterReplaceHandler<Texture>( TextureEncoder.Encode );
            RegisterReplaceHandler<Bitmap>( ( path ) =>
            {
                using ( var bitmap = new Bitmap( path ) )
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

        protected override void InitializeViewCore()
        {
        }

        protected override void OnRename( string oldName )
        {
            SetProperty( Name, nameof( Data.Name ) );
            base.OnRename( oldName );
        }

        protected override void OnReplace( object oldData )
        {
            var oldDataT = ( Texture )oldData;
            if ( string.IsNullOrEmpty( Data.Name ) )
                Rename( oldDataT.Name );
            Data.ID = oldDataT.ID;

            GlobalTextureSet.Textures.Remove( oldDataT );
            GlobalTextureSet.Textures.Add( Data );

            base.OnReplace( oldData );
        }

        public TextureNode( string name, Texture data ) :
            base( string.IsNullOrEmpty( data.Name ) ? name : data.Name, data )
        {
        }
    }
}
