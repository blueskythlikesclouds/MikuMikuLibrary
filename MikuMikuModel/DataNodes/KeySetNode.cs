using System.Collections.Generic;
using System.ComponentModel;
using MikuMikuLibrary.Motions;

namespace MikuMikuModel.DataNodes
{
    public class KeySetNode : DataNode<KeySet>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Leaf;

        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        [DisplayName( "Is interpolated" )]
        public bool IsInterpolated => GetProperty<bool>();

        public List<Key> Keys => GetProperty<List<Key>>();

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        public KeySetNode( string name, KeySet data ) : base( name, data )
        {
        }
    }
}