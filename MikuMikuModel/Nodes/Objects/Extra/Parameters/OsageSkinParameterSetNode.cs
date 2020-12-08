using System;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Objects.Extra.Parameters;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.IO;

namespace MikuMikuModel.Nodes.Objects.Extra.Parameters
{
    public class OsageSkinParameterSetNode : BinaryFileNode<OsageSkinParameterSet>
    {
        public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Rename | NodeFlags.Replace | NodeFlags.Export;

        protected override void Initialize()
        {
            AddReplaceHandler<OsageSkinParameterSet>( BinaryFile.Load<OsageSkinParameterSet> );
            AddExportHandler<OsageSkinParameterSet>( x => Data.Save( x ) );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<OsageSkinParameter>( "Osage block parameters", Data.Parameters, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public OsageSkinParameterSetNode( string name, OsageSkinParameterSet data ) : base( name, data )
        {
        }

        public OsageSkinParameterSetNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}