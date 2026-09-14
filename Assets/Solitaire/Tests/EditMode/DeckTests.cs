using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Solitaire
{
    public sealed class DeckTests
    {
        [Test]
        public void StandardConfiguration_CreatesUniqueFiftyTwoCardDeck()
        {
            DeckDefinition definition = ScriptableObject.CreateInstance<DeckDefinition>();

            try
            {
                definition.CreateStandardConfiguration();
                var deck = new Deck(definition);
                var definitionIds = new HashSet<string>();
                var instanceIds = new HashSet<int>();

                for (int i = 0; i < deck.Cards.Count; i++)
                {
                    definitionIds.Add(deck.Cards[i].Definition.Id);
                    instanceIds.Add(deck.Cards[i].InstanceId);
                }

                Assert.That(definition.Suits, Has.Count.EqualTo(4));
                Assert.That(definition.Ranks, Has.Count.EqualTo(13));
                Assert.That(deck.Count, Is.EqualTo(52));
                Assert.That(definitionIds, Has.Count.EqualTo(52));
                Assert.That(instanceIds, Has.Count.EqualTo(52));
            }
            finally
            {
                Object.DestroyImmediate(definition);
            }
        }

        [Test]
        public void TryDraw_RemovesCardFromDeck()
        {
            DeckDefinition definition = ScriptableObject.CreateInstance<DeckDefinition>();

            try
            {
                definition.CreateStandardConfiguration();
                var deck = new Deck(definition);

                bool drawn = deck.TryDraw(out Card card);

                Assert.That(drawn, Is.True);
                Assert.That(card, Is.Not.Null);
                Assert.That(deck.Count, Is.EqualTo(51));
            }
            finally
            {
                Object.DestroyImmediate(definition);
            }
        }
    }
}
