#if UNITY_EDITOR
using PrecisionDrop.LevelGeneration.Runtime;
using UnityEditor;
using UnityEngine;
using RangeInt = CocaCopa.Primitives.RangeInt;

namespace PrecisionDrop.LevelGeneration.EditorUtils {
    [CustomPropertyDrawer(typeof(GapConfig))]
    public sealed class GapConfigDrawer : PropertyDrawer {

        private static readonly GUIContent TotalLabel = new GUIContent(
            "Total Gaps",
            "Number of gaps this configuration generates on a platform."
        );

        private static readonly GUIContent RangeLabel = new GUIContent(
            "Gap Range",
            "The segment index range where gaps may be placed."
        );

        private static readonly GUIContent ChanceLabel = new GUIContent(
            "Chance (%)",
            "Selection probability of this gap configuration."
        );

        private static readonly GUIContent RemainingLabel = new GUIContent(
            "Remaining (%)",
            "How much unallocated chance is left (100 - sum of all chances)."
        );

        private static readonly GUIContent MaxAllowedLabel = new GUIContent(
            "Max Allowed (%)",
            "Maximum value this slider can reach based on other entries."
        );

        // Rich-text style for header so we can color parts of the label.
        private static readonly GUIStyle HeaderStyle = new GUIStyle(EditorStyles.boldLabel) {
            richText = true
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            var totalProp    = property.FindPropertyRelative(nameof(GapConfig.totalGaps));
            var gapRangeProp = property.FindPropertyRelative(nameof(GapConfig.gapRange));
            var chanceProp   = property.FindPropertyRelative(nameof(GapConfig.chance));

            float lineH  = EditorGUIUtility.singleLineHeight;
            float vSpace = EditorGUIUtility.standardVerticalSpacing;

            bool inArray = TryGetParentArrayProperty(property, out _);

            // Header
            var headerRect = new Rect(position.x, position.y, position.width, lineH);
            EditorGUI.LabelField(headerRect, BuildHeaderLabel(property, label), HeaderStyle);

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = oldIndent + 1;

            // Total Gaps
            var totalRect = new Rect(position.x, headerRect.yMax + vSpace, position.width, lineH);
            EditorGUI.BeginChangeCheck();
            int totalValue = EditorGUI.IntField(totalRect, TotalLabel, totalProp.intValue);
            if (EditorGUI.EndChangeCheck()) {
                totalProp.intValue = Mathf.Max(0, totalValue);
            }

            // Gap Range (dynamic height + draw children)
            float rangeH = EditorGUI.GetPropertyHeight(gapRangeProp, RangeLabel, true);
            var rangeRect = new Rect(position.x, totalRect.yMax + vSpace, position.width, rangeH);
            EditorGUI.PropertyField(rangeRect, gapRangeProp, RangeLabel, true);

            // Chance
            var chanceRect = new Rect(position.x, rangeRect.yMax + vSpace, position.width, lineH);

            EditorGUI.BeginChangeCheck();
            float newChance = EditorGUI.Slider(
                chanceRect,
                ChanceLabel,
                chanceProp.floatValue,
                0f,
                100f
            );

            float maxAllowed = inArray ? CalculateMaxAllowedChance(property) : 100f;

            if (EditorGUI.EndChangeCheck()) {
                chanceProp.floatValue = Mathf.Clamp(newChance, 0f, maxAllowed);
            }

            // Remaining + MaxAllowed only when inside array
            if (inArray) {
                float remaining = CalculateRemainingChance(property);

                var remainingRect = new Rect(position.x, chanceRect.yMax + vSpace, position.width, lineH);
                EditorGUI.LabelField(
                    remainingRect,
                    RemainingLabel,
                    new GUIContent($"{remaining:0.##}%")
                );

                var maxAllowedRect = new Rect(position.x, remainingRect.yMax + vSpace, position.width, lineH);
                EditorGUI.LabelField(
                    maxAllowedRect,
                    MaxAllowedLabel,
                    new GUIContent($"{maxAllowed:0.##}%")
                );
            }

            EditorGUI.indentLevel = oldIndent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float lineH  = EditorGUIUtility.singleLineHeight;
            float vSpace = EditorGUIUtility.standardVerticalSpacing;

            var gapRangeProp = property.FindPropertyRelative(nameof(GapConfig.gapRange));
            float rangeH = EditorGUI.GetPropertyHeight(gapRangeProp, RangeLabel, true);

            bool inArray = TryGetParentArrayProperty(property, out _);

            // Header + Total + Range + Chance (+ optional Remaining + MaxAllowed)
            float height =
                lineH + vSpace +     // header
                lineH + vSpace +     // total
                rangeH + vSpace +    // range (dynamic)
                lineH;               // chance

            if (inArray) {
                height += vSpace + lineH; // remaining
                height += vSpace + lineH; // max allowed
            }

            return height;
        }

        private static GUIContent BuildHeaderLabel(SerializedProperty elementProperty, GUIContent fallbackLabel) {
            var chanceProp = elementProperty.FindPropertyRelative(nameof(GapConfig.chance));
            float chance = chanceProp != null ? Mathf.Clamp(chanceProp.floatValue, 0f, 100f) : 0f;

            const string orange = "#FFA500";

            return TryGetElementIndex(elementProperty, out int index)
                ? new GUIContent($"Config {index + 1} <color={orange}>({chance:0.#}%)</color>")
                : new GUIContent($"{fallbackLabel.text} ({chance:0.#}%)");
        }

        private static bool TryGetElementIndex(SerializedProperty elementProperty, out int index) {
            index = -1;

            string path = elementProperty.propertyPath;

            int start = path.LastIndexOf('[');
            int end   = path.LastIndexOf(']');

            if (start < 0 || end <= start)
                return false;

            string number = path.Substring(start + 1, end - start - 1);
            return int.TryParse(number, out index);
        }

        private static float CalculateMaxAllowedChance(SerializedProperty elementProperty) {
            if (!TryGetParentArrayProperty(elementProperty, out var arrayProp))
                return 100f;

            float usedByOthers = 0f;

            for (int i = 0; i < arrayProp.arraySize; i++) {
                var other = arrayProp.GetArrayElementAtIndex(i);

                if (SerializedProperty.EqualContents(other, elementProperty))
                    continue;

                var chanceProp = other.FindPropertyRelative(nameof(GapConfig.chance));
                usedByOthers += Mathf.Clamp(chanceProp.floatValue, 0f, 100f);
            }

            return Mathf.Clamp(100f - usedByOthers, 0f, 100f);
        }

        private static float CalculateRemainingChance(SerializedProperty elementProperty) {
            if (!TryGetParentArrayProperty(elementProperty, out var arrayProp)) {
                var selfChance = elementProperty.FindPropertyRelative(nameof(GapConfig.chance)).floatValue;
                return Mathf.Clamp(100f - selfChance, 0f, 100f);
            }

            float totalUsed = 0f;

            for (int i = 0; i < arrayProp.arraySize; i++) {
                var element = arrayProp.GetArrayElementAtIndex(i);
                var chance  = element.FindPropertyRelative(nameof(GapConfig.chance));
                totalUsed += Mathf.Clamp(chance.floatValue, 0f, 100f);
            }

            return Mathf.Clamp(100f - totalUsed, 0f, 100f);
        }

        private static bool TryGetParentArrayProperty(SerializedProperty elementProperty, out SerializedProperty arrayProperty) {
            arrayProperty = null;

            string path = elementProperty.propertyPath;

            int arrayIndex = path.LastIndexOf(".Array.data[", System.StringComparison.Ordinal);
            if (arrayIndex < 0)
                return false;

            string parentPath = path.Substring(0, arrayIndex);

            SerializedProperty parent =
                elementProperty.serializedObject.FindProperty(parentPath);

            if (parent == null || !parent.isArray)
                return false;

            arrayProperty = parent;
            return true;
        }
    }
}
#endif
