using System;
using System.Collections.Generic;

namespace Solitaire
{
    public sealed class Deck
    {
        private readonly List<Card> cards;

        public IReadOnlyList<Card> Cards => cards;
        public int Count => cards.Count;

        public Deck(DeckDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            cards = new List<Card>(definition.Cards.Count);

            for (int i = 0; i < definition.Cards.Count; i++)
            {
                CardDefinition cardDefinition = definition.Cards[i];

                if (cardDefinition == null)
                {
                    throw new InvalidOperationException($"Card definition at index {i} is null.");
                }

                cards.Add(new Card(
                    i,
                    cardDefinition,
                    definition.GetSuit(cardDefinition.SuitId),
                    definition.GetRank(cardDefinition.RankId)));
            }
        }

        public void Shuffle(Random random = null)
        {
            random ??= new Random();

            for (int i = cards.Count - 1; i > 0; i--)
            {
                int swapIndex = random.Next(i + 1);
                (cards[i], cards[swapIndex]) = (cards[swapIndex], cards[i]);
            }
        }

        public bool TryDraw(out Card card)
        {
            if (cards.Count == 0)
            {
                card = null;
                return false;
            }

            int lastIndex = cards.Count - 1;
            card = cards[lastIndex];
            cards.RemoveAt(lastIndex);
            return true;
        }
    }
}
