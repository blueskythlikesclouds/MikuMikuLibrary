using System;
using System.IO;
using System.Windows.Forms;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Sprites;
using MikuMikuModel.Nodes.Databases;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Misc;
using MikuMikuModel.Nodes.Textures;
using Ookii.Dialogs.WinForms;

namespace MikuMikuModel.Nodes.Sprites
{
    public class SpriteSetNode : BinaryFileNode<SpriteSet>
    {
        private TextureSetNode mTextureSetNode;

        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Import | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            RegisterReplaceHandler<SpriteSet>( BinaryFile.Load<SpriteSet> );
            RegisterExportHandler<SpriteSet>( filePath => Data.Save( filePath ) );
            RegisterCustomHandler( "Export All", () =>
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
                    var spriteSetEntry = spriteDatabaseNode.Data.SpriteSets[ 0 ];
                    foreach ( var spriteEntry in spriteSetEntry.Sprites )
                        Data.Sprites[ spriteEntry.Index ].Name = spriteEntry.Name;

                    foreach ( var textureEntry in spriteSetEntry.Textures )
                        Data.TextureSet.Textures[ textureEntry.Index ].Name = textureEntry.Name;
                }
            }
            Nodes.Add( new ListNode<Sprite>( "Sprites", Data.Sprites, x => x.Name ) );
            Nodes.Add( mTextureSetNode = new TextureSetNode( "Texture Set", Data.TextureSet ) );
        }

        protected override void SynchronizeCore()
        {
            if ( Data.TextureSet == mTextureSetNode.Data )
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