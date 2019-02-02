using System;
using System.Drawing;
using System.IO;
using MikuMikuLibrary.Motions;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Misc;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.Motions
{
    public class MotionNode : BinaryFileNode<Motion>
    {
        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        public override Bitmap Image => ResourceStore.LoadBitmap( "Icons/Motion.png" );

        protected override void PopulateCore()
        {
            var motionDatabase = Configuration?.MotionDatabase;
            var skeletonEntry = Configuration?.BoneDatabase?.Skeletons?[ 0 ];

            if ( skeletonEntry != null )
                Nodes.Add(
                    new MotionControllerNode( "Controller", Data.GetController( skeletonEntry, motionDatabase ) ) );
            else
            {
                Nodes.Add( new ListNode<KeySet>( "Key sets", Data.KeySets ) );
                Nodes.Add( new ListNode<BoneInfo>( "Bones", Data.BoneInfos, x => x.Name ) );
            }
        }

        protected override void SynchronizeCore()
        {
        }

        public MotionNode( string name, Motion data ) : base( name, data )
        {
        }

        public MotionNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }

    public class BoneInfoNode : Node<BoneInfo>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        public int ID
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public BoneInfoNode( string name, BoneInfo data ) : base( name, data )
        {
        }
    }
}