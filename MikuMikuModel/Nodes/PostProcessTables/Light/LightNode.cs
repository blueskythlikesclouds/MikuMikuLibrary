// Code by Thatrandomlurker
using MikuMikuModel.Nodes.Collections;
using System;
using System.IO;
using MikuMikuLibrary.PostProcessTables.LightTable;
using MikuMikuLibrary.IO;
using MikuMikuModel.Nodes.IO;

namespace MikuMikuModel.Nodes.PostProcessTables.Light
{
    class LightTableNode : BinaryFileNode<LightTable>
    {
        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            AddReplaceHandler<LightTable>( BinaryFile.Load<LightTable> );
            AddExportHandler<LightTable>( filePath => Data.Save( filePath ) );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<LightSection>( "Sections", Data.LightSections, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public LightTableNode( string name, LightTable data ) : base( name, data )
        {
        }

        public LightTableNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}
