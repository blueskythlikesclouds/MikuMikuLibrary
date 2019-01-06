using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MikuMikuModel.DataNodes
{
    public class ReferenceNode : DataNode
    {
        public DataNode Reference => ( DataNode )base.Data;
        public override object Data => Reference.Data;
        public override Type DataType => Reference.DataType;
        public override DataNodeFlags Flags => Reference.Flags;
        public override DataNodeActionFlags ActionFlags => Reference.ActionFlags;
        public override ContextMenuStrip ContextMenuStrip => Reference.ContextMenuStrip;
        public override Control Control => Reference.Control;
        public override Bitmap Icon => Reference.Icon;

        private void OnNodeAdded( object sender, DataNodeNodeEventArgs e ) =>
            Add( new ReferenceNode( e.ChildNode.Name, e.ChildNode ) );

        private void OnNodeRemoved( object sender, DataNodeNodeEventArgs e )
        {
            var reference = Nodes.OfType<ReferenceNode>().FirstOrDefault( x => x.Reference == e.ChildNode );
            if ( reference != null )
                Remove( reference );
        }

        private void OnNodeMoved( object sender, DataNodeNodeMovedEventArgs e )
        {
            var node = mNodes[ e.OldIndex ];
            mNodes.RemoveAt( e.OldIndex );
            mNodes.Insert( e.NewIndex, node );
            OnMove( e.OldIndex, e.NewIndex, node );
        }

        private void OnNameChanged( object sender, DataNodeNameChangedEventArgs e ) => Rename( e.Node.Name );

        protected override void InitializeCore()
        {
            Reference.Initialize();
            Reference.InitializeContextMenuStrip();
            Reference.NodeAdded += OnNodeAdded;
            Reference.NodeRemoved += OnNodeRemoved;
            Reference.NodeMoved += OnNodeMoved;
            Reference.NameChanged += OnNameChanged;
        }

        protected override void InitializeViewCore() => Reference.InitializeView();

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                Reference.NodeAdded -= OnNodeAdded;
                Reference.NodeRemoved -= OnNodeRemoved;
                Reference.NodeMoved -= OnNodeMoved;
                Reference.NameChanged -= OnNameChanged;
            }

            base.Dispose( disposing );
        }

        public override void Export( string filePath ) => Reference.Export( filePath );
        public override string Export() => Reference.Export();
        public override void Import( string filePath ) => Reference.Import( filePath );
        public override void Import() => Reference.Import();
        public override void Replace( object data ) => Reference.Replace( data );
        public override void Replace( string filePath ) => Reference.Replace( filePath );
        public override void Replace() => Reference.Replace();

        public ReferenceNode( string name, object data ) : base( name, data )
        {
        }
    }
}
