using System;
using CocaCopa.EditorUtils;
using UnityEditor;
using PrecisionDrop.Platforms.Unity;

namespace PrecisionDrop.Platforms.EditorUtils {
    [CustomEditor(typeof(PlatformBuilder))]
    internal sealed class PlatformBuilderEditor : Editor {
        private SerializedProperty poolId;
        private SerializedProperty rootId;
        private SerializedProperty partId;
        private SerializedProperty pieceId;
        private SerializedProperty platformsHolder;
        private SerializedProperty totalParts;
        private SerializedProperty segments;
        private SerializedProperty platformGap;

        private void OnEnable() {
            FindProperties();
        }

        private void FindProperties() {
            poolId = serializedObject.FindProperty(nameof(poolId));
            rootId = serializedObject.FindProperty(nameof(rootId));
            partId = serializedObject.FindProperty(nameof(partId));
            pieceId = serializedObject.FindProperty(nameof(pieceId));
            platformsHolder = serializedObject.FindProperty(nameof(platformsHolder));
            totalParts = serializedObject.FindProperty(nameof(totalParts));
            segments = serializedObject.FindProperty(nameof(segments));
            platformGap = serializedObject.FindProperty(nameof(platformGap));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorCommon.DisplayScriptReference(serializedObject);
            DrawPoolSelection();
            DrawHolder();
            DrawSettings();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPoolSelection() {
            EditorGUILayout.PropertyField(poolId);
            EditorGUI.indentLevel++;
            {
                EditorGUILayout.PropertyField(rootId);
                EditorGUILayout.PropertyField(partId);
                EditorGUILayout.PropertyField(pieceId);
            }
            EditorGUI.indentLevel--;
        }

        private void DrawHolder() {
            EditorGUILayout.PropertyField(platformsHolder);
        }

        private void DrawSettings() {
            EditorGUILayout.PropertyField(totalParts);
            EditorGUILayout.PropertyField(segments);
            EditorGUILayout.PropertyField(platformGap);
        }
    }
}