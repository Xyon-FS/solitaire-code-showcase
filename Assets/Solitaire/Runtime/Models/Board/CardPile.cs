using System;
using System.Collections.Generic;

namespace Solitaire
{
    public sealed class CardPile
    {
        private readonly List<Card> cards = new List<Card>();

        public string Id { get; }
        public IReadOnlyList<Card> Cards => cards;
        public int Count => cards.Count;

        public Card TopCard
        {
            get
            {
                if (cards.Count == 0)
                {
                    return null;
                }

                return cards[cards.Count - 1];
            }
        }
        
        public CardPile(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("A pile id is required.", nameof(id));
            }

            Id = id;
        }
        
        

        public void Add(Card card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            if (cards.Contains(card))
            {
                throw new InvalidOperationException($"{card} is already in pile '{Id}'.");
            }

            cards.Add(card);
        }

        public void Insert(int index, Card card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            if (index < 0 || index > cards.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (cards.Contains(card))
            {
                throw new InvalidOperationException($"{card} is already in pile '{Id}'.");
            }

            cards.Insert(index, card);
        }

        public bool Remove(Card card)
        {
            return card != null && cards.Remove(card);
        }

        public bool Contains(Card card)
        {
            return card != null && cards.Contains(card);
        }

        public int IndexOf(Card card)
        {
            return card == null ? -1 : cards.IndexOf(card);
        }
    }
}
