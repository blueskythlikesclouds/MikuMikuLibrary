using System;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Misc;

namespace MikuMikuModel.Nodes.Databases
{
    public class TextureDatabaseNode : BinaryFileNode<TextureDatabase>
    {
        public override NodeFlags Flags => 
            NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            RegisterExportHandler<TextureDatabase>( filePath => Data.Save( filePath ) );
            RegisterReplaceHandler<TextureDatabase>( BinaryFile.Load<TextureDatabase> );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<TextureEntry>( "Textures", Data.Textures, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public TextureDatabaseNode( string name, TextureDatabase data ) : base( name, data )
        {
        }

        public TextureDatabaseNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }

    public class TextureEntryNode : Node<TextureEntry>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        public int ID
        {
            get => GetProperty<int>();
            set => SetProperty( value );
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

        public TextureEntryNode( string name, TextureEntry data ) : base( name, data )
        {
        }
    }
}