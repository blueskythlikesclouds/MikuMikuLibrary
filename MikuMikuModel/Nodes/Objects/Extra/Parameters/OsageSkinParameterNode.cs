using System.ComponentModel;
using MikuMikuLibrary.Objects.Extra.Parameters;
using MikuMikuModel.Nodes.Collections;

namespace MikuMikuModel.Nodes.Objects.Extra.Parameters
{
    public class OsageSkinParameterNode : Node<OsageSkinParameter>
    {
        public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Rename;

        [Category( "General" )]
        public float Force
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Force gain" )]
        public float ForceGain
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Air resistance" )]
        public float AirResistance
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Rotation Y" )]
        public float RotationY
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Rotation Z" )]
        public float RotationZ
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public float Friction
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Wind affection" )]
        public float WindAffection
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Collision type" )]
        public uint CollisionType
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Init rotation Y" )]
        public float InitRotationY
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Init rotation Z" )]
        public float InitRotationZ
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Hinge Y" )]
        public float HingeY
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Hinge Z" )]
        public float HingeZ
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Collision radius" )]
        public float CollisionRadius
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public float Stiffness
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Move cancel" )]
        public float MoveCancel
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            if ( Data.Collisions.Count > 0 )
                Nodes.Add( new ListNode<OsageCollisionParameter>( "Collisions", Data.Collisions,
                    x => $"{x.Bone0.Name}/{x.Bone1.Name}" ) );

            if ( Data.Bocs.Count > 0 )
                Nodes.Add( new ListNode<OsageBocParameter>( "BOCs", Data.Bocs, x => x.EdRoot ) );

            if ( Data.Nodes.Count > 0 )
                Nodes.Add( new ListNode<OsageNodeParameter>( "Nodes", Data.Nodes ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public OsageSkinParameterNode( string name, OsageSkinParameter data ) : base( name, data )
        {
        }
    }
}