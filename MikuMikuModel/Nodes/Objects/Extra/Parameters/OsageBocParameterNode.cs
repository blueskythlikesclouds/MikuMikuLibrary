using System.ComponentModel;
using MikuMikuLibrary.Objects.Extra.Parameters;

namespace MikuMikuModel.Nodes.Objects.Extra.Parameters
{
    public class OsageBocParameterNode : Node<OsageBocParameter>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [Category( "General" )]
        [DisplayName( "ST Node" )]
        public uint StNode
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "ED Node" )]
        public uint EdNode
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "ED Root" )]
        public string EdRoot
        {
            get => GetProperty<string>();
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

        public OsageBocParameterNode( string name, OsageBocParameter data ) : base( name, data )
        {
        }
    }
}