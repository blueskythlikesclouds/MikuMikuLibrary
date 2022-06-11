// Code by Thatrandomlurker

using MikuMikuLibrary.PostProcessTables;
using MikuMikuModel.Nodes.TypeConverters;

namespace MikuMikuModel.Nodes.PostProcessTables.Light;

public class LightSettingNode : Node<LightSetting>
{
    public override NodeFlags Flags => NodeFlags.Rename;

    [Category("Properties")]
    public uint ID
    {
        get => GetProperty<uint>();
        set => SetProperty(value);
    }

    [Category("Properties")]
    public uint LightFlags
    {
        get => GetProperty<uint>();
        set => SetProperty(value);
    }

    [Category("Properties")]
    public uint Type
    {
        get => GetProperty<uint>();
        set => SetProperty(value);
    }

    [Category("General")]
    [TypeConverter(typeof(Vector4TypeConverter))]
    public Vector4 Diffuse
    {
        get => GetProperty<Vector4>();
        set => SetProperty(value);
    }

    [Category("General")]
    [TypeConverter(typeof(Vector4TypeConverter))]
    public Vector4 Ambient
    {
        get => GetProperty<Vector4>();
        set => SetProperty(value);
    }

    [Category("General")]
    [TypeConverter(typeof(Vector4TypeConverter))]
    public Vector4 Specular
    {
        get => GetProperty<Vector4>();
        set => SetProperty(value);
    }

    [Category("General")]
    [TypeConverter(typeof(Vector3TypeConverter))]
    public Vector3 Position
    {
        get => GetProperty<Vector3>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Tone Curve")]
    [TypeConverter(typeof(Vector3TypeConverter))]
    public Vector3 ToneCurve
    {
        get => GetProperty<Vector3>();
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

    public LightSettingNode(string name, LightSetting data) : base(name, data)
    {
    }
}