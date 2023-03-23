using MikuMikuLibrary.Aets;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.Aets;

public class LayerNode : Node<Layer>
{
    public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Rename;

    public override Bitmap Image
    {
        get
        {
            if (Data != null)
            {
                switch (Data.Item)
                {
                    case Audio audio:
                        return ResourceStore.LoadBitmap("Icons/Audio.png");

                    case Composition composition:
                        return ResourceStore.LoadBitmap("Icons/Composition.png");

                    case Video video:
                        return ResourceStore.LoadBitmap("Icons/Video.png");
                }
            }

            return base.Image;
        }
    }

    [Category("General")]
    [DisplayName("Start time")]
    public float StartTime
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("End time")]
    public float EndTime
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Offset time")]
    public float OffsetTime
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Time scale")]
    public float TimeScale
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Flags")]
    public LayerFlags LayerFlags
    {
        get => GetProperty<LayerFlags>(nameof(Data.Flags));
        set => SetProperty(value, nameof(Data.Flags));
    }

    [Category("General")]
    [DisplayName("Quality")]
    public LayerQuality Quality
    {
        get => GetProperty<LayerQuality>();
        set => SetProperty(value);
    }

    [Category("General")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public object Item
    {
        get => GetProperty<object>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("Parent name")]
    public string ParentName => Data.Parent?.Name ?? "(none)";

    protected override void Initialize()
    {
        if (Data.Markers.Count > 0)
        {
            AddCustomHandler("Add dummy marker",
                () => { Data.Markers.Add(new Marker() { Frame = 0, Name = "DUMMY" }); }, Keys.None,
                CustomHandlerFlags.Repopulate);
        }
    }

    protected override void PopulateCore()
    {
        if (Data.Markers.Count > 0)
            Nodes.Add(new ListNode<Marker>("Markers", Data.Markers, x => x.Name));

        if (Data.Item is Composition composition)
            Nodes.Add(new CompositionNode("Composition", composition));

        if (Data.Audio != null)
            Nodes.Add(new LayerAudioNode("Audio", Data.Audio));

        if (Data.Video != null)
            Nodes.Add(new LayerVideoNode("Video", Data.Video));
    }

    protected override void SynchronizeCore()
    {
    }

    public LayerNode(string name, Layer data) : base(name, data)
    {
    }
}