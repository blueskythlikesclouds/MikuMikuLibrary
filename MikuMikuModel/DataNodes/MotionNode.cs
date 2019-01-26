using System.ComponentModel;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Motions;

namespace MikuMikuModel.DataNodes
{
    public class MotionNode : DataNode<Motion>
    {
        private MotionDatabase mMotionDatabase;
        private SkeletonEntry mSkeletonEntry;

        public override DataNodeFlags Flags => DataNodeFlags.Branch;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        [Browsable( false )]
        public MotionControllerNode MotionControllerNode { get; set; }

        [Browsable( false )]
        public ListNode<KeySet> KeySetListNode { get; set; }

        public int ID => GetProperty<int>();

        [DisplayName( "Frame count" )]
        public int FrameCount => GetProperty<int>();

        protected override void InitializeCore()
        {
            //RegisterDataUpdateHandler( () =>
            //{
            //    Motion motion;

            //    if ( KeySetListNode != null )
            //    {
            //        motion = new Motion();
            //        motion.KeySets.AddRange( KeySetListNode.Data );
            //    }
            //    else
            //        motion = MotionControllerNode.Data.ToMotion( mSkeletonEntry, mMotionDatabase );

            //    motion.ID = Data.ID;
            //    motion.Name = Data.Name;
            //    motion.FrameCount = Data.FrameCount;
            //    motion.HighBits = Data.HighBits;

            //    return motion;
            //} );
        }

        protected override void InitializeViewCore()
        {
            mMotionDatabase = mConfiguration?.MotionDatabase;

            var boneDatabase = mConfiguration?.BoneDatabase;
            if ( boneDatabase != null )
                mSkeletonEntry = boneDatabase.Skeletons[ 0 ];

            if ( mMotionDatabase != null && mSkeletonEntry != null )
                Add( MotionControllerNode = new MotionControllerNode( "Controller",
                    Data.GetController( mSkeletonEntry, mMotionDatabase ) ) );

            else
                Add( KeySetListNode = new ListNode<KeySet>( "Key sets", Data.KeySets ) );
        }

        public MotionNode( string name, Motion data ) : base( name, data )
        {
        }
    }
}