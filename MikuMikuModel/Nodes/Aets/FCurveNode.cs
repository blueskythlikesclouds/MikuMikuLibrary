using System.Collections.Generic;
using System.ComponentModel;
using MikuMikuLibrary.Aets;

namespace MikuMikuModel.Nodes.Aets
{
    public class FCurveNode : Node<FCurve>
    {
        public override NodeFlags Flags => NodeFlags.None;

        [Category( "General" )] 
        public List<Key> Keys => GetProperty<List<Key>>();

        [Category( "General" )]
        [DisplayName( "Key count" )]
        public int KeyCount => Data.Keys.Count;

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public FCurveNode( string name, FCurve data ) : base( name, data )
        {
        }
    }
}