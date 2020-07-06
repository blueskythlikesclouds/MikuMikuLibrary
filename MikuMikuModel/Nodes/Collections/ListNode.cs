using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MikuMikuModel.Resources;
using Ookii.Dialogs.WinForms;

namespace MikuMikuModel.Nodes.Collections
{
    public class ListNode<T> : Node<List<T>> where T : class
    {
        private readonly Func<T, string> mNameGetter;

        public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Remove | NodeFlags.Move;
        public override Bitmap Image => ResourceStore.LoadBitmap( "Icons/Folder.png" );

        [Category( "General" )] public int Count => GetProperty<int>();

        protected override void Initialize()
        {
            AddCustomHandler( "Append prefix to node names", () =>
            {
                using ( var inputDialog = new InputDialog() { WindowTitle = "Type the prefix." } )
                {
                    if ( inputDialog.ShowDialog() != DialogResult.OK )
                        return;

                    foreach ( var node in Nodes )
                        node.Rename( inputDialog.Input + node.Name );
                }
            } );
            AddCustomHandler( "Append suffix to node names", () =>
            {
                using ( var inputDialog = new InputDialog() { WindowTitle = "Type the suffix." } )
                {
                    if ( inputDialog.ShowDialog() != DialogResult.OK )
                        return;

                    foreach ( var node in Nodes )
                        node.Rename( node.Name + inputDialog.Input );
                }
            } );

            AddCustomHandlerSeparator();

            AddCustomHandler( "Make all node names lowercase", () =>
            {
                foreach ( var node in Nodes )
                    node.Rename( node.Name.ToLowerInvariant() );
            } );
            AddCustomHandler( "Make all node names uppercase", () =>
            {
                foreach ( var node in Nodes )
                    node.Rename( node.Name.ToUpperInvariant() );
            } );

            AddCustomHandlerSeparator();

            AddCustomHandler( "Set all node names", () =>
            {
                using ( var inputDialog = new InputDialog() { WindowTitle = "Type the node name." } )
                {
                    if ( inputDialog.ShowDialog() != DialogResult.OK )
                        return;

                    for ( int i = 0; i < Nodes.Count; i++ )
                        Nodes[ i ].Rename( $"{inputDialog.Input}_{i}" );
                }
            } );

            AddCustomHandlerSeparator();

            AddCustomHandler( "Replace substring in node names", () =>
            {
                using ( var inputDialog1 = new InputDialog { WindowTitle = "Type the substring to be replaced." } )
                {
                    while ( inputDialog1.ShowDialog() == DialogResult.OK )
                    {
                        if ( string.IsNullOrEmpty( inputDialog1.Input ) )
                        {
                            MessageBox.Show( "Please enter a valid substring.", Program.Name,
                                MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }

                        else
                        {
                            using ( var inputDialog2 = new InputDialog
                            {
                                WindowTitle = $"Type the substring to replace all occurrences of {inputDialog1.Input}"
                            } )
                            {
                                if ( inputDialog2.ShowDialog() != DialogResult.OK )
                                    break;

                                foreach ( var node in Nodes )
                                    node.Rename( node.Name.Replace( inputDialog1.Input, inputDialog2.Input ) );
                            }

                            break;
                        }
                    }
                }
            } );
        }

        protected override void PopulateCore()
        {
            for ( int i = 0; i < Data.Count; i++ )
            {
                string nodeName = mNameGetter?.Invoke( Data[ i ] ) ?? $"{typeof( T ).Name} #{i}";
                Nodes.Add( NodeFactory.Create( nodeName, Data[ i ] ) );
            }
        }

        protected override void SynchronizeCore()
        {
            Data.Clear();

            foreach ( var node in Nodes )
                Data.Add( ( T ) node.Data );
        }

        public ListNode( string name, List<T> data ) : base( name, data )
        {
        }

        public ListNode( string name, List<T> data, Func<T, string> nameGetter ) : base( name, data )
        {
            mNameGetter = nameGetter;
        }
    }
}