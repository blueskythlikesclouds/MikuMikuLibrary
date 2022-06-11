// Code by Thatrandomlurker

using MikuMikuLibrary.PostProcessTables;
using MikuMikuModel.Nodes.TypeConverters;

namespace MikuMikuModel.Nodes.PostProcessTables.ColorCorrect;

public class ColorCorrectSettingNode : Node<ColorCorrectSetting>
{
    public override NodeFlags Flags => NodeFlags.Rename;

    [Category("General")]
    public float Hue
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    [Category("General")]
    public float Saturation
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    [Category("General")]
    public float Lightness
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    [Category("General")]
    public float Exposure
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    [Category("General")]
    [TypeConverter(typeof(Vector3TypeConverter))]
    public Vector3 Gamma
    {
        get => GetProperty<Vector3>();
        set => SetProperty(value);
    }

    [Category("General")]
    public float Contrast
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

    public ColorCorrectSettingNode(string name, ColorCorrectSetting data) : base(name, data)
    {
    }
}