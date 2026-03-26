using CocaCopa.EditorUtils;
using UnityEditor;
using UnityEngine;

namespace CocaCopa.Unity.Components.EditorTools {
    [CustomEditor(typeof(PopAnimation))]
    public sealed class PopAnimationEditor : Editor {
        private SerializedProperty scaleCurve;
        private SerializedProperty startScaleMultiplier;
        private SerializedProperty scaleSpeed;

        private SerializedProperty delayBeforeFade;
        private SerializedProperty fadeCurve;
        private SerializedProperty fadeSpeed;

        private void OnEnable() {
            scaleCurve = serializedObject.FindProperty(nameof(scaleCurve));
            startScaleMultiplier = serializedObject.FindProperty(nameof(startScaleMultiplier));
            scaleSpeed = serializedObject.FindProperty(nameof(scaleSpeed));

            delayBeforeFade = serializedObject.FindProperty(nameof(delayBeforeFade));
            fadeCurve = serializedObject.FindProperty(nameof(fadeCurve));
            fadeSpeed = serializedObject.FindProperty(nameof(fadeSpeed));
        }

        public override void OnInspectorGUI() {
            EditorCommon.DisplayScriptReference(serializedObject);
            serializedObject.Update();
            DrawScaleSection();
            DrawFadeSection();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawScaleSection() {
            EditorGUILayout.LabelField("Scale Animation", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(scaleCurve, new GUIContent("Curve"));
            EditorGUILayout.PropertyField(startScaleMultiplier, new GUIContent("Start Multiplier"));
            EditorGUILayout.PropertyField(scaleSpeed, new GUIContent("Speed"));

            EditorGUILayout.Space(10f);
        }

        private void DrawFadeSection() {
            EditorGUILayout.LabelField("Fade Out", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(delayBeforeFade, new GUIContent("Delay Before Fade"));
            EditorGUILayout.PropertyField(fadeCurve, new GUIContent("Curve"));
            EditorGUILayout.PropertyField(fadeSpeed, new GUIContent("Speed"));
        }
    }
}