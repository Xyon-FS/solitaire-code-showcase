using System;
using System.Collections.Generic;

namespace Solitaire
{
    public sealed class CommandHistory
    {
        private readonly Stack<IUndoableCommand[]> operations =
            new Stack<IUndoableCommand[]>();

        public event Action StateChanged;

        public bool CanUndo
        {
            get
            {
                return operations.Count > 0;
            }
        }

        public int Count
        {
            get
            {
                return operations.Count;
            }
        }

        public void Record(params IUndoableCommand[] commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException(nameof(commands));
            }

            if (commands.Length == 0)
            {
                throw new ArgumentException(
                    "At least one command is required.",
                    nameof(commands));
            }

            IUndoableCommand[] operation =
                new IUndoableCommand[commands.Length];

            for (int i = 0; i < commands.Length; i++)
            {
                if (commands[i] == null)
                {
                    throw new ArgumentException(
                        "An operation cannot contain null commands.",
                        nameof(commands));
                }

                operation[i] = commands[i];
            }

            operations.Push(operation);
            NotifyStateChanged();
        }

        public IReadOnlyList<IUndoableCommand> UndoLast()
        {
            if (!CanUndo)
            {
                return null;
            }

            IUndoableCommand[] operation = operations.Peek();

            for (int i = operation.Length - 1; i >= 0; i--)
            {
                operation[i].Undo();
            }

            operations.Pop();
            NotifyStateChanged();

            return operation;
        }

        public void Clear()
        {
            if (!CanUndo)
            {
                return;
            }

            operations.Clear();
            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            if (StateChanged != null)
            {
                StateChanged.Invoke();
            }
        }
    }
}