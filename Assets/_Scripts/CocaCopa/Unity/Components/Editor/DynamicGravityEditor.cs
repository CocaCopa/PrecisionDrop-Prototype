using CocaCopa.EditorUtils;
using CocaCopa.Unity.Components.BetterGravity;
using UnityEditor;
using UnityEngine;

namespace CocaCopa.Unity.Components.EditorTools {
    [CustomEditor(typeof(DynamicGravity))]
    internal sealed class DynamicGravityEditor : Editor {
        private SerializedProperty source;
        private SerializedProperty localGravity;
        private SerializedProperty maxMagnitude;
        private SerializedProperty useAccelerationMode;
        private SerializedProperty gravityUp;
        private SerializedProperty gravityDown;
        private SerializedProperty upMultiplier;
        private SerializedProperty downMultiplier;

        private bool upGravityFoldout;
        private bool downGravityFoldout;

        private bool pendingScaleChanges;
        private Rigidbody attachedRb;

        private void OnEnable() {
            FindProperties();
            LoadEditorPrefs();
            attachedRb = (target as DynamicGravity).GetComponent<Rigidbody>();
        }

        private void FindProperties() {
            source = serializedObject.FindProperty(nameof(source));
            localGravity = serializedObject.FindProperty(nameof(localGravity));
            maxMagnitude = serializedObject.FindProperty(nameof(maxMagnitude));
            useAccelerationMode = serializedObject.FindProperty(nameof(useAccelerationMode));
            gravityUp = serializedObject.FindProperty(nameof(gravityUp));
            gravityDown = serializedObject.FindProperty(nameof(gravityDown));
            upMultiplier = serializedObject.FindProperty(nameof(upMultiplier));
            downMultiplier = serializedObject.FindProperty(nameof(downMultiplier));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorCommon.DisplayScriptReference(target);

            if (attachedRb.useGravity) { EditorGUILayout.HelpBox("Gravity is enabled on the attached Rigidbody.", MessageType.Warning); }

            EditorGUI.BeginChangeCheck();
            DrawMainSettings();
            DrawScaleSettings();
            DrawAdvancedSettings();
            if (EditorGUI.EndChangeCheck()) {
                SaveEditorPrefs();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawMainSettings() {
            EditorGUILayout.LabelField("Main Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(source);
            EditorGUI.indentLevel++;
            if (source.enumValueIndex == 0) {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Vector3Field("Global Gravity", Physics.gravity);
                EditorGUI.EndDisabledGroup();
            }
            else { EditorGUILayout.PropertyField(localGravity); }
            EditorGUI.indentLevel--;
            using (new EditorGUI.DisabledGroupScope(true)) {
                EditorGUILayout.LabelField("Not Implemented:");
                EditorGUILayout.PropertyField(maxMagnitude);
            }

            EditorGUILayout.Space(10f);
        }

        private void DrawScaleSettings() {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Scale Settings", EditorStyles.boldLabel);
            ScaleSettingsFoldout(ref upGravityFoldout, "Gravity Up", gravityUp, upMultiplier);
            ScaleSettingsFoldout(ref downGravityFoldout, "Gravity Down", gravityDown, downMultiplier);
            if (EditorGUI.EndChangeCheck()) {
                pendingScaleChanges = true;
            }
            EditorGUILayout.Space(5f);
            if (EditorApplication.isPlaying && pendingScaleChanges) {
                EditorGUILayout.HelpBox("Pending Changes... Press Recalculate for the new settings to take effect", MessageType.Info);
            }
            using (new EditorGUI.DisabledGroupScope(!EditorApplication.isPlaying || !pendingScaleChanges)) {
                string buttonMsg = EditorApplication.isPlaying ? "Recalculate" : "Recalculate (Play Mode only)";
                if (GUILayout.Button(buttonMsg)) {
                    (target as DynamicGravity).UpdateScaleSettings_EditorOnly();
                    pendingScaleChanges = false;
                }
            }
            GUI.enabled = true;
            EditorGUILayout.Space(10f);
        }

        private void ScaleSettingsFoldout(ref bool foldout, string label, SerializedProperty scaleSettings, SerializedProperty multiplier) {
            foldout = EditorGUILayout.Foldout(foldout, label, toggleOnLabelClick: true);
            if (foldout) {
                var curve = scaleSettings.FindPropertyRelative(nameof(ScaleSettings.curve));
                var curveOffset = scaleSettings.FindPropertyRelative(nameof(ScaleSettings.curveOffset));
                var duration = scaleSettings.FindPropertyRelative(nameof(ScaleSettings.duration));

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(curve);
                EditorGUILayout.PropertyField(curveOffset);
                EditorGUILayout.PropertyField(duration);
                using (new EditorGUI.DisabledGroupScope(true)) {
                    EditorGUILayout.PropertyField(multiplier, new GUIContent("Current Multiplier"));
                }
                EditorGUI.indentLevel--;
            }
        }

        private void DrawAdvancedSettings() {
            EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useAccelerationMode);

            EditorGUILayout.Space(10f);
        }

        private void SaveEditorPrefs() {
            EditorReg.Save(target, nameof(upGravityFoldout), upGravityFoldout);
            EditorReg.Save(target, nameof(downGravityFoldout), downGravityFoldout);
        }

        private void LoadEditorPrefs() {
            upGravityFoldout = EditorReg.Load(target, nameof(upGravityFoldout), false);
            downGravityFoldout = EditorReg.Load(target, nameof(downGravityFoldout), false);
        }
    }
}
