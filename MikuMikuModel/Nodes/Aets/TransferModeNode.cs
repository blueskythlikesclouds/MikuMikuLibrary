using System.ComponentModel;
using MikuMikuLibrary.Aets;

namespace MikuMikuModel.Nodes.Aets
{
    public class TransferModeNode : Node<TransferMode>
    {
        public override NodeFlags Flags => NodeFlags.None;

        [Category( "General" )]
        [DisplayName( "Blend mode" )]
        public BlendMode BlendMode
        {
            get => GetProperty<BlendMode>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Flags" )]
        public TransferFlags TransferFlags
        {
            get => GetProperty<TransferFlags>( nameof( Data.Flags ) );
            set => SetProperty( value, nameof( Data.Flags ) );
        }

        [Category( "General" )]
        [DisplayName( "Track matte" )]
        public TrackMatte TrackMatte
        {
            get => GetProperty<TrackMatte>();
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

        public TransferModeNode( string name, TransferMode data ) : base( name, data )
        {
        }
    }
}