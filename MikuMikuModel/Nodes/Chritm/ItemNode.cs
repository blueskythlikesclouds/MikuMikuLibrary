using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using MikuMikuLibrary.Chritm;

namespace MikuMikuModel.Nodes.Chritm
{
    class ItemNode : Node<Item>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [Category("Object Data")]
        [DisplayName("Objects")]
        public List<ItemObject> Objects => GetProperty<List<ItemObject>>();

        [Category("Object Data")]
        [DisplayName("Object Set IDs")]
        public List<uint> ObjectSetIDs => GetProperty<List<uint>>();

        [Category("Object Data")]
        [DisplayName("Cmnitm Adjust Settings")]
        public List<CmnitmAdjust> CmnitmAdjustSettings => GetProperty<List<CmnitmAdjust>>();
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
        [DisplayName("Attr")]
        public int Attr
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
        [DisplayName("Dest ID")]
        public int DestID
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
        [DisplayName("Org Itm")]
        public int OrgItm
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

        public ItemNode(string name, Item data) : base(name, data)
        {
        }
    }
}
