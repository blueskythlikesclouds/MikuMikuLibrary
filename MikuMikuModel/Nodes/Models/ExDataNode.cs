using System.Collections.Generic;
using System.ComponentModel;
using MikuMikuLibrary.Models;

namespace MikuMikuModel.Nodes.Models
{
    public class ExDataNode : Node<ExData>
    {
        public override NodeFlags Flags => NodeFlags.None;

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

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public ExDataNode( string name, ExData data ) : base( name, data )
        {
        }
    }
}