// Code by Thatrandomlurker
using MikuMikuModel.Nodes.Collections;
using System;
using System.IO;
using MikuMikuLibrary.PostProcessTables.DepthOfFieldTable;
using MikuMikuLibrary.IO;
using MikuMikuModel.Nodes.IO;

namespace MikuMikuModel.Nodes.PostProcessTables.DepthOfField
{
    class DOFTableNode : BinaryFileNode<DOFTable>
    {
        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            AddReplaceHandler<DOFTable>( BinaryFile.Load<DOFTable> );
            AddExportHandler<DOFTable>( filePath => Data.Save( filePath ) );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<DOFSetting>( "Entries", Data.DOFEntries, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public DOFTableNode( string name, DOFTable data ) : base( name, data )
        {
        }

        public DOFTableNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}
