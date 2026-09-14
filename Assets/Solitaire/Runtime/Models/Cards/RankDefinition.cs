using System;
using UnityEngine;

namespace Solitaire
{
    [Serializable]
    public sealed class RankDefinition
    {
        [SerializeField] private string id;
        [SerializeField] private int order;

        public string Id => id;
        public int Order => order;

        public RankDefinition(string id, int order)
        {
            this.id = id;
            this.order = order;
        }
    }
}
