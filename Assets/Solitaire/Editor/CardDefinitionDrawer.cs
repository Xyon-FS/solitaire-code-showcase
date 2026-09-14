using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Solitaire
{
    [CustomPropertyDrawer(typeof(CardDefinition))]
    public sealed class CardDefinitionDrawer : PropertyDrawer
    {
        private const int ExpandedLineCount = 4;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lineCount = property.isExpanded ? ExpandedLineCount + 1 : 1;
            return lineCount * EditorGUIUtility.singleLineHeight +
                   (lineCount - 1) * EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty suitId =
                property.FindPropertyRelative("suitId");

            SerializedProperty rankId =
                property.FindPropertyRelative("rankId");

            SerializedProperty frontTexture =
                property.FindPropertyRelative("frontTexture");

            string cardLabel =
                $"{suitId.stringValue} {rankId.stringValue}";

            Rect line = GetLine(position, 0);

            property.isExpanded = EditorGUI.Foldout(
                line,
                property.isExpanded,
                new GUIContent(cardLabel),
                true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                suitId = property.FindPropertyRelative("suitId");
                rankId = property.FindPropertyRelative("rankId");
                frontTexture = property.FindPropertyRelative("frontTexture");
                var deck = property.serializedObject.targetObject as DeckDefinition;

                DrawIdPopup(GetLine(position, 1), "Suit", suitId, GetSuitIds(deck));
                DrawIdPopup(GetLine(position, 2), "Rank", rankId, GetRankIds(deck));

                using (new EditorGUI.DisabledScope(true))
                {
                    string cardId = $"{suitId.stringValue}_{rankId.stringValue}";
                    EditorGUI.TextField(GetLine(position, 3), "Card ID", cardId);
                }

                EditorGUI.PropertyField(GetLine(position, 4), frontTexture);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        private static void DrawIdPopup(
            Rect position,
            string label,
            SerializedProperty idProperty,
            IReadOnlyList<string> validIds)
        {
            var options = new List<string>();
            int selectedIndex = -1;

            for (int i = 0; i < validIds.Count; i++)
            {
                options.Add(validIds[i]);

                if (validIds[i] == idProperty.stringValue)
                {
                    selectedIndex = i;
                }
            }

            int validIdOffset = 0;

            if (selectedIndex < 0)
            {
                string missingId = string.IsNullOrWhiteSpace(idProperty.stringValue)
                    ? "<Select>"
                    : $"Missing ({idProperty.stringValue})";
                options.Insert(0, missingId);
                selectedIndex = 0;
                validIdOffset = 1;
            }

            if (options.Count == 0)
            {
                options.Add("<No values defined>");
            }

            int newIndex = EditorGUI.Popup(position, label, selectedIndex, options.ToArray());

            if (newIndex != selectedIndex && newIndex >= validIdOffset)
            {
                idProperty.stringValue = validIds[newIndex - validIdOffset];
            }
        }

        private static List<string> GetSuitIds(DeckDefinition deck)
        {
            var ids = new List<string>();

            if (deck == null)
            {
                return ids;
            }

            for (int i = 0; i < deck.Suits.Count; i++)
            {
                if (deck.Suits[i] != null)
                {
                    ids.Add(deck.Suits[i].Id);
                }
            }

            return ids;
        }

        private static List<string> GetRankIds(DeckDefinition deck)
        {
            var ids = new List<string>();

            if (deck == null)
            {
                return ids;
            }

            for (int i = 0; i < deck.Ranks.Count; i++)
            {
                if (deck.Ranks[i] != null)
                {
                    ids.Add(deck.Ranks[i].Id);
                }
            }

            return ids;
        }

        private static Rect GetLine(Rect position, int index)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            return new Rect(
                position.x,
                position.y + index * (lineHeight + spacing),
                position.width,
                lineHeight);
        }
    }
}
