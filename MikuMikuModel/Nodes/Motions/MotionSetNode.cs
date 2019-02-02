using System;
using System.IO;
using MikuMikuLibrary.Motions;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Misc;

namespace MikuMikuModel.Nodes.Motions
{
    public class MotionSetNode : BinaryFileNode<MotionSet>
    {
        public override NodeFlags Flags => 
            NodeFlags.Add | NodeFlags.Import | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void PopulateCore()
        {
            var motionDatabase = Configuration?.MotionDatabase;
            if ( motionDatabase != null )
            {
                string motionSetName = Path.GetFileNameWithoutExtension( Name );
                if ( motionSetName.StartsWith( "mot_", StringComparison.OrdinalIgnoreCase ) )
                    motionSetName = motionSetName.Substring( 4 );

                var motionSetEntry = motionDatabase.GetMotionSet( motionSetName );
                if ( motionSetEntry == null || Data.Motions.Count != motionSetEntry.Motions.Count )
                    return;

                for ( int i = 0; i < motionSetEntry.Motions.Count; i++ )
                {
                    Data.Motions[ i ].Name = motionSetEntry.Motions[ i ].Name;
                    Data.Motions[ i ].ID = motionSetEntry.Motions[ i ].ID;
                }
            }

            Nodes.Add( new ListNode<Motion>( "Motions", Data.Motions, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public MotionSetNode( string name, MotionSet data ) : base( name, data )
        {
        }

        public MotionSetNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}