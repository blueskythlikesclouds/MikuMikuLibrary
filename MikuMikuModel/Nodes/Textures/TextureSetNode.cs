using System;
using System.Drawing;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Textures;
using MikuMikuModel.Nodes.Databases;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Misc;
using MikuMikuModel.Resources;

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
            RegisterExportHandler<TextureSet>( filePath => Data.Save( filePath ) );
            RegisterReplaceHandler<TextureSet>( BinaryFile.Load<TextureSet> );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            if ( Parent != null && Name.EndsWith( ".txd", StringComparison.OrdinalIgnoreCase ) )
            {
                var textureDatabaseNode = 
                    Parent.FindNode<TextureDatabaseNode>( Path.ChangeExtension( Name, "txi" ) );

                if ( textureDatabaseNode != null && Data.Textures.Count == textureDatabaseNode.Data.Textures.Count )
                {
                    for ( int i = 0; i < Data.Textures.Count; i++ )
                    {
                        var texture = Data.Textures[ i ];
                        var textureEntry = textureDatabaseNode.Data.Textures[ i ];

                        texture.Name = textureEntry.Name;
                        texture.Id = textureEntry.Id;
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
                textureDatabase.Textures.Add( new TextureEntry
                {
                    Id = texture.Id,
                    Name = texture.Name,
                } );

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