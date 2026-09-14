using System;

namespace Solitaire
{
    public sealed class CommandExecutor
    {
        public bool Execute(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return command.Execute();
        }
    }
}
