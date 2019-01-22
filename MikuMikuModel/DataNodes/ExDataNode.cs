using MikuMikuLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikuMikuModel.DataNodes
{
    public class ExDataNode : DataNode<ExData>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Leaf;
        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        [DisplayName( "Osage bones" )]
        public List<ExOsageBoneEntry> OsageBones => GetProperty<List<ExOsageBoneEntry>>();

        [DisplayName( "Osage names" )]
        public List<string> OsageNames => GetProperty<List<string>>();

        [DisplayName( "Ex blocks" )]
        public List<ExBlock> ExBlocks => GetProperty<List<ExBlock>>();

        [DisplayName( "Bone names" )]
        public List<string> BoneNames => GetProperty<List<string>>();

        [DisplayName( "Entries" )]
        public List<ExEntry> Entries => GetProperty<List<ExEntry>>();

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        public ExDataNode( string name, ExData data ) : base( name, data )
        {
        }
    }
}
