using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuLibrary.Models;

namespace MikuMikuModel.DataNodes
{
    public class MeshExDataNode : DataNode<MeshExData>
    {
        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Leaf; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get { return DataNodeActionFlags.None; }
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        public MeshExDataNode( string name, MeshExData data ) : base( name, data )
        {
        }
    }
}
