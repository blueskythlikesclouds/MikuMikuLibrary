using MikuMikuLibrary.CharacterItem;

namespace MikuMikuModel.Nodes.CharacterItem;

class CostumeItemNode : Node<CostumeItem>
{
    public override NodeFlags Flags => NodeFlags.Rename;

    [Category("Object Data")]
    [DisplayName("Objects")]
    public List<CostumeItemObject> Objects => GetProperty<List<CostumeItemObject>>();

    [Category("Object Data")]
    [DisplayName("Object Set IDs")]
    public List<uint> ObjectSetIDs => GetProperty<List<uint>>();

    [Category("Object Data")]
    [DisplayName("Common Item Adjust Settings")]
    public List<CommonItemAdjust> CommonItemAdjustSettings => GetProperty<List<CommonItemAdjust>>();

    [Category("Object Data")]
    [DisplayName("Texture Change Settings")]
    public List<TextureChange> TextureChangeSettings => GetProperty<List<TextureChange>>();

    [Category("Item Data")]
    [DisplayName("Unk01")]
    public int Unk01
    {
        get => GetProperty<int>();
        set => SetProperty<int>(value);
    }

    [Category("Item Data")]
    [DisplayName("Unk02")]
    public int Unk02
    {
        get => GetProperty<int>();
        set => SetProperty<int>(value);
    }

    [Category("Item Data")]
    [DisplayName("Flag")]
    public int Flag
    {
        get => GetProperty<int>();
        set => SetProperty<int>(value);
    }

    [Category("Item Data")]
    [DisplayName("Type")]
    public int Type
    {
        get => GetProperty<int>();
        set => SetProperty<int>(value);
    }

    [Category("Item Data")]
    [DisplayName("Attribute")]
    public int Attribute
    {
        get => GetProperty<int>();
        set => SetProperty<int>(value);
    }

    [Category("Item Data")]
    [DisplayName("Item Number")]
    public int ItemNumber
    {
        get => GetProperty<int>();
        set => SetProperty<int>(value);
    }

    [Category("Item Data")]
    [DisplayName("Destination ID")]
    public int DestinationID
    {
        get => GetProperty<int>();
        set => SetProperty<int>(value);
    }

    [Category("Item Data")]
    [DisplayName("Sub ID")]
    public int SubID
    {
        get => GetProperty<int>();
        set => SetProperty<int>(value);
    }

    [Category("Item Data")]
    [DisplayName("Unk03")]
    public int Unk03
    {
        get => GetProperty<int>();
        set => SetProperty<int>(value);
    }

    [Category("Item Data")]
    [DisplayName("Unk04")]
    public int Unk04
    {
        get => GetProperty<int>();
        set => SetProperty<int>(value);
    }

    [Category("Item Data")]
    [DisplayName("Original Item")]
    public int OriginalItem
    {
        get => GetProperty<int>();
        set => SetProperty<int>(value);
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

    public CostumeItemNode(string name, CostumeItem data) : base(name, data)
    {
    }
}