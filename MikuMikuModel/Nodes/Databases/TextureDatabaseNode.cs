using System;
using System.ComponentModel;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.TypeConverters;

namespace MikuMikuModel.Nodes.Databases
{
    public class TextureDatabaseNode : BinaryFileNode<TextureDatabase>
    {
        public override NodeFlags Flags =>
            NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            AddExportHandler<TextureDatabase>( filePath => Data.Save( filePath ) );
            AddReplaceHandler<TextureDatabase>( BinaryFile.Load<TextureDatabase> );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<TextureInfo>( "Textures", Data.Textures, x => x.Name ) );
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

    public class TextureInfoNode : Node<TextureInfo>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [TypeConverter( typeof( IdTypeConverter ) )]
        public uint Id
        {
            get => GetProperty<uint>();
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

        public TextureInfoNode( string name, TextureInfo data ) : base( name, data )
        {
        }
    }
}