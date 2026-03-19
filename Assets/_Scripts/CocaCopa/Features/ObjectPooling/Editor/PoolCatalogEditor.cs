using CocaCopa.EditorUtils;
using CocaCopa.ObjectPooling.Unity.Config;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CocaCopa.ObjectPooling.EditorUtils {
    [CustomEditor(typeof(PoolCatalog))]
    internal sealed class PoolCatalogEditor : Editor {
        private const float ElementPadding = 6f;
        private const float ElementSpacing = 4f;

        private SerializedProperty groupsProp;
        private ReorderableList groupsList;

        private void OnEnable() {
            groupsProp = serializedObject.FindProperty("groups");

            groupsList = new ReorderableList(serializedObject, groupsProp, true, true, true, true)
            {
                drawHeaderCallback = DrawHeader,
                drawElementCallback = DrawElement,
                elementHeightCallback = GetElementHeight,
                drawElementBackgroundCallback = DrawElementBackground
            };
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorCommon.DisplayScriptReference(serializedObject, 10f);
            groupsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Prefab Groups");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            SerializedProperty element = groupsProp.GetArrayElementAtIndex(index);

            rect.x += ElementPadding;
            rect.width -= ElementPadding * 2f;
            rect.y += ElementPadding;
            rect.height = EditorGUI.GetPropertyHeight(element, true);

            GUIContent label = BuildGroupLabel(element, index);
            EditorGUI.PropertyField(rect, element, label, true);
        }

        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused) {
            rect.y += 2f;
            rect.height -= 4f;

            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);
        }

        private float GetElementHeight(int index) {
            SerializedProperty element = groupsProp.GetArrayElementAtIndex(index);
            float propertyHeight = EditorGUI.GetPropertyHeight(element, true);
            return propertyHeight + ElementPadding * 2f + ElementSpacing;
        }

        private static GUIContent BuildGroupLabel(SerializedProperty groupProp, int index) {
            SerializedProperty groupIdProp = groupProp.FindPropertyRelative(nameof(PoolGroup.groupId));
            SerializedProperty entriesProp = groupProp.FindPropertyRelative(nameof(PoolGroup.entries));

            string groupId = string.IsNullOrWhiteSpace(groupIdProp.stringValue)
                ? $"Group {index}"
                : groupIdProp.stringValue;

            int entryCount = entriesProp.arraySize;
            return new GUIContent($"{groupId} ({entryCount} entr{(entryCount == 1 ? "y" : "ies")})");
        }
    }
}