namespace MikuMikuModel.Mementos
{
    public interface IMemento
    {
        void Undo();
        void Redo();
    }
}