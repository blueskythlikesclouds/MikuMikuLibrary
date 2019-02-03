using System;
using System.Collections.Generic;
using System.Drawing;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.Misc
{
    public class ListNode<T> : Node<List<T>> where T : class
    {
        private readonly Func<T, string> mNameGetter;

        public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Remove | NodeFlags.Move;
        public override Bitmap Image => ResourceStore.LoadBitmap( "Icons/Folder.png" );

        public int Count => GetProperty<int>();

        protected override void Initialize()
        {
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