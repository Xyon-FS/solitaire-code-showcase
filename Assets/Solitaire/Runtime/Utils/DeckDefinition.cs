using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire
{
    [CreateAssetMenu(fileName = "DeckDefinition", menuName = "Solitaire/Deck Definition")]
    public sealed class DeckDefinition : ScriptableObject
    {
        [SerializeField] private List<SuitDefinition> suits = new List<SuitDefinition>();
        [SerializeField] private List<RankDefinition> ranks = new List<RankDefinition>();
        [SerializeField] private List<CardDefinition> cards = new List<CardDefinition>();

        public IReadOnlyList<SuitDefinition> Suits => suits;
        public IReadOnlyList<RankDefinition> Ranks => ranks;
        public IReadOnlyList<CardDefinition> Cards => cards;

        public void CreateStandardConfiguration()
        {
            suits = new List<SuitDefinition>
            {
                new SuitDefinition("clubs"),
                new SuitDefinition("diamonds"),
                new SuitDefinition("hearts"),
                new SuitDefinition("spades")
            };

            ranks = new List<RankDefinition>
            {
                new RankDefinition("ace", 1),
                new RankDefinition("two", 2),
                new RankDefinition("three", 3),
                new RankDefinition("four", 4),
                new RankDefinition("five", 5),
                new RankDefinition("six", 6),
                new RankDefinition("seven", 7),
                new RankDefinition("eight", 8),
                new RankDefinition("nine", 9),
                new RankDefinition("ten", 10),
                new RankDefinition("jack", 11),
                new RankDefinition("queen", 12),
                new RankDefinition("king", 13)
            };

            GenerateCards();
        }

        public void GenerateCards()
        {
            cards = CardDefinitionGenerator.Generate(suits, ranks, cards);
        }

        public SuitDefinition GetSuit(string id)
        {
            for (int i = 0; i < suits.Count; i++)
            {
                if (string.Equals(suits[i].Id, id, StringComparison.Ordinal))
                {
                    return suits[i];
                }
            }

            throw new KeyNotFoundException($"Suit '{id}' is not defined by deck '{name}'.");
        }

        public RankDefinition GetRank(string id)
        {
            for (int i = 0; i < ranks.Count; i++)
            {
                if (string.Equals(ranks[i].Id, id, StringComparison.Ordinal))
                {
                    return ranks[i];
                }
            }

            throw new KeyNotFoundException($"Rank '{id}' is not defined by deck '{name}'.");
        }
    }
}
