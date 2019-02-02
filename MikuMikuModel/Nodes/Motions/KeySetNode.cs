using System.Collections.Generic;
using System.ComponentModel;
using MikuMikuLibrary.Motions;

namespace MikuMikuModel.Nodes.Motions
{
    public class KeySetNode : Node<KeySet>
    {
        public override NodeFlags Flags => NodeFlags.None;

        [DisplayName( "Is interpolated" )]
        public bool IsInterpolated
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        public List<Key> Keys => GetProperty<List<Key>>();

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public KeySetNode( string name, KeySet data ) : base( name, data )
        {
        }
    }
}