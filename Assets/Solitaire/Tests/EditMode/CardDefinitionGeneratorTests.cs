using System.Collections.Generic;
using NUnit.Framework;

namespace Solitaire
{
    public sealed class CardDefinitionGeneratorTests
    {
        [Test]
        public void Generate_CreatesEverySuitAndRankCombination()
        {
            var suits = new List<SuitDefinition>
            {
                new SuitDefinition("hearts"),
                new SuitDefinition("spades")
            };
            var ranks = new List<RankDefinition>
            {
                new RankDefinition("ace", 1),
                new RankDefinition("king", 13)
            };

            List<CardDefinition> cards = CardDefinitionGenerator.Generate(suits, ranks);

            Assert.That(cards, Has.Count.EqualTo(4));
            Assert.That(cards[0].Id, Is.EqualTo("hearts_ace"));
            Assert.That(cards[1].Id, Is.EqualTo("hearts_king"));
            Assert.That(cards[2].Id, Is.EqualTo("spades_ace"));
            Assert.That(cards[3].Id, Is.EqualTo("spades_king"));
        }

        [Test]
        public void Generate_RejectsDuplicateRankOrder()
        {
            var suits = new List<SuitDefinition>
            {
                new SuitDefinition("hearts")
            };
            var ranks = new List<RankDefinition>
            {
                new RankDefinition("ace", 1),
                new RankDefinition("one", 1)
            };

            Assert.Throws<System.InvalidOperationException>(
                () => CardDefinitionGenerator.Generate(suits, ranks));
        }
    }
}
