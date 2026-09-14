using System;

namespace Solitaire
{
    public sealed class MoveCardCommand : IUndoableCommand
    {
        public Card Card => card;

        public CardPile Source => source;

        public int SourceIndex => sourceIndex;

        private readonly Card card;
        private readonly CardPile source;
        private readonly CardPile destination;

        private int sourceIndex;
        private bool isExecuted;

        public MoveCardCommand(Card card, CardPile source, CardPile destination)
        {
            this.card = card ?? throw new ArgumentNullException(nameof(card));
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.destination = destination ?? throw new ArgumentNullException(nameof(destination));
        }

        public bool Execute()
        {
            if (isExecuted || ReferenceEquals(source, destination) || destination.Contains(card))
            {
                return false;
            }

            if (source.Count == 0 || !ReferenceEquals(source.Cards[source.Count - 1], card))
            {
                return false;
            }

            sourceIndex = source.Count - 1;

            if (!source.Remove(card))
            {
                throw new InvalidOperationException("The source pile changed while executing a move.");
            }

            destination.Add(card);
            isExecuted = true;
            return true;
        }

        public void Undo()
        {
            if (!isExecuted)
            {
                throw new InvalidOperationException("The move has not been executed.");
            }

            if (destination.Count == 0 ||
                !ReferenceEquals(destination.Cards[destination.Count - 1], card))
            {
                throw new InvalidOperationException("The destination pile changed before the move was undone.");
            }

            destination.Remove(card);
            source.Insert(sourceIndex, card);
            isExecuted = false;
        }
    }
}
