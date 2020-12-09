using System.ComponentModel;
using MikuMikuLibrary.Aets;

namespace MikuMikuModel.Nodes.Aets
{
    public class VideoSourceNode : Node<VideoSource>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [Category( "General" )]
        public uint Id
        {
            get => GetProperty<uint>();
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

        public VideoSourceNode( string name, VideoSource data ) : base( name, data )
        {
        }
    }
}