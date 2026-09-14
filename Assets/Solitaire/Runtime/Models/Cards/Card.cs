using System;

namespace Solitaire
{
    public sealed class Card
    {
        public int InstanceId { get; }
        public CardDefinition Definition { get; }
        public SuitDefinition Suit { get; }
        public RankDefinition Rank { get; }
        public bool IsFaceUp { get; private set; }

        public Card(
            int instanceId,
            CardDefinition definition,
            SuitDefinition suit,
            RankDefinition rank)
        {
            if (instanceId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(instanceId));
            }

            InstanceId = instanceId;
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            Suit = suit ?? throw new ArgumentNullException(nameof(suit));
            Rank = rank ?? throw new ArgumentNullException(nameof(rank));
        }

        public void SetFaceUp(bool isFaceUp)
        {
            IsFaceUp = isFaceUp;
        }

        public override string ToString()
        {
            return $"Card #{InstanceId}: {Definition.Id}";
        }
    }
}
