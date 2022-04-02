// Code by Thatrandomlurker
using MikuMikuModel.Nodes.Collections;
using System;
using System.IO;
using MikuMikuLibrary.PostProcessTables.BloomTable;
using MikuMikuLibrary.IO;
using MikuMikuModel.Nodes.IO;

namespace MikuMikuModel.Nodes.PostProcessTables.Bloom
{
    class BloomTableNode : BinaryFileNode<BloomTable>
    {
        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            AddReplaceHandler<BloomTable>( BinaryFile.Load<BloomTable> );
            AddExportHandler<BloomTable>( filePath => Data.Save( filePath ) );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<BloomSetting>( "Entries", Data.BloomTableEntries, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public BloomTableNode(string name, BloomTable data) : base(name, data)
        {
        }

        public BloomTableNode(string name, Func<Stream> streamGetter) : base(name, streamGetter)
        {
        }
    }
}
