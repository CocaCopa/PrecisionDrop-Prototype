using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CocaCopa.EditorUtils {
    public static class EditorCommon {
        /// <summary>
        /// Draws the internal <c>m_Script</c> reference field at the top of a custom inspector,
        /// matching Unity's default inspector behavior.
        /// The field is rendered in a disabled state to prevent modification.
        /// </summary>
        /// <param name="so">
        /// The <see cref="SerializedObject"/> of the inspected target.
        /// </param>
        /// <param name="space">
        /// Optional vertical spacing added after the field.
        /// </param>
        public static void DisplayScriptReference(SerializedObject so, float space = 0f) {
            var scriptProperty = so?.FindProperty("m_Script");
            if (scriptProperty == null) return;

            using (new EditorGUI.DisabledScope(true)) {
                EditorGUILayout.PropertyField(scriptProperty, includeChildren: true);
            }

            if (space > 0f) EditorGUILayout.Space(space);
        }

        /// <summary>
        /// Draws a foldout header and, when expanded, renders the provided serialized properties
        /// using their associated labels.
        /// </summary>
        /// <param name="expand">
        /// Reference to the foldout state. Updated based on user interaction.
        /// </param>
        /// <param name="header">
        /// The foldout header label.
        /// </param>
        /// <param name="properties">
        /// A collection of <see cref="SerializedProperty"/> and <see cref="GUIContent"/> pairs
        /// to render when the foldout is expanded.
        /// </param>
        /// <param name="toggleOnLabelClick">
        /// If true, clicking the header label toggles the foldout.
        /// </param>
        /// <param name="headerStyle">
        /// Optional GUI style for the foldout header. Defaults to <see cref="EditorStyles.foldoutHeader"/>.
        /// </param>
        public static void CreateFoldout(ref bool expand, string header, Dictionary<SerializedProperty, GUIContent> properties, bool toggleOnLabelClick = true, GUIStyle headerStyle = null) {
            if (headerStyle == null) headerStyle = EditorStyles.foldoutHeader;
            expand = EditorGUILayout.Foldout(expand, header, toggleOnLabelClick, headerStyle);
            if (expand) {
                EditorGUI.indentLevel++;
                foreach (var kvp in properties) {
                    EditorGUILayout.PropertyField(kvp.Key, kvp.Value);
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}
