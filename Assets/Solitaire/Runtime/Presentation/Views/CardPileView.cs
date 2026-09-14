using System;
using UnityEngine;

namespace Solitaire
{
    public sealed class CardPileView : MonoBehaviour
    {
        [SerializeField] private string pileId;
        [SerializeField] private Vector3 cardOffset = new Vector3(0f, -0.3f, -0.01f);

        private CardPile pile;

        public string PileId
        {
            get
            {
                return pileId;
            }
        }

        public CardPile Pile
        {
            get
            {
                return pile;
            }
        }

        public void Initialize(CardPile pile)
        {
            if (pile == null)
            {
                throw new ArgumentNullException(nameof(pile));
            }

            this.pile = pile;
        }

        public void Place(CardView cardView, int cardIndex)
        {
            if (cardView == null)
            {
                throw new ArgumentNullException(nameof(cardView));
            }

            if (cardIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cardIndex));
            }

            cardView.transform.SetParent(transform, false);
            cardView.transform.localPosition = cardOffset * cardIndex;
            cardView.SetOwner(this);
            cardView.RefreshFacing();
        }
    }
}
