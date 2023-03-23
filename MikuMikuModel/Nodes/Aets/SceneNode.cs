using MikuMikuLibrary.Aets;
using MikuMikuModel.Nodes.Collections;

namespace MikuMikuModel.Nodes.Aets;

public class AetSceneNode : Node<Scene>
{
    public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Rename;

    [Category("General")]
    [DisplayName("Start frame")]
    public float StartFrame
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("End frame")]
    public float EndFrame
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Frame rate")]
    public float FrameRate
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Background color")]
    public Vector4 BackgroundColor
    {
        get => GetProperty<Vector4>();
        set => SetProperty(value);
    }

    [Category("General")]
    public int Width
    {
        get => GetProperty<int>();
        set => SetProperty(value);
    }

    [Category("General")]
    public int Height
    {
        get => GetProperty<int>();
        set => SetProperty(value);
    }

    protected override void Initialize()
    {
    }

    protected override void PopulateCore()
    {
        if (Data.Camera != null)
            Nodes.Add(new CameraNode("Camera", Data.Camera));

        Nodes.Add(new CompositionNode("Composition", Data.Compositions[^1]));
        Nodes.Add(new ListNode<Video>("Videos", Data.Videos, x => x.Sources.Count == 1 ? x.Sources[0].Name : null));
        Nodes.Add(new ListNode<Audio>("Audios", Data.Audios, x => $"Audio (Sound Id: {x.SoundId})"));
    }

    protected override void SynchronizeCore()
    {
    }

    public AetSceneNode(string name, Scene data) : base(name, data)
    {
    }
}