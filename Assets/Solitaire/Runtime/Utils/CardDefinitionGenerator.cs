using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire
{
    public static class CardDefinitionGenerator
    {
        public static List<CardDefinition> Generate(
            IReadOnlyList<SuitDefinition> suits,
            IReadOnlyList<RankDefinition> ranks,
            IReadOnlyList<CardDefinition> existingCards = null)
        {
            ValidateSuits(suits);
            ValidateRanks(ranks);

            Dictionary<string, Texture2D> existingTextures = CollectExistingTextures(existingCards);
            var cards = new List<CardDefinition>(suits.Count * ranks.Count);

            for (int suitIndex = 0; suitIndex < suits.Count; suitIndex++)
            {
                SuitDefinition suit = suits[suitIndex];

                for (int rankIndex = 0; rankIndex < ranks.Count; rankIndex++)
                {
                    RankDefinition rank = ranks[rankIndex];
                    string cardId = $"{suit.Id}_{rank.Id}";

                    existingTextures.TryGetValue(cardId, out Texture2D texture);
                    cards.Add(new CardDefinition(suit.Id, rank.Id, texture));
                }
            }

            return cards;
        }

        private static Dictionary<string, Texture2D> CollectExistingTextures(
            IReadOnlyList<CardDefinition> existingCards)
        {
            var textures = new Dictionary<string, Texture2D>(StringComparer.Ordinal);

            if (existingCards == null)
            {
                return textures;
            }

            for (int i = 0; i < existingCards.Count; i++)
            {
                CardDefinition card = existingCards[i];

                if (card != null && !string.IsNullOrWhiteSpace(card.Id))
                {
                    textures[card.Id] = card.FrontTexture;
                }
            }

            return textures;
        }

        private static void ValidateSuits(IReadOnlyList<SuitDefinition> suits)
        {
            if (suits == null || suits.Count == 0)
            {
                throw new InvalidOperationException("At least one suit is required.");
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);

            for (int i = 0; i < suits.Count; i++)
            {
                SuitDefinition suit = suits[i];

                if (suit == null || string.IsNullOrWhiteSpace(suit.Id))
                {
                    throw new InvalidOperationException($"Suit at index {i} has no valid id.");
                }

                if (!ids.Add(suit.Id))
                {
                    throw new InvalidOperationException($"Duplicate suit id '{suit.Id}'.");
                }
            }
        }

        private static void ValidateRanks(IReadOnlyList<RankDefinition> ranks)
        {
            if (ranks == null || ranks.Count == 0)
            {
                throw new InvalidOperationException("At least one rank is required.");
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);
            var orders = new HashSet<int>();

            for (int i = 0; i < ranks.Count; i++)
            {
                RankDefinition rank = ranks[i];

                if (rank == null || string.IsNullOrWhiteSpace(rank.Id))
                {
                    throw new InvalidOperationException($"Rank at index {i} has no valid id.");
                }

                if (!ids.Add(rank.Id))
                {
                    throw new InvalidOperationException($"Duplicate rank id '{rank.Id}'.");
                }

                if (!orders.Add(rank.Order))
                {
                    throw new InvalidOperationException($"Duplicate rank order '{rank.Order}'.");
                }
            }
        }
    }
}
