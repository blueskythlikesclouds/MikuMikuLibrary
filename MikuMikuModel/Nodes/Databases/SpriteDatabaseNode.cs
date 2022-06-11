using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.TypeConverters;

namespace MikuMikuModel.Nodes.Databases;

public class SpriteDatabaseNode : BinaryFileNode<SpriteDatabase>
{
    public override NodeFlags Flags =>
        NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

    protected override void Initialize()
    {
        AddExportHandler<SpriteDatabase>(filePath => Data.Save(filePath));
        AddReplaceHandler<SpriteDatabase>(BinaryFile.Load<SpriteDatabase>);

        base.Initialize();
    }

    protected override void PopulateCore()
    {
        Nodes.Add(new ListNode<SpriteSetInfo>("Sprite sets", Data.SpriteSets, x => x.Name));
    }

    protected override void SynchronizeCore()
    {
    }

    public SpriteDatabaseNode(string name, SpriteDatabase data) : base(name, data)
    {
    }

    public SpriteDatabaseNode(string name, Func<Stream> streamGetter) : base(name, streamGetter)
    {
    }
}

public class SpriteSetInfoNode : Node<SpriteSetInfo>
{
    public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Rename;

    [Category("General")]
    [TypeConverter(typeof(IdTypeConverter))]
    public uint Id
    {
        get => GetProperty<uint>();
        set => SetProperty(value);
    }

    [Category("General")]
    [DisplayName("File name")]
    public string FileName
    {
        get => GetProperty<string>();
        set => SetProperty(value);
    }

    protected override void Initialize()
    {
    }

    protected override void PopulateCore()
    {
        Nodes.Add(new ListNode<SpriteInfo>("Sprites", Data.Sprites, x => x.Name));
        Nodes.Add(new ListNode<SpriteTextureInfo>("Textures", Data.Textures, x => x.Name));
    }

    protected override void SynchronizeCore()
    {
    }

    public SpriteSetInfoNode(string name, SpriteSetInfo data) : base(name, data)
    {
    }
}

public class SpriteInfoNode : Node<SpriteInfo>
{
    public override NodeFlags Flags => NodeFlags.Rename;

    [Category("General")]
    [TypeConverter(typeof(IdTypeConverter))]
    public uint Id
    {
        get => GetProperty<uint>();
        set => SetProperty(value);
    }

    [Category("General")]
    public ushort Index
    {
        get => GetProperty<ushort>();
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

    public SpriteInfoNode(string name, SpriteInfo data) : base(name, data)
    {
    }
}

public class SpriteTextureInfoNode : Node<SpriteTextureInfo>
{
    public override NodeFlags Flags => NodeFlags.Rename;

    [Category("General")]
    [TypeConverter(typeof(IdTypeConverter))]
    public uint Id
    {
        get => GetProperty<uint>();
        set => SetProperty(value);
    }

    [Category("General")]
    public ushort Index
    {
        get => GetProperty<ushort>();
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

    public SpriteTextureInfoNode(string name, SpriteTextureInfo data) : base(name, data)
    {
    }
}