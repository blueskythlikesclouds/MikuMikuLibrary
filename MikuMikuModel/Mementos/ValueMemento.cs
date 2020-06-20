using System;

namespace MikuMikuModel.Mementos
{
    public class ValueMemento<T> : IMemento
    {
        private readonly T mOldValue;
        private readonly T mNewValue;
        private readonly Action<T> mSetter;

        public void Undo()
        {
            mSetter( mOldValue );
        }

        public void Redo()
        {
            mSetter( mNewValue );
        }

        public ValueMemento( T oldValue, T newValue, Action<T> setter )
        {
            mOldValue = oldValue;
            mNewValue = oldValue;
            mSetter = setter;
        }
    }
}