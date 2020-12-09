using System.ComponentModel;
using MikuMikuLibrary.Aets;

namespace MikuMikuModel.Nodes.Aets
{
    public class MarkerNode : Node<Marker>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [Category( "General" )]
        public float Frame
        {
            get => GetProperty<float>();
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

        public MarkerNode( string name, Marker data ) : base( name, data )
        {
        }
    }
}