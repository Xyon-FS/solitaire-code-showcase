using UnityEditor;
using UnityEngine;

namespace Solitaire
{
    [CustomEditor(typeof(DeckDefinition))]
    public sealed class DeckDefinitionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            if (GUILayout.Button("Create Standard Configuration"))
            {
                CreateStandardConfiguration();
            }

            if (!GUILayout.Button("Generate Cards"))
            {
                return;
            }

            var deckDefinition = (DeckDefinition)target;
            Undo.RecordObject(deckDefinition, "Generate Card Definitions");
            deckDefinition.GenerateCards();
            EditorUtility.SetDirty(deckDefinition);
        }

        private void CreateStandardConfiguration()
        {
            if (!EditorUtility.DisplayDialog(
                    "Create Standard Deck",
                    "Replace the current suits, ranks and generated cards with a standard 52-card configuration?",
                    "Create",
                    "Cancel"))
            {
                return;
            }

            var deckDefinition = (DeckDefinition)target;
            Undo.RecordObject(deckDefinition, "Create Standard Deck Configuration");
            deckDefinition.CreateStandardConfiguration();
            EditorUtility.SetDirty(deckDefinition);
        }
    }
}
