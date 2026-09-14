using System;

namespace Solitaire
{
    public sealed class RevealTopCardCommand : IUndoableCommand
    {
        private readonly CardPile pile;

        private Card revealedCard;
        private bool isExecuted;

        public Card RevealedCard
        {
            get
            {
                return revealedCard;
            }
        }

        public RevealTopCardCommand(CardPile pile)
        {
            this.pile = pile ??
                        throw new ArgumentNullException(nameof(pile));
        }

        public bool Execute()
        {
            if (isExecuted)
            {
                return false;
            }

            Card topCard = pile.TopCard;

            if (topCard == null || topCard.IsFaceUp)
            {
                return false;
            }

            revealedCard = topCard;
            revealedCard.SetFaceUp(true);
            isExecuted = true;

            return true;
        }

        public void Undo()
        {
            if (!isExecuted)
            {
                throw new InvalidOperationException(
                    "The reveal command has not been executed.");
            }

            if (!ReferenceEquals(pile.TopCard, revealedCard))
            {
                throw new InvalidOperationException(
                    "The pile changed before the reveal was undone.");
            }

            revealedCard.SetFaceUp(false);
            isExecuted = false;
        }
    }
}