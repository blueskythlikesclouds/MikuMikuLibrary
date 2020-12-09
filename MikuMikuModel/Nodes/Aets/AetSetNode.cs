using System;
using System.IO;
using MikuMikuLibrary.Aets;
using MikuMikuLibrary.IO;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.IO;

namespace MikuMikuModel.Nodes.Aets
{
    public class AetSetNode : BinaryFileNode<AetSet>
    {
        public override NodeFlags Flags => 
            NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            AddReplaceHandler<AetSet>( BinaryFile.Load<AetSet> );
            AddExportHandler<AetSet>( filePath => Data.Save( filePath ) );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<Scene>( "Aet scenes", Data.Scenes, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public AetSetNode( string name, AetSet data ) : base( name, data )
        {
        }

        public AetSetNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}