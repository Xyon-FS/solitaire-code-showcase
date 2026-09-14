using NUnit.Framework;

namespace Solitaire
{
    public sealed class MoveCardCommandTests
    {
        [Test]
        public void Execute_MovesTopCardAndUndoRestoresIt()
        {
            Card lowerCard = CreateCard(0, "ace", 1);
            Card topCard = CreateCard(1, "king", 13);
            var source = new CardPile("source");
            var destination = new CardPile("destination");
            source.Add(lowerCard);
            source.Add(topCard);
            var history = new CommandHistory();
            var controller = new MoveController(new CommandExecutor(), history);

            bool executed = controller.Move(topCard, source, destination);

            Assert.That(executed, Is.True);
            Assert.That(source.Cards, Is.EqualTo(new[] { lowerCard }));
            Assert.That(destination.Cards, Is.EqualTo(new[] { topCard }));
            Assert.That(history.CanUndo, Is.True);

            Assert.That(history.UndoLast(), Is.True);
            Assert.That(source.Cards, Is.EqualTo(new[] { lowerCard, topCard }));
            Assert.That(destination.Count, Is.Zero);
            Assert.That(history.CanUndo, Is.False);
        }

        [Test]
        public void Execute_RejectsCardThatIsNotOnTop()
        {
            Card lowerCard = CreateCard(0, "ace", 1);
            Card topCard = CreateCard(1, "king", 13);
            var source = new CardPile("source");
            var destination = new CardPile("destination");
            source.Add(lowerCard);
            source.Add(topCard);
            var history = new CommandHistory();
            var controller = new MoveController(new CommandExecutor(), history);

            bool executed = controller.Move(lowerCard, source, destination);

            Assert.That(executed, Is.False);
            Assert.That(source.Cards, Is.EqualTo(new[] { lowerCard, topCard }));
            Assert.That(destination.Count, Is.Zero);
            Assert.That(history.CanUndo, Is.False);
        }

        [Test]
        public void History_UndoesMovesInReverseOrder()
        {
            Card firstCard = CreateCard(0, "ace", 1);
            Card secondCard = CreateCard(1, "king", 13);
            var firstPile = new CardPile("first");
            var secondPile = new CardPile("second");
            var thirdPile = new CardPile("third");
            firstPile.Add(firstCard);
            firstPile.Add(secondCard);
            var history = new CommandHistory();
            var controller = new MoveController(new CommandExecutor(), history);

            controller.Move(secondCard, firstPile, secondPile);
            controller.Move(secondCard, secondPile, thirdPile);

            Assert.That(history.Count, Is.EqualTo(2));
            history.UndoLast();
            Assert.That(secondPile.Cards, Is.EqualTo(new[] { secondCard }));
            Assert.That(thirdPile.Count, Is.Zero);

            history.UndoLast();
            Assert.That(firstPile.Cards, Is.EqualTo(new[] { firstCard, secondCard }));
            Assert.That(secondPile.Count, Is.Zero);
        }

        private static Card CreateCard(int instanceId, string rankId, int rankOrder)
        {
            var suit = new SuitDefinition("spades");
            var rank = new RankDefinition(rankId, rankOrder);
            var definition = new CardDefinition(suit.Id, rank.Id);

            return new Card(instanceId, definition, suit, rank);
        }
    }
}
