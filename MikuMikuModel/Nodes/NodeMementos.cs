using System.Collections.Generic;
using MikuMikuModel.Mementos;

namespace MikuMikuModel.Nodes
{
    public abstract partial class Node<T> where T : class
    {
        // you know this whole thing is actually a p5 reference
        private int mIgnoreMementos;

        protected void BeginMementoExecution()
        {
            mIgnoreMementos++;
        }

        protected void PushMemento( IMemento memento )
        {
            if ( mIgnoreMementos > 0 )
                return;

            MementoStack.Push( memento );
        }

        protected void EndMementoExecution()
        {
            mIgnoreMementos--;
        }

        private partial class NodeList
        {
            private class AddMemento : IMemento
            {
                private readonly NodeList mNodeList;
                private readonly INode mNode;
                private readonly int mIndex;

                public void Undo()
                {
                    mNodeList.mNode.BeginMementoExecution();
                    mNodeList.RemoveAt( mIndex );
                    mNodeList.mNode.EndMementoExecution();
                }

                public void Redo()
                {
                    mNodeList.mNode.BeginMementoExecution();
                    mNodeList.Insert( mIndex, mNode );
                    mNodeList.mNode.EndMementoExecution();
                }

                public AddMemento( NodeList nodeList, INode node, int index )
                {
                    mNodeList = nodeList;
                    mNode = node;
                    mIndex = index;
                }
            }

            private class RemoveMemento : IMemento
            {
                private readonly NodeList mNodeList;
                private readonly INode mNode;
                private readonly int mIndex;

                public void Undo()
                {
                    mNodeList.mNode.BeginMementoExecution();
                    mNodeList.Insert( mIndex, mNode );
                    mNodeList.mNode.EndMementoExecution();
                }

                public void Redo()
                {
                    mNodeList.mNode.BeginMementoExecution();
                    mNodeList.RemoveAt( mIndex );
                    mNodeList.mNode.EndMementoExecution();
                }

                public RemoveMemento( NodeList nodeList, INode node, int index )
                {
                    mNodeList = nodeList;
                    mNode = node;
                    mIndex = index;
                }
            }

            private class MoveMemento : IMemento
            {
                private readonly NodeList mNodeList;
                private readonly int mOldIndex;
                private readonly int mNewIndex;

                public void Undo()
                {
                    mNodeList.mNode.BeginMementoExecution();
                    mNodeList.Move( mNewIndex, mOldIndex );
                    mNodeList.mNode.EndMementoExecution();
                }

                public void Redo()
                {
                    mNodeList.mNode.BeginMementoExecution();
                    mNodeList.Move( mOldIndex, mNewIndex );
                    mNodeList.mNode.EndMementoExecution();
                }

                public MoveMemento( NodeList nodeList, int oldIndex, int newIndex )
                {
                    mNodeList = nodeList;
                    mOldIndex = oldIndex;
                    mNewIndex = newIndex;
                }
            }
        }

        private class RenameMemento : IMemento
        {
            private readonly Node<T> mNode;
            private readonly string mOldName;
            private readonly string mNewName;

            public void Undo()
            {
                mNode.BeginMementoExecution();
                mNode.Rename( mOldName );
                mNode.EndMementoExecution();
            }

            public void Redo()
            {
                mNode.BeginMementoExecution();
                mNode.Rename( mNewName );
                mNode.EndMementoExecution();
            }

            public RenameMemento( Node<T> node, string oldName, string newName )
            {
                mNode = node;
                mOldName = oldName;
                mNewName = newName;
            }
        }

        private class PropertyMemento<TProperty> : IMemento
        {
            private readonly Node<T> mNode;
            private readonly string mPropertyName;
            private readonly TProperty mOldPropertyValue;
            private readonly TProperty mNewPropertyValue;

            public void Undo()
            {
                mNode.BeginMementoExecution();
                mNode.SetProperty( mOldPropertyValue, mPropertyName );
                mNode.EndMementoExecution();
            }

            public void Redo()
            {
                mNode.BeginMementoExecution();
                mNode.SetProperty( mNewPropertyValue, mPropertyName );
                mNode.EndMementoExecution();
            }

            public PropertyMemento( Node<T> node, string propertyName, TProperty oldPropertyValue, TProperty newPropertyValue )
            {
                mNode = node;
                mPropertyName = propertyName;
                mOldPropertyValue = oldPropertyValue;
                mNewPropertyValue = newPropertyValue;
            }
        }
    }
}