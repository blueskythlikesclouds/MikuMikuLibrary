using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Hashes;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Sprites;
using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.Processing;
using MikuMikuModel.Modules;
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
                string filePath = ModuleExportUtilities.SelectModuleExport<Texture>(
                    "Select a folder to export textures to.", "Enter into a directory and press Save" );

                if ( string.IsNullOrEmpty( filePath ) )
                    return;

                string directoryPath = Path.GetDirectoryName( filePath );
                string extension = Path.GetExtension( filePath ).Trim( '.' );

                foreach ( var texture in Data.Textures )
                    TextureDecoder.DecodeToFile( texture, Path.Combine( directoryPath, $"{texture.Name}.{extension}" ) );

            }, Keys.Control | Keys.Shift | Keys.E );           
            
            AddCustomHandler( "Export All (Flipped)", () =>
            {
                string filePath = ModuleExportUtilities.SelectModuleExport<Bitmap>(
                    "Select a folder to export textures to.", "Enter into a directory and press Save" );

                if ( string.IsNullOrEmpty( filePath ) )
                    return;

                string directoryPath = Path.GetDirectoryName( filePath );
                string extension = Path.GetExtension( filePath ).Trim( '.' );

                foreach ( var texture in Data.Textures )
                {
                    using ( var bitmap = TextureDecoder.DecodeToBitmap( texture ) )
                    {
                        bitmap.RotateFlip( RotateFlipType.Rotate180FlipX );
                        bitmap.Save( Path.Combine( directoryPath, $"{texture.Name}.{extension}" ) );
                    }
                }
            } );

            AddCustomHandlerSeparator();

            AddDirtyCustomHandler( "Replace All", () =>
            {
                var fileNames = ModuleImportUtilities.SelectModuleImportMultiselect<Texture>();

                if ( fileNames == null )
                    return false;

                bool any = false;

                foreach ( string fileName in fileNames )
                {
                    string textureName = Path.GetFileNameWithoutExtension( fileName );

                    int textureIndex = Data.Textures.FindIndex( x => x.Name.Equals( textureName, StringComparison.OrdinalIgnoreCase ) );

                    if ( textureIndex == -1 )
                        continue;

                    any = true;

                    var texture = Data.Textures[ textureIndex ];

                    var newTexture = TextureEncoder.EncodeFromFile( fileName,
                        texture.IsYCbCr ? TextureFormat.RGBA8 : texture.Format, texture.MipMapCount != 1 );

                    newTexture.Name = texture.Name;
                    newTexture.Id = texture.Id;

                    Data.Textures[ textureIndex ] = newTexture;
                }

                return any;
            }, Keys.Control | Keys.Shift | Keys.R, CustomHandlerFlags.Repopulate | CustomHandlerFlags.ClearMementos );

            AddDirtyCustomHandler( "Replace All (Flipped)", () =>
            {
                var fileNames = ModuleImportUtilities.SelectModuleImportMultiselect<Bitmap>();

                if ( fileNames == null )
                    return false;

                bool any = false;

                foreach ( string fileName in fileNames )
                {
                    // Boy do I love duplicate code C:

                    string textureName = Path.GetFileNameWithoutExtension( fileName );

                    int textureIndex = Data.Textures.FindIndex( x => x.Name.Equals( textureName, StringComparison.OrdinalIgnoreCase ) );

                    if ( textureIndex == -1 )
                        continue;

                    any = true;

                    var texture = Data.Textures[ textureIndex ];

                    Texture newTexture;

                    using ( var bitmap = new Bitmap( fileName ) )
                    {
                        bitmap.RotateFlip( RotateFlipType.Rotate180FlipX );

                        newTexture = TextureEncoder.EncodeFromBitmap( bitmap,
                            texture.IsYCbCr ? TextureFormat.RGBA8 : texture.Format, texture.MipMapCount != 1 );
                    }

                    newTexture.Name = texture.Name;
                    newTexture.Id = texture.Id;

                    Data.Textures[ textureIndex ] = newTexture;
                }

                return any;
            }, Keys.None, CustomHandlerFlags.Repopulate | CustomHandlerFlags.ClearMementos );

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