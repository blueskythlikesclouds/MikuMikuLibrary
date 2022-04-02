// Code by Thatrandomlurker
using System.ComponentModel;
using System.Numerics;
using MikuMikuLibrary.PostProcessTables.LightTable;
using MikuMikuModel.Nodes.TypeConverters;
using Color = MikuMikuLibrary.Misc.Color;

namespace MikuMikuModel.Nodes.PostProcessTables.Light
{
    public class LightSettingNode : Node<LightSetting>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [Category( "Properties" )]
        public uint ID
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Properties" )]
        public uint LightFlags
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "Properties" )]
        public uint Type
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [TypeConverter( typeof( ColorTypeConverter ) )]
        public Color Diffuse
        {
            get => GetProperty<Color>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [TypeConverter( typeof( ColorTypeConverter ) )]
        public Color Ambient
        {
            get => GetProperty<Color>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [TypeConverter( typeof( ColorTypeConverter ) )]
        public Color Specular
        {
            get => GetProperty<Color>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [TypeConverter( typeof( Vector3TypeConverter ) )]
        public Vector3 Position
        {
            get => GetProperty<Vector3>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Tone Curve" )]
        [TypeConverter( typeof( Vector3TypeConverter ) )]
        public Vector3 ToneCurve
        {
            get => GetProperty<Vector3>();
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

        public LightSettingNode( string name, LightSetting data ) : base( name, data )
        {
        }
    }
}
