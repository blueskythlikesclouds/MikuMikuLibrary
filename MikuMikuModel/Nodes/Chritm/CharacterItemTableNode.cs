using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using MikuMikuLibrary.Chritm;
using MikuMikuLibrary.IO;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.IO;

namespace MikuMikuModel.Nodes.Chritm
{
    class CharacterItemTableNode : BinaryFileNode<CharacterItemTable>
    {
        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            AddReplaceHandler<CharacterItemTable>(BinaryFile.Load<CharacterItemTable>);
            AddExportHandler<CharacterItemTable>(filePath => Data.Save(filePath));

            AddDirtyCustomHandler("Add Costume Item", () =>
            {
                CostumeItem item = new CostumeItem();
                Data.Items.Add(item);
                return true;
            }, Keys.None, CustomHandlerFlags.Repopulate | CustomHandlerFlags.ClearMementos);

            AddDirtyCustomHandler("Add Costume", () =>
            {
                Costume cos = new Costume();
                for (int i = 0; i < 25; i++)
                {
                    cos.Parts.Add(0);
                }
                Data.Costumes.Add(cos);
                return true;
            }, Keys.None, CustomHandlerFlags.Repopulate | CustomHandlerFlags.ClearMementos);

            AddDirtyCustomHandler("Add Debug Set", () =>
            {
                DebugSet dbg = new DebugSet();
                for (int i = 0; i < 25; i++)
                {
                    dbg.Parts.Add(0);
                }
                Data.DebugSets.Add(dbg);
                return true;
            }, Keys.None, CustomHandlerFlags.Repopulate | CustomHandlerFlags.ClearMementos);

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            Nodes.Add(new ListNode<CostumeItem>("Items", Data.Items, x => x.Name));
            Nodes.Add(new ListNode<Costume>("Costumes", Data.Costumes, x => $"COS_{x.CostumeID+1:d3}"));
            Nodes.Add(new ListNode<DebugSet>("Debug Sets", Data.DebugSets, x => x.Name));
        }

        protected override void SynchronizeCore()
        {
        }

        public CharacterItemTableNode(string name, CharacterItemTable data) : base(name, data)
        {
        }

        public CharacterItemTableNode(string name, Func<Stream> streamGetter) : base(name, streamGetter)
        {
        }
    }
}
