using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using MikuMikuLibrary.Chritm;

namespace MikuMikuModel.Nodes.Chritm
{
    class CosNode : Node<Cos>
    {
        public override NodeFlags Flags => NodeFlags.None;
        [Category("General")]
        public List<int> Parts => GetProperty<List<int>>();
        [Category("General")]
        public int CostumeID
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

        public CosNode(string name, Cos data) : base(name, data)
        {
        }
    }
}
