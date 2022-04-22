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
    class ChritmSetNode : BinaryFileNode<ChritmSet>
    {
        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            AddReplaceHandler<ChritmSet>(BinaryFile.Load<ChritmSet>);
            AddExportHandler<ChritmSet>(filePath => Data.Save(filePath));

            AddDirtyCustomHandler("Add Item", () =>
            {
                Item item = new Item();
                Data.Items.Add(item);
                return true;
            }, Keys.None, CustomHandlerFlags.Repopulate | CustomHandlerFlags.ClearMementos);

            AddDirtyCustomHandler("Add Costume", () =>
            {
                Cos cos = new Cos();
                for (int i = 0; i < 25; i++)
                {
                    cos.Parts.Add(0);
                }
                Data.Costumes.Add(cos);
                return true;
            }, Keys.None, CustomHandlerFlags.Repopulate | CustomHandlerFlags.ClearMementos);

            AddDirtyCustomHandler("Add Debug Set", () =>
            {
                Dbg dbg = new Dbg();
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
            Nodes.Add(new ListNode<Item>("Items", Data.Items, x => x.Name));
            Nodes.Add(new ListNode<Cos>("Costumes", Data.Costumes, x => $"COS_{x.CostumeID+1:d3}"));
            Nodes.Add(new ListNode<Dbg>("Debug Sets", Data.DebugSets, x => x.Name));
        }

        protected override void SynchronizeCore()
        {
        }

        public ChritmSetNode(string name, ChritmSet data) : base(name, data)
        {
        }

        public ChritmSetNode(string name, Func<Stream> streamGetter) : base(name, streamGetter)
        {
        }
    }
}
