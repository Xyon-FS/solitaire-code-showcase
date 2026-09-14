using System;
using UnityEngine;

namespace Solitaire
{
    [Serializable]
    public sealed class SuitDefinition
    {
        [SerializeField] private string id;

        public string Id => id;

        public SuitDefinition(string id)
        {
            this.id = id;
        }
    }
}
