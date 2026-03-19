using CocaCopa.ObjectPooling.Unity.Config;
using UnityEditor;
using UnityEngine;

namespace CocaCopa.ObjectPooling.EditorUtils {
    [CustomPropertyDrawer(typeof(PoolGroup))]
    internal sealed class PoolGroupDrawer : PropertyDrawer {
        private const float VerticalSpacing = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty groupIdProp = property.FindPropertyRelative(nameof(PoolGroup.groupId));
            SerializedProperty prewarmGroupProp = property.FindPropertyRelative(nameof(PoolGroup.prewarmGroup));
            SerializedProperty prewarmPercentageProp = property.FindPropertyRelative(nameof(PoolGroup.prewarmPercentage));
            SerializedProperty entriesProp = property.FindPropertyRelative(nameof(PoolGroup.entries));

            float y = position.y;
            float width = position.width;

            var rect = new Rect(position.x, y, width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);
            y += EditorGUIUtility.singleLineHeight + VerticalSpacing;

            if (property.isExpanded) {
                EditorGUI.indentLevel++;

                rect = new Rect(position.x, y, width, EditorGUIUtility.singleLineHeight);
                var groupIdGui = new GUIContent(ObjectNames.NicifyVariableName(groupIdProp.name), PoolGroupDocs.GroupId);
                EditorGUI.PropertyField(rect, groupIdProp, groupIdGui);
                y += EditorGUIUtility.singleLineHeight + VerticalSpacing;

                rect = new Rect(position.x, y, width, EditorGUIUtility.singleLineHeight);
                var prewarmGroupGui = new GUIContent(ObjectNames.NicifyVariableName(prewarmGroupProp.name), PoolGroupDocs.PrewarmGroup);
                EditorGUI.PropertyField(rect, prewarmGroupProp, prewarmGroupGui);
                y += EditorGUIUtility.singleLineHeight + VerticalSpacing;

                if (prewarmGroupProp.boolValue) {
                    rect = new Rect(position.x, y, width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.indentLevel++;
                    var prewarmPercentageGui = new GUIContent("Percent", PoolGroupDocs.PrewarmPercentage);
                    prewarmPercentageProp.intValue = EditorGUI.IntSlider(rect, prewarmPercentageGui, prewarmPercentageProp.intValue, 0, 100);
                    EditorGUI.indentLevel--;
                    y += EditorGUIUtility.singleLineHeight + VerticalSpacing;
                }

                var entriesGui = new GUIContent(ObjectNames.NicifyVariableName(entriesProp.name), PoolGroupDocs.Entries);
                float entriesHeight = EditorGUI.GetPropertyHeight(entriesProp, entriesGui, true);
                rect = new Rect(position.x, y, width, entriesHeight);
                EditorGUI.PropertyField(rect, entriesProp, true);

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            SerializedProperty prewarmGroupProp = property.FindPropertyRelative(nameof(PoolGroup.prewarmGroup));
            SerializedProperty entriesProp = property.FindPropertyRelative(nameof(PoolGroup.entries));

            float height = EditorGUIUtility.singleLineHeight + VerticalSpacing;

            if (!property.isExpanded) { return height; }

            height += EditorGUIUtility.singleLineHeight + VerticalSpacing; // groupId
            height += EditorGUIUtility.singleLineHeight + VerticalSpacing; // prewarmGroup

            if (prewarmGroupProp.boolValue) {
                height += EditorGUIUtility.singleLineHeight + VerticalSpacing; // prewarmCount
            }

            height += EditorGUI.GetPropertyHeight(entriesProp, true); // entries

            return height;
        }
    }
}