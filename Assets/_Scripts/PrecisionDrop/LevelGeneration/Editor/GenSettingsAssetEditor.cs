#if UNITY_EDITOR
using CocaCopa.EditorUtils;
using UnityEditor;
using PrecisionDrop.LevelGeneration.Runtime;
using PrecisionDrop.LevelGeneration.Unity;

namespace PrecisionDrop.LevelGeneration.EditorUtils {
    [CustomEditor(typeof(GenerationSettingsAsset))]
    internal sealed class GenSettingsAssetEditor : Editor {
        private SerializedProperty generationSettings;
        private SerializedProperty firstBatchCount;
        private SerializedProperty gapConfigs;
        private SerializedProperty dangerConfig;

        private void OnEnable() {
            FindProperties();
        }

        private void FindProperties() {
            generationSettings = serializedObject.FindProperty(nameof(generationSettings));
            firstBatchCount = generationSettings.FindPropertyRelative(nameof(GenerationSettings.firstBatchCount));
            gapConfigs = generationSettings.FindPropertyRelative(nameof(GenerationSettings.gapConfigs));
            dangerConfig = generationSettings.FindPropertyRelative(nameof(GenerationSettings.dangerConfig));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorCommon.DisplayScriptReference(serializedObject);
            DrawGeneralSettings();
            DrawGapConfigs();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGeneralSettings() {
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(firstBatchCount);

            EditorGUILayout.Space(10f);
        }

        private void DrawGapConfigs() {
            EditorGUILayout.LabelField("Platform Recipe", EditorStyles.boldLabel);
            var headerOpt = new ArrayDrawUtils.HeaderOpt("Gap Configs", EditorStyles.boldLabel);
            var rectOpt = new ArrayDrawUtils.RectOpt(5);
            ArrayDrawUtils.DrawCustomArray(gapConfigs, headerOpt, rectOpt);
            EditorGUILayout.PropertyField(dangerConfig);
        }
    }
}
#endif
