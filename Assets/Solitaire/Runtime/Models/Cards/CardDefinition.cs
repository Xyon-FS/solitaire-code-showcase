using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Solitaire
{
    [Serializable]
    public sealed class CardDefinition
    {
        [SerializeField] private string suitId;
        [SerializeField] private string rankId;
        [FormerlySerializedAs("frontSprite")]
        [SerializeField] private Texture2D frontTexture;

        public string Id
        {
            get
            {
                return $"{suitId}_{rankId}";
            }
        }

        public string SuitId
        {
            get
            {
                return suitId;
            }
        }

        public string RankId
        {
            get
            {
                return rankId;
            }
        }

        public Texture2D FrontTexture
        {
            get
            {
                return frontTexture;
            }
        }

        public CardDefinition(
            string suitId,
            string rankId,
            Texture2D frontTexture = null)
        {
            this.suitId = suitId;
            this.rankId = rankId;
            this.frontTexture = frontTexture;
        }
    }
}
