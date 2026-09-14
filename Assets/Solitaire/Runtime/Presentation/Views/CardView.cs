using System;
using UnityEngine;

namespace Solitaire
{
    public sealed class CardView : MonoBehaviour
    {
        private static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");

        [SerializeField] private MeshRenderer frontRenderer;

        private MaterialPropertyBlock propertyBlock;
        private Card card;
        private CardPileView owner;

        public Card Card
        {
            get
            {
                return card;
            }
        }

        public CardPileView Owner
        {
            get
            {
                return owner;
            }
        }

        public void Initialize(Card card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            if (frontRenderer == null)
            {
                throw new InvalidOperationException("The front renderer is not assigned.");
            }

            this.card = card;
            name = $"Card_{card.Definition.Id}_{card.InstanceId}";

            if (propertyBlock == null)
            {
                propertyBlock = new MaterialPropertyBlock();
            }

            frontRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetTexture(BaseMapId, card.Definition.FrontTexture);
            frontRenderer.SetPropertyBlock(propertyBlock);
        }

        public void SetOwner(CardPileView owner)
        {
            this.owner = owner;
        }

        public void RefreshFacing()
        {
            if (card == null)
            {
                return;
            }

            float yRotation = card.IsFaceUp ? 0f : 180f;
            transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }
}
