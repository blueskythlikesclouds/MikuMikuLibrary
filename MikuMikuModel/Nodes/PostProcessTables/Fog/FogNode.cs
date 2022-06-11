// Code by Thatrandomlurker

using MikuMikuLibrary.IO;
using MikuMikuLibrary.PostProcessTables;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.IO;

namespace MikuMikuModel.Nodes.PostProcessTables.Fog;

class FogTableNode : BinaryFileNode<FogTable>
{
    public override NodeFlags Flags =>
        NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

    protected override void Initialize()
    {
        AddReplaceHandler<FogTable>(BinaryFile.Load<FogTable>);
        AddExportHandler<FogTable>(filePath => Data.Save(filePath));

        base.Initialize();
    }

    protected override void PopulateCore()
    {
        Nodes.Add(new ListNode<FogSetting>("Entries", Data.FogEntries, x => x.Name));
    }

    protected override void SynchronizeCore()
    {
    }

    public FogTableNode(string name, FogTable data) : base(name, data)
    {
    }

    public FogTableNode(string name, Func<Stream> streamGetter) : base(name, streamGetter)
    {
    }
}