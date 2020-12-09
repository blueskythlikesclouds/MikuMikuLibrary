using System.ComponentModel;
using MikuMikuLibrary.Aets;

namespace MikuMikuModel.Nodes.Aets
{
    public class AudioNode : Node<Audio>
    {
        public override NodeFlags Flags => NodeFlags.None;

        [Category( "General" )]
        [DisplayName( "Sound id" )]
        public uint SoundId
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

        public AudioNode( string name, Audio data ) : base( name, data )
        {
        }
    }
}