using System;
using System.IO;
using System.Windows.Forms;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Sprites;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.Databases;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Textures;
using Ookii.Dialogs.WinForms;
using System.Globalization;
using MikuMikuModel.Mementos;

namespace MikuMikuModel.Nodes.Sprites
{
    public class SpriteSetNode : BinaryFileNode<SpriteSet>
    {
        private TextureSetNode mTextureSetNode;

        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Import | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            AddReplaceHandler<SpriteSet>( BinaryFile.Load<SpriteSet> );
            AddExportHandler<SpriteSet>( filePath => Data.Save( filePath ) );

            AddDirtyCustomHandler( "Add dummy sprite", () =>
            {
                Data.Sprites.Add( new Sprite { Name = "DUMMY", ResolutionMode = ResolutionMode.HDTV720 } );
                return true;
            }, Keys.None, CustomHandlerFlags.Repopulate );

            AddCustomHandlerSeparator();

            AddCustomHandler( "Export All", () =>
            {
                using ( var folderBrowseDialog = new VistaFolderBrowserDialog() )
                {
                    folderBrowseDialog.Description = "Select a folder to save sprites to.";
                    folderBrowseDialog.UseDescriptionForTitle = true;

                    if ( folderBrowseDialog.ShowDialog() != DialogResult.OK )
                        return;

                    foreach ( var pair in SpriteCropper.Crop( Data ) )
                        pair.Value.Save( Path.Combine( folderBrowseDialog.SelectedPath, $"{pair.Key.Name}.png" ) );
                }
            }, Keys.Control | Keys.Shift | Keys.E );

            AddCustomHandlerSeparator();

            AddDirtyCustomHandler( "Scale all sprites", () =>
            {
                string input = "2.0";

                while ( true )
                {
                    using ( var inputDialog = new InputDialog { WindowTitle = "Scale all sprites", Input = input } )
                    {
                        if ( inputDialog.ShowDialog() != DialogResult.OK )
                            break;

                        if ( float.TryParse( inputDialog.Input,
                            NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
                            out float factor ) )
                        {
                            foreach ( var sprite in Data.Sprites )
                            {
                                sprite.Width *= factor;
                                sprite.Height *= factor;
                                sprite.X *= factor;
                                sprite.Y *= factor;
                            }

                            return true;
                        }
                        else
                            MessageBox.Show( "Invalid factor.", Program.Name, MessageBoxButtons.OK,
                                MessageBoxIcon.Error );

                        input = inputDialog.Input;
                    }
                }

                return false;
            } );

            AddDirtyCustomHandler( "Set all resolution modes to...", () =>
            {
                string input = "HDTV720";

                while ( true )
                {
                    using ( var inputDialog = new InputDialog { WindowTitle = "Set all resolution modes", Input = input } )
                    {
                        if ( inputDialog.ShowDialog() != DialogResult.OK )
                            break;

                        if ( !Enum.TryParse( input = inputDialog.Input, true, out ResolutionMode mode ) )
                        {
                            MessageBox.Show( "Please enter a valid resolution mode.", Program.Name,
                                MessageBoxButtons.OK, MessageBoxIcon.Error );

                            continue;
                        }

                        foreach ( var sprite in Data.Sprites ) 
                            sprite.ResolutionMode = mode;

                        return true;
                    }
                }

                return false;
            } );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            if ( Parent != null && Name.EndsWith( ".spr", StringComparison.OrdinalIgnoreCase ) )
            {
                var spriteDatabaseNode =
                    Parent.FindNode<SpriteDatabaseNode>( Path.ChangeExtension( Name, "spi" ) );

                if ( spriteDatabaseNode != null )
                {
                    var spriteSetInfo = spriteDatabaseNode.Data.SpriteSets[ 0 ];

                    foreach ( var spriteInfo in spriteSetInfo.Sprites )
                        Data.Sprites[ spriteInfo.Index ].Name = spriteInfo.Name;

                    foreach ( var textureInfo in spriteSetInfo.Textures )
                        Data.TextureSet.Textures[ textureInfo.Index ].Name = textureInfo.Name;
                }
            }

            Nodes.Add( new ListNode<Sprite>( "Sprites", Data.Sprites, x => x.Name ) );
            Nodes.Add( mTextureSetNode = new TextureSetNode( "Texture Set", Data.TextureSet ) );
        }

        protected override void SynchronizeCore()
        {
            if ( mTextureSetNode == null || Data.TextureSet == mTextureSetNode.Data )
                return;

            Data.TextureSet.Textures.Clear();
            Data.TextureSet.Textures.AddRange( mTextureSetNode.Data.Textures );
        }

        public SpriteSetNode( string name, SpriteSet data ) : base( name, data )
        {
        }

        public SpriteSetNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}