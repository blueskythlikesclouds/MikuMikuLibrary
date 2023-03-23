using MikuMikuLibrary.IO;
using MikuMikuLibrary.Text;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.Text;

public class TextFileNode : BinaryFileNode<TextFile>
{
    public TextFileNode(string name, TextFile data) : base(name, data)
    {
    }

    public TextFileNode(string name, Func<Stream> streamGetter) : base(name, streamGetter)
    {
    }

    public override NodeFlags Flags =>
        NodeFlags.Export | NodeFlags.Move | NodeFlags.Remove | NodeFlags.Rename | NodeFlags.Replace;

    public override Bitmap Image =>
        ResourceStore.LoadBitmap("Icons/Text.png");

    public override Control Control => new TextViewControl(this);

    protected override void Initialize()
    {
        AddExportHandler<TextFile>(filePath => Data.Save(filePath));
        AddReplaceHandler<TextFile>(BinaryFile.Load<TextFile>);

        base.Initialize();
    }

    protected override void PopulateCore()
    {
    }

    protected override void SynchronizeCore()
    {
    }
}