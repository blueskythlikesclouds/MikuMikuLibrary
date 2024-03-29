﻿using MikuMikuLibrary.Aets;
using MikuMikuModel.Nodes.Collections;

namespace MikuMikuModel.Nodes.Aets;

public class VideoNode : Node<Video>
{
    public override NodeFlags Flags => NodeFlags.Add;

    [Category("General")]
    public Vector4 Color
    {
        get => GetProperty<Vector4>();
        set => SetProperty(value);
    }

    [Category("General")]
    public ushort Width
    {
        get => GetProperty<ushort>();
        set => SetProperty(value);
    }

    [Category("General")]
    public ushort Height
    {
        get => GetProperty<ushort>();
        set => SetProperty(value);
    }

    [Category("General")]
    public float Frames
    {
        get => GetProperty<float>();
        set => SetProperty(value);
    }

    protected override void Initialize()
    {
    }

    protected override void PopulateCore()
    {
        Nodes.Add(new ListNode<VideoSource>("Video sources", Data.Sources, x => x.Name));
    }

    protected override void SynchronizeCore()
    {
    }

    public VideoNode(string name, Video data) : base(name, data)
    {
    }
}