// Code by Thatrandomlurker

using MikuMikuLibrary.PostProcessTables;

namespace MikuMikuModel.Nodes.PostProcessTables.ToonEdge;

public class ToonSettingNode : Node<ToonSetting>
{
    public override NodeFlags Flags => NodeFlags.Rename;

    public uint SettingFlags
    {
        get => GetProperty<uint>();
        set => SetProperty(value);
    }

    public float ToonShineIntensity
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    public float ToonShineFocus
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    public float EdgeAR
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    public float EdgeAG
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    public float EdgeAB
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    public float EdgeBR
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    public float EdgeBG
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    public float EdgeBB
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    public float CharaEdgeThickness
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    public float StageEdgeThickness
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    public float CharaEdgeFace
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    public float StageEdgeFace
    {
        get => GetProperty<float>();
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

    public ToonSettingNode(string name, ToonSetting data) : base(name, data)
    {
    }
}