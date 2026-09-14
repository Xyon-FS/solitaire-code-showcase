using System;
using UnityEngine;
using UnityEngine.UI;

namespace Solitaire
{
    public class UndoButtonView : MonoBehaviour
    {
        public event Action UndoRequested;
        
        [SerializeField] private Button button;

        private void Awake()
        {
            button.onClick.AddListener(NotifyUndoRequested);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(NotifyUndoRequested);
        }

        private void NotifyUndoRequested()
        {
            if (UndoRequested != null)
            {
                UndoRequested.Invoke();
            }
        }

        public void SetInteractable(bool value)
        {
            button.interactable = value;
        }
    }
}
