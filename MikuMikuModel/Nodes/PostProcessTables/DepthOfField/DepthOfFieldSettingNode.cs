// Code by Thatrandomlurker
using MikuMikuLibrary.PostProcessTables.DepthOfFieldTable;

namespace MikuMikuModel.Nodes.PostProcessTables.DepthOfField
{
    public class DOFSettingNode : Node<DOFSetting>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        public uint SettingFlags
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        public float Focus
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float FocusRange
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float FuzzingRange
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Ratio
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Quality
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

        public DOFSettingNode( string name, DOFSetting data ) : base( name, data )
        {
        }
    }
}
