using System;
using System.Drawing;
using System.IO;
using MikuMikuLibrary.Motions;
using MikuMikuModel.Configurations;
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

        protected override void Initialize()
        {
            RegisterReplaceHandler<Motion>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                var motion = new Motion();
                {
                    motion.Load( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ] );
                }
                return motion;
            } );
            RegisterExportHandler<Motion>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                {
                    Data.Save( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ] );
                }
            } );
            RegisterExportHandler<MotionSet>( filePath =>
            {
                using ( var motionSet = new MotionSet() )
                {
                    motionSet.Motions.Add( Data );

                    var configuration = ConfigurationList.Instance.CurrentConfiguration;
                    {
                        motionSet.Save( filePath,
                            configuration?.BoneDatabase?.Skeletons?[ 0 ], configuration?.MotionDatabase );
                    }
                }
            } );
        }

        protected override void Load( Motion data, Stream source )
        {
            data.Load( source, SourceConfiguration?.BoneDatabase?.Skeletons?[ 0 ] );
        }

        protected override void PopulateCore()
        {
            if ( !Data.HasBinding )
            {
                var skeleton = SourceConfiguration?.BoneDatabase?.Skeletons?[ 0 ];
                var motionDatabase = SourceConfiguration?.MotionDatabase;

                try
                {
                    Data.Bind( skeleton, motionDatabase );
                }
                catch ( ArgumentNullException )
                {
                }
            }

            if ( Data.HasBinding )
            {
                Nodes.Add( new MotionBindingNode( "Binding", Data.Bind() ) );
            }
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

        public int Id
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