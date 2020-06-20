using System;
using System.Collections;
using System.Collections.Generic;

namespace MikuMikuModel.Nodes
{
    public abstract partial class Node<T> : INode where T : class
    {
        private partial class NodeList : IList<INode>
        {
            private readonly List<INode> mNodes = new List<INode>();
            private readonly Node<T> mNode;

            public int Count => mNodes.Count;
            public bool IsReadOnly => !mNode.Flags.HasFlag( NodeFlags.Add );

            public IEnumerator<INode> GetEnumerator()
            {
                return mNodes.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return mNodes.GetEnumerator();
            }

            public void Add( INode item )
            {
                if ( !mNode.Flags.HasFlag( NodeFlags.Add ) || item == null )
                    return;

                item.Parent = mNode;
                item.Renamed += mNode.OnChildRenamed;
                item.Replaced += mNode.OnChildReplaced;

                mNodes.Add( item );
                mNode.OnAdd( item, mNodes.Count - 1 );
                mNode.IsPendingSynchronization = true;

                mNode.PushMemento( new AddMemento( this, item, mNodes.Count - 1 ) );
            }

            public void Clear()
            {
                Clear( false );
            }

            public bool Contains( INode item )
            {
                return mNodes.Contains( item );
            }

            public void CopyTo( INode[] array, int arrayIndex )
            {
                mNodes.CopyTo( array, arrayIndex );
            }

            public bool Remove( INode item )
            {
                return Remove( item, false );
            }

            public int IndexOf( INode item )
            {
                return mNodes.IndexOf( item );
            }

            public void Insert( int index, INode item )
            {
                if ( !mNode.Flags.HasFlag( NodeFlags.Add ) )
                    return;

                item.Parent = mNode;
                item.Renamed += mNode.OnChildRenamed;
                item.Replaced += mNode.OnChildReplaced;

                mNodes.Insert( index, item );
                mNode.OnAdd( item, index );
                mNode.IsPendingSynchronization = true;

                mNode.PushMemento( new AddMemento( this, item, index ) );
            }

            public void RemoveAt( int index )
            {
                RemoveAt( index, false );
            }

            public INode this[ int index ]
            {
                get => mNodes[ index ];
                set => throw new NotSupportedException();
            }

            public void Clear( bool force )
            {
                if ( !mNode.Flags.HasFlag( NodeFlags.Remove ) && !force )
                    return;

                while ( mNodes.Count != 0 )
                    Remove( mNodes[ 0 ], force );
            }

            public bool Remove( INode item, bool force )
            {
                if ( !mNode.Flags.HasFlag( NodeFlags.Remove ) && !force )
                    return false;

                int index = mNodes.IndexOf( item );
                if ( index == -1 )
                    return false;

                bool result = mNodes.Remove( item );
                if ( !result )
                    return false;

                item.Parent = null;
                item.Renamed -= mNode.OnChildRenamed;
                item.Replaced -= mNode.OnChildReplaced;

                mNode.OnRemove( item );
                mNode.IsPendingSynchronization = true;

                mNode.PushMemento( new RemoveMemento( this, item, index ) );

                return true;
            }

            public void RemoveAt( int index, bool force )
            {
                if ( !mNode.Flags.HasFlag( NodeFlags.Remove ) && !force )
                    return;

                var node = mNodes[ index ];

                node.Parent = null;
                node.Renamed -= mNode.OnChildRenamed;
                node.Replaced -= mNode.OnChildReplaced;

                mNodes.RemoveAt( index );
                mNode.OnRemove( node );
                mNode.IsPendingSynchronization = true;

                mNode.PushMemento( new RemoveMemento( this, node, index ) );
            }

            public void Move( int index, int targetIndex )
            {
                if ( !mNode.Flags.HasFlag( NodeFlags.Move ) )
                    return;

                index = Clamp( index );
                targetIndex = Clamp( targetIndex );

                if ( index != targetIndex )
                {
                    var node = mNodes[ index ];
                    mNodes.RemoveAt( index );
                    mNodes.Insert( targetIndex, node );

                    mNode.OnMove( node, index, targetIndex );
                    mNode.IsPendingSynchronization = true;

                    mNode.PushMemento( new MoveMemento( this, index, targetIndex ) );
                }

                int Clamp( int value ) => 
                    value >= mNodes.Count ? mNodes.Count - 1 : value < 0 ? 0 : value;
            }

            public NodeList( Node<T> node )
            {
                mNode = node;
            }
        }
    }
}