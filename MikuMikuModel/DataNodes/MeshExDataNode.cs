using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuLibrary.Models;
using System.ComponentModel;

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

        [DisplayName( "Osage bones" )]
        public List<MeshExOsageBoneEntry> OsageBones => GetProperty<List<MeshExOsageBoneEntry>>();

        [DisplayName( "Osage names" )]
        public List<string> OsageNames => GetProperty<List<string>>();

        [DisplayName( "Ex blocks" )]
        public List<MeshExBlock> ExBlocks => GetProperty<List<MeshExBlock>>();

        [DisplayName( "Bone names" )]
        public List<string> BoneNames => GetProperty<List<string>>();

        [DisplayName( "Entries" )]
        public List<MeshExEntry> Entries => GetProperty<List<MeshExEntry>>();

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
