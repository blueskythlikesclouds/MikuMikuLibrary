using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace MikuMikuModel.Nodes.TypeConverters
{
    public class DirtyCollectionEditor : CollectionEditor
    {
        private static readonly HashSet<INode> sNodeSet = new HashSet<INode>();

        public DirtyCollectionEditor( Type type ) : base( type )
        {
        }

        private static void OnClosed( object sender, EventArgs e )
        {
            if ( !( sender is Form form ) )
                return;

            if ( form.DialogResult == DialogResult.OK )
            {
                foreach ( var node in sNodeSet )
                    node.NotifyModified( NodeModifyFlags.Property );
            }

            sNodeSet.Remove( form.Tag as INode );
        }

        protected override CollectionForm CreateCollectionForm()
        {
            var form = base.CreateCollectionForm();

            if ( Context.Instance is INode node )
            {
                sNodeSet.Add( node );
                form.Tag = node;
            }

            form.Closed += OnClosed;

            return form;
        }
    }
}