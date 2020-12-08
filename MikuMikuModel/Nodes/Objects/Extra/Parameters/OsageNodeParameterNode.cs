using System.ComponentModel;
using MikuMikuLibrary.Objects.Extra.Parameters;

namespace MikuMikuModel.Nodes.Objects.Extra.Parameters
{
    public class OsageNodeParameterNode : Node<OsageNodeParameter>
    {
        public override NodeFlags Flags => NodeFlags.None;

        [Category( "General" )]
        [DisplayName( "Hinge Y min" )]
        public float HingeYMin
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Hinge Y max" )]
        public float HingeYMax
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Hinge Y min" )]
        public float HingeZMin
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Hinge Z max" )]
        public float HingeZMax
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public float Radius
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public float Weight
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Inertial cancel" )]
        public float InertialCancel
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

        public OsageNodeParameterNode( string name, OsageNodeParameter data ) : base( name, data )
        {
        }
    }
}