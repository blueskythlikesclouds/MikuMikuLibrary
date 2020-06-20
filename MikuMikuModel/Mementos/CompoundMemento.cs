using System.Collections.Generic;

namespace MikuMikuModel.Mementos
{
    public class CompoundMemento : IMemento
    {
        private readonly IReadOnlyList<IMemento> mMementos;

        public void Undo()
        {
            foreach ( var memento in mMementos )
                memento.Undo();
        }

        public void Redo()
        {
            foreach ( var memento in mMementos )
                memento.Redo();
        }

        public CompoundMemento( IReadOnlyList<IMemento> mementos )
        {
            mMementos = mementos;
        }
    }
}