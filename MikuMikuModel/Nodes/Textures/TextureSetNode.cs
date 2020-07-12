using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Hashes;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.Processing;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.Databases;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Sprites;
using MikuMikuModel.Resources;
using Ookii.Dialogs.WinForms;

namespace MikuMikuModel.Nodes.Textures
{
    public class TextureSetNode : BinaryFileNode<TextureSet>
    {
        private TextureDatabaseNode mTextureDatabaseNode;

        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Import | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        public override Bitmap Image =>
            ResourceStore.LoadBitmap( "Icons/TextureSet.png" );

        protected override void Initialize()
        {
            AddImportHandler<Texture>( filePath =>
            {
                var texture = TextureEncoder.EncodeFromFile( filePath, TextureFormat.Unknown, Parent.FindParent<SpriteSetNode>() == null );
                {
                    texture.Name = Path.GetFileNameWithoutExtension( filePath );
                    texture.Id = MurmurHash.Calculate( texture.Name );
                }

                Data.Textures.Add( texture );
            } );
            AddExportHandler<TextureSet>( filePath => Data.Save( filePath ) );
            AddReplaceHandler<TextureSet>( BinaryFile.Load<TextureSet> );
            AddCustomHandler( "Export All", () =>
            {
                using ( var folderBrowseDialog = new VistaFolderBrowserDialog() )
                {
                    folderBrowseDialog.Description = "Select a folder to save textures to.";
                    folderBrowseDialog.UseDescriptionForTitle = true;

                    if ( folderBrowseDialog.ShowDialog() != DialogResult.OK )
                        return;

                    foreach ( var texture in Data.Textures )
                    {
                        string extension = !TextureFormatUtilities.IsBlockCompressed( texture.Format ) || texture.IsYCbCr ? "png" : "dds";
                        TextureDecoder.DecodeToFile( texture, Path.Combine( folderBrowseDialog.SelectedPath, $"{texture.Name}.{extension}" ) );
                    }
                }
            }, Keys.Control | Keys.Shift | Keys.E );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            if ( Parent != null && Name.EndsWith( ".txd", StringComparison.OrdinalIgnoreCase ) )
            {
                var textureDatabaseNode = Parent.FindNode<TextureDatabaseNode>( Path.ChangeExtension( Name, "txi" ) );

                if ( textureDatabaseNode != null && Data.Textures.Count == textureDatabaseNode.Data.Textures.Count )
                {
                    for ( int i = 0; i < Data.Textures.Count; i++ )
                    {
                        var texture = Data.Textures[ i ];
                        var textureInfo = textureDatabaseNode.Data.Textures[ i ];

                        texture.Name = textureInfo.Name;
                        texture.Id = textureInfo.Id;
                    }

                    mTextureDatabaseNode = textureDatabaseNode;
                }
            }

            Nodes.Add( new ListNode<Texture>( "Textures", Data.Textures, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
            if ( mTextureDatabaseNode == null )
                return;

            var textureDatabase = new TextureDatabase();

            foreach ( var texture in Data.Textures )
            {
                textureDatabase.Textures.Add( new TextureInfo
                {
                    Id = texture.Id,
                    Name = texture.Name
                } );
            }

            mTextureDatabaseNode.Replace( textureDatabase );
        }

        public TextureSetNode( string name, TextureSet data ) : base( name, data )
        {
        }

        public TextureSetNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}