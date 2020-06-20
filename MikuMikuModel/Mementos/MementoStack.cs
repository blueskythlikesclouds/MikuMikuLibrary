using System.Collections.Generic;

// iS tHiS a MotHeRfRiCkInG p5 rEfErEnCe?
namespace MikuMikuModel.Mementos
{
    public static class MementoStack
    {
        private static readonly Stack<IMemento> sUndoStack = new Stack<IMemento>();
        private static readonly Stack<IMemento> sRedoStack = new Stack<IMemento>();

        public static bool IsPendingUndo => sUndoStack.Count > 0;
        public static bool IsPendingRedo => sRedoStack.Count > 0;

        public static void Push( IMemento memento )
        {
            sUndoStack.Push( memento );
            sRedoStack.Clear();
        }

        public static void Undo()
        {
            if ( sUndoStack.Count == 0 )
                return;

            var memento = sUndoStack.Pop();
            sRedoStack.Push( memento );

            memento.Undo();
        }

        public static void Redo()
        {
            if ( sRedoStack.Count == 0 )
                return;

            var memento = sRedoStack.Pop();
            sUndoStack.Push( memento );

            memento.Redo();
        }

        public static void Clear()
        {
            sUndoStack.Clear();
            sRedoStack.Clear();
        }
    }
}