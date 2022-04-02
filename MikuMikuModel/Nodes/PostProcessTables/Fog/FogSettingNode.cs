// Code by Thatrandomlurker
using System.ComponentModel;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.PostProcessTables.FogTable;
using MikuMikuModel.Nodes.TypeConverters;

namespace MikuMikuModel.Nodes.PostProcessTables.Fog
{
    public class FogSettingNode : Node<FogSetting>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [Category( "General" )]
        public uint Type
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Group 0" )]
        [DisplayName( "Group 0 Density" )]
        public float Group0Density
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "Group 0" )]
        [DisplayName( "Group 0 Linear Start" )]
        public float Group0LinStart
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "Group 0" )]
        [DisplayName( "Group 0 Linear End" )]
        public float Group0LinEnd
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "Group 0" )]
        [DisplayName( "Group 0 Color" )]
        [TypeConverter( typeof( ColorTypeConverter ) )]
        public Color Group0Color
        {
            get => GetProperty<Color>();
            set => SetProperty( value );
        }

        [Category( "Group 1" )]
        [DisplayName( "Group 1 Density" )]
        public float Group1Density
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "Group 1" )]
        [DisplayName( "Group 1 Linear Start" )]
        public float Group1LinStart
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "Group 1" )]
        [DisplayName( "Group 1 Linear End" )]
        public float Group1LinEnd
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "Group 1" )]
        [DisplayName( "Group 1 Color" )]
        [TypeConverter( typeof( ColorTypeConverter ) )]
        public Color Group1Color
        {
            get => GetProperty<Color>();
            set => SetProperty( value );
        }

        [Category( "Unknowns" )]
        public uint Unk1
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Unknowns" )]
        public float Unk2
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "Unknowns" )]
        public float Unk3
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "Unknowns" )]
        public float Unk4
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        [Category( "Unknowns" )]
        public float Unk5
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

        public FogSettingNode( string name, FogSetting data ) : base( name, data )
        {
        }
    }
}
