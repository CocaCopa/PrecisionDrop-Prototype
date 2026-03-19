using CocaCopa.ObjectPooling.Unity.Config;
using UnityEditor;
using UnityEngine;

namespace CocaCopa.ObjectPooling.EditorUtils {
    [CustomPropertyDrawer(typeof(PoolEntry))]
    internal sealed class PoolEntryDrawer : PropertyDrawer {
        private const float VerticalSpacing = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty idProp = property.FindPropertyRelative(nameof(PoolEntry.id));
            SerializedProperty prefabProp = property.FindPropertyRelative(nameof(PoolEntry.prefab));
            SerializedProperty maxPoolCountProp = property.FindPropertyRelative(nameof(PoolEntry.maxPoolCount));
            SerializedProperty prewarmProp = property.FindPropertyRelative(nameof(PoolEntry.prewarm));
            SerializedProperty prewarmCountProp = property.FindPropertyRelative(nameof(PoolEntry.prewarmCount));

            bool groupPrewarms = IsParentGroupPrewarmEnabled(property);

            float y = position.y;
            float width = position.width;

            var rect = new Rect(position.x, y, width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);
            y += EditorGUIUtility.singleLineHeight + VerticalSpacing;

            if (property.isExpanded) {
                EditorGUI.indentLevel++;

                rect = new Rect(position.x, y, width, EditorGUIUtility.singleLineHeight);

                var idGui = new GUIContent(ObjectNames.NicifyVariableName(idProp.name), PoolEntryDocs.Id);
                EditorGUI.PropertyField(rect, idProp, idGui);
                y += EditorGUIUtility.singleLineHeight + VerticalSpacing;

                rect = new Rect(position.x, y, width, EditorGUIUtility.singleLineHeight);
                var prefabGui = new GUIContent(ObjectNames.NicifyVariableName(prefabProp.name), PoolEntryDocs.Prefab);
                EditorGUI.PropertyField(rect, prefabProp, prefabGui);
                y += EditorGUIUtility.singleLineHeight + VerticalSpacing;

                rect = new Rect(position.x, y, width, EditorGUIUtility.singleLineHeight);
                var maxPoolCountGui = new GUIContent(ObjectNames.NicifyVariableName(maxPoolCountProp.name), PoolEntryDocs.MaxPoolCount);
                EditorGUI.PropertyField(rect, maxPoolCountProp, maxPoolCountGui);
                y += EditorGUIUtility.singleLineHeight + VerticalSpacing;

                if (!groupPrewarms) {
                    EditorGUILayout.Space(10f);
                    rect = new Rect(position.x, y, width, EditorGUIUtility.singleLineHeight);

                    var prewarmGui = new GUIContent(ObjectNames.NicifyVariableName(prewarmProp.name), PoolEntryDocs.Prewarm);
                    EditorGUI.PropertyField(rect, prewarmProp, prewarmGui);
                    y += EditorGUIUtility.singleLineHeight + VerticalSpacing;

                    rect = new Rect(position.x, y, width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.indentLevel++;
                    var prewarmCountGui = new GUIContent("Count", PoolEntryDocs.PrewarmCount);
                    EditorGUI.PropertyField(rect, prewarmCountProp, prewarmCountGui);
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            bool groupPrewarms = IsParentGroupPrewarmEnabled(property);

            float height = EditorGUIUtility.singleLineHeight + VerticalSpacing;

            if (!property.isExpanded) { return height; }

            height += EditorGUIUtility.singleLineHeight + VerticalSpacing; // key
            height += EditorGUIUtility.singleLineHeight + VerticalSpacing; // prefab
            height += EditorGUIUtility.singleLineHeight + VerticalSpacing; // maxPoolCount

            if (!groupPrewarms) {
                height += EditorGUIUtility.singleLineHeight + VerticalSpacing; // prewarm
                height += EditorGUIUtility.singleLineHeight + VerticalSpacing; // prewarmCount
            }

            return height;
        }

        private static bool IsParentGroupPrewarmEnabled(SerializedProperty entryProperty) {
            SerializedProperty prewarmGroupProp = FindParentGroupPrewarmProperty(entryProperty);
            return prewarmGroupProp != null && prewarmGroupProp.boolValue;
        }

        private static SerializedProperty FindParentGroupPrewarmProperty(SerializedProperty entryProperty) {
            string path = entryProperty.propertyPath;

            int entriesIndex = path.IndexOf(".entries.Array.data[", System.StringComparison.Ordinal);
            if (entriesIndex < 0) { return null; }

            string groupPath = path.Substring(0, entriesIndex);
            return entryProperty.serializedObject.FindProperty($"{groupPath}.{nameof(PoolGroup.prewarmGroup)}");
        }
    }
}