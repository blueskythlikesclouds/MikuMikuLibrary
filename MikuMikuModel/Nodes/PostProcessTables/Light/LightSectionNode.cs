// Code by Thatrandomlurker

using MikuMikuLibrary.PostProcessTables;
using MikuMikuModel.Nodes.Collections;

namespace MikuMikuModel.Nodes.PostProcessTables.Light;

public class LightSectionNode : Node<LightSection>
{
    public override NodeFlags Flags => NodeFlags.Add;

    public List<LightSetting> LightSettings => GetProperty<List<LightSetting>>();


    protected override void Initialize()
    {
    }

    protected override void PopulateCore()
    {
        Nodes.Add(new ListNode<LightSetting>("Settings", Data.LightSettings));
    }

    protected override void SynchronizeCore()
    {
    }

    public LightSectionNode(string name, LightSection data) : base(name, data)
    {
    }
}