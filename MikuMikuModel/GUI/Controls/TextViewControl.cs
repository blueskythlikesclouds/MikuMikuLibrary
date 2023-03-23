using MikuMikuLibrary.Text;
using MikuMikuModel.Nodes;
using MikuMikuModel.Nodes.Text;

namespace MikuMikuModel.GUI.Controls;

public class TextViewControl : RichTextBox
{
    private readonly TextFileNode mNode;

    protected override void OnTextChanged(EventArgs e)
    {
        if (Text != mNode.Data.Text)
        {
            mNode.Data.Text = Text;
            mNode.NotifyModified(NodeModifyFlags.Property);
        }

        base.OnTextChanged(e);
    }

    public TextViewControl(TextFileNode node)
    {
        mNode = node;
        Text = node.Data.Text;
        BorderStyle = BorderStyle.FixedSingle;
    }
}