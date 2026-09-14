using System;
using System.Collections.Generic;

namespace Solitaire
{
    public sealed class MoveController
    {
        private readonly CommandExecutor executor;
        private readonly CommandHistory history;

        public MoveController(CommandExecutor executor, CommandHistory history)
        {
            this.executor = executor ?? throw new ArgumentNullException(nameof(executor));
            this.history = history ?? throw new ArgumentNullException(nameof(history));
        }
        
        public bool Move(
            Card card,
            CardPile source,
            CardPile destination)
        {
            MoveCardCommand moveCommand =
                new MoveCardCommand(card, source, destination);

            if (!executor.Execute(moveCommand))
            {
                return false;
            }
            
            Card topCard = source.TopCard;

            if (topCard == null || topCard.IsFaceUp)
            {
                history.Record(moveCommand);
                return true;
            }

            RevealTopCardCommand revealCommand =
                new RevealTopCardCommand(source);

            if (executor.Execute(revealCommand))
            {
                history.Record(moveCommand, revealCommand);
            }
            else
            {
                history.Record(moveCommand);
            }

            return true;
        }
    }
}
