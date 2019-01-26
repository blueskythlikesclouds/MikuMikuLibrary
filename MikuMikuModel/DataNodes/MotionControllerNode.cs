using System.ComponentModel;
using MikuMikuLibrary.Motions;

namespace MikuMikuModel.DataNodes
{
    public class MotionControllerNode : DataNode<MotionController>
    {
        public override DataNodeFlags Flags => DataNodeFlags.Branch;
        public override DataNodeActionFlags ActionFlags => DataNodeActionFlags.None;

        [Browsable( false )]
        public ListNode<KeyController> KeyControllerListNode { get; set; }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
            Add( KeyControllerListNode =
                new ListNode<KeyController>( "Key controllers", Data.KeyControllers, x => x.Target ) );
        }

        public MotionControllerNode( string name, MotionController data ) : base( name, data )
        {
        }
    }
}