using System;
using System.ComponentModel;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Motions;
using MikuMikuModel.Configurations;

namespace MikuMikuModel.DataNodes
{
    public class MotionSetNode : BinaryFileNode<MotionSet>
    {
        private MotionDatabase mMotionDatabase;
        private SkeletonEntry mSkeletonEntry;

        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags =>
            DataNodeActionFlags.Export | DataNodeActionFlags.Move | DataNodeActionFlags.Remove |
            DataNodeActionFlags.Rename | DataNodeActionFlags.Replace;

        [Browsable( false )]
        public ListNode<Motion> MotionListNode { get; set; }

        protected override void InitializeCore()
        {
            RegisterDataUpdateHandler( () =>
            {
                var motionSet = new MotionSet();
                {
                    motionSet.Motions.AddRange( MotionListNode.Data );
                }
                return motionSet;
            } );
            RegisterReplaceHandler<MotionSet>( BinaryFile.Load<MotionSet> );
            RegisterExportHandler<MotionSet>( path =>
            {
                Configuration configuration;

                bool exported = false;
                if ( mMotionDatabase != null && mSkeletonEntry != null &&
                     ( configuration = ConfigurationList.Instance.FindConfiguration( path ) ) != null )
                {
                    var motionDatabase = configuration.MotionDatabase;
                    var boneDatabase = configuration.BoneDatabase;

                    if ( motionDatabase != null && boneDatabase != null )
                    {
                        var skeletonEntry = boneDatabase.Skeletons[ 0 ];

                        var motionSet = new MotionSet();
                        foreach ( var motion in Data.Motions )
                            motionSet.Motions.Add( motion.GetController( mSkeletonEntry, mMotionDatabase )
                                .ToMotion( skeletonEntry, motionDatabase ) );

                        motionSet.Save( path );

                        exported = true;
                    }
                }

                if ( !exported )
                    Data.Save( path );
            } );
        }

        protected override void InitializeViewCore()
        {
            mMotionDatabase = mConfiguration?.MotionDatabase;
            if ( mMotionDatabase != null )
            {
                var motionSetName = Path.GetFileNameWithoutExtension( Name );
                if ( motionSetName.StartsWith( "mot_", StringComparison.OrdinalIgnoreCase ) )
                    motionSetName = motionSetName.Substring( 4 );

                var motionSetEntry = mMotionDatabase.GetMotionSet( motionSetName );
                if ( motionSetEntry != null && Data.Motions.Count == motionSetEntry.Motions.Count )
                {
                    for ( int i = 0; i < motionSetEntry.Motions.Count; i++ )
                    {
                        Data.Motions[ i ].Name = motionSetEntry.Motions[ i ].Name;
                        Data.Motions[ i ].ID = motionSetEntry.Motions[ i ].ID;
                    }
                }

                var boneDatabase = mConfiguration.BoneDatabase;
                if ( boneDatabase != null )
                    mSkeletonEntry = boneDatabase.Skeletons[ 0 ];
            }

            Add( MotionListNode = new ListNode<Motion>( "Motions", Data.Motions, x => x.Name ) );
        }

        public MotionSetNode( string name, MotionSet data ) : base( name, data )
        {
        }

    }
}