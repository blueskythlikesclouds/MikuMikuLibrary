// Code by Thatrandomlurker

using MikuMikuLibrary.IO;
using MikuMikuLibrary.PostProcessTables;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.IO;

namespace MikuMikuModel.Nodes.PostProcessTable.ColorCorrect;

class ColorCorrectTableNode : BinaryFileNode<ColorCorrectTable>
{
    public override NodeFlags Flags =>
        NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

    protected override void Initialize()
    {
        AddReplaceHandler<ColorCorrectTable>(BinaryFile.Load<ColorCorrectTable>);
        AddExportHandler<ColorCorrectTable>(filePath => Data.Save(filePath));

        base.Initialize();
    }

    protected override void PopulateCore()
    {
        Nodes.Add(new ListNode<ColorCorrectSetting>("Entries", Data.ColorCorrectTableEntries, x => x.Name));
    }

    protected override void SynchronizeCore()
    {
    }

    public ColorCorrectTableNode(string name, ColorCorrectTable data) : base(name, data)
    {
    }

    public ColorCorrectTableNode(string name, Func<Stream> streamGetter) : base(name, streamGetter)
    {
    }
}