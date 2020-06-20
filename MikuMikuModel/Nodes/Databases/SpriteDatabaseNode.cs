using System;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuModel.Nodes.IO;

namespace MikuMikuModel.Nodes.Databases
{
    public class SpriteDatabaseNode : BinaryFileNode<SpriteDatabase>
    {
        public override NodeFlags Flags =>
            NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            AddExportHandler<SpriteDatabase>( filePath => Data.Save( filePath ) );
            AddReplaceHandler<SpriteDatabase>( BinaryFile.Load<SpriteDatabase> );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public SpriteDatabaseNode( string name, SpriteDatabase data ) : base( name, data )
        {
        }

        public SpriteDatabaseNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}