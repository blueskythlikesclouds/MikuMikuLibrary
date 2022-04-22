using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using MikuMikuLibrary.Chritm;

namespace MikuMikuModel.Nodes.Chritm
{
    class DbgNode : Node<Dbg>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [Category("General")]
        public ulong ID
        {
            get => GetProperty<ulong>();
            set => SetProperty<ulong>(value);
        }

        [Category("General")]
        public List<int> Parts => GetProperty<List<int>>();

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public DbgNode(string name, Dbg data) : base(name, data)
        {
        }
    }
}
