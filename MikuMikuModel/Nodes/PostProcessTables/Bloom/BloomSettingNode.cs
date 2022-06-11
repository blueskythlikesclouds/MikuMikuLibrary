// Code by Thatrandomlurker

using MikuMikuLibrary.PostProcessTables;
using MikuMikuModel.Nodes.TypeConverters;

namespace MikuMikuModel.Nodes.PostProcessTables.Bloom;

public class BloomSettingNode : Node<BloomSetting>
{
    public override NodeFlags Flags => NodeFlags.Rename;

    [Category("General")]
    [TypeConverter(typeof(Vector3TypeConverter))]
    public Vector3 Color
    {
        get => GetProperty<Vector3>();
        set => SetProperty(value);
    }

    [Category("General")]
    [TypeConverter(typeof(Vector3TypeConverter))]
    public Vector3 Brightpass
    {
        get => GetProperty<Vector3>();
        set => SetProperty(value);
    }

    [Category("General")]
    public float Range
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    [Category("General")]
    public uint Flag
    {
        get => GetProperty<uint>();
        set => SetProperty(value);
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

    public BloomSettingNode(string name, BloomSetting data) : base(name, data)
    {
    }
}