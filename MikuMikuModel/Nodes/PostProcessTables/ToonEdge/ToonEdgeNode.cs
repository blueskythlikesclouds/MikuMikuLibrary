// Code by Thatrandomlurker
using MikuMikuModel.Nodes.Collections;
using System;
using System.IO;
using MikuMikuLibrary.PostProcessTables.ToonEdgeTable;
using MikuMikuLibrary.IO;
using MikuMikuModel.Nodes.IO;

namespace MikuMikuModel.Nodes.PostProcessTables.ToonEdge
{
    class ToonEdgeTableNode : BinaryFileNode<ToonEdgeTable>
    {
        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            AddReplaceHandler<ToonEdgeTable>( BinaryFile.Load<ToonEdgeTable> );
            AddExportHandler<ToonEdgeTable>( filePath => Data.Save( filePath ) );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<ToonSetting>( "Entries", Data.ToonEntries, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public ToonEdgeTableNode( string name, ToonEdgeTable data ) : base( name, data )
        {
        }

        public ToonEdgeTableNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}
