namespace Solitaire
{
    public interface IUndoableCommand : ICommand
    {
        void Undo();
    }
}
