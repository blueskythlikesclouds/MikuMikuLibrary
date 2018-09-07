using MikuMikuLibrary.IO;
using MikuMikuLibrary.Sprites;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace MikuMikuModel.DataNodes
{
    [DataNodeSpecialName( "Sprite Set" )]
    public class SpriteSetNode : BinaryFileNode<SpriteSet>
    {
        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Branch; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get
            {
                return
                  DataNodeActionFlags.Import | DataNodeActionFlags.Export | DataNodeActionFlags.Move |
                  DataNodeActionFlags.Remove | DataNodeActionFlags.Rename | DataNodeActionFlags.Replace;
            }
        }

        [Browsable( false )]
        public ListNode<Sprite> Sprites { get; set; }

        [Browsable( false )]
        public TextureSetNode Textures { get; set; }

        protected override void InitializeCore()
        {
            RegisterReplaceHandler<SpriteSet>( BinaryFile.Load<SpriteSet> );
            RegisterExportHandler<SpriteSet>( ( path ) => Data.Save( path ) );
            RegisterCustomHandler( "Export All", () =>
            {
                using ( var saveFileDialog = new SaveFileDialog() )
                {
                    saveFileDialog.AutoUpgradeEnabled = true;
                    saveFileDialog.CheckPathExists = true;
                    saveFileDialog.Title = "Select a folder to export textures to.";
                    saveFileDialog.FileName = "Enter into a directory and press Save";

                    if ( saveFileDialog.ShowDialog() == DialogResult.OK )
                    {
                        var outputDirectory = Path.GetDirectoryName( saveFileDialog.FileName );
                        foreach ( var sprite in SpriteCropper.Crop( Data ) )
                            sprite.Value.Save( Path.Combine( outputDirectory, sprite.Key.Name + ".png" ) );
                    }
                }
            }, Keys.Control | Keys.Shift | Keys.E );
            RegisterDataUpdateHandler( () =>
            {
                var data = new SpriteSet();
                data.Format = Format;
                data.Endianness = Endianness;
                data.Sprites.AddRange( Sprites.Data );
                data.TextureSet.Textures.AddRange( Textures.Data.Textures );
                return data;
            } );
        }

        protected override void InitializeViewCore()
        {
            Add( Sprites = new ListNode<Sprite>( nameof( Data.Sprites ), Data.Sprites ) );
            Add( Textures = new TextureSetNode( "Texture Set", Data.TextureSet ) );
        }

        public SpriteSetNode( string name, SpriteSet data ) : base( name, data )
        {
        }
    }
}
