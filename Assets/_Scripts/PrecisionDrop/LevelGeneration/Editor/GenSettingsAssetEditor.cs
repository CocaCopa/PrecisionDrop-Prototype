#if UNITY_EDITOR
using CocaCopa.EditorUtils;
using PrecisionDrop.LevelGeneration.Runtime;
using PrecisionDrop.LevelGeneration.Unity;
using UnityEditor;
using UnityEngine;

namespace PrecisionDrop.LevelGeneration.EditorUtils {
    [CustomEditor(typeof(GenerationSettingsAsset))]
    internal sealed class GenSettingsAssetEditor : Editor {
        private SerializedProperty dangerConfig;
        private SerializedProperty firstBatchCount;
        private SerializedProperty gapConfigs;
        private SerializedProperty gapVariationConfig;
        private SerializedProperty generationSettings;
        private SerializedProperty maxGapShrinkRatio;
        private SerializedProperty maxPairDensityRatio;
        private SerializedProperty minDangerPairCount;
        private SerializedProperty offsetConfig;
        private SerializedProperty offsetUtilizationRatio;
        private SerializedProperty pairCountConfig;

        private void OnEnable() {
            FindProperties();
        }

        private void FindProperties() {
            generationSettings = serializedObject.FindProperty(nameof(generationSettings));
            firstBatchCount = generationSettings.FindPropertyRelative(nameof(GenerationSettings.firstBatchCount));

            gapConfigs = generationSettings.FindPropertyRelative(nameof(GenerationSettings.gapConfigs));
            dangerConfig = generationSettings.FindPropertyRelative(nameof(GenerationSettings.dangerConfig));

            pairCountConfig = dangerConfig.FindPropertyRelative(nameof(DangerConfig.pairCount));
            maxPairDensityRatio = pairCountConfig.FindPropertyRelative(nameof(PairCountConfig.maxPairDensityRatio));
            minDangerPairCount = pairCountConfig.FindPropertyRelative(nameof(PairCountConfig.minDangerPairCount));

            gapVariationConfig = dangerConfig.FindPropertyRelative(nameof(DangerConfig.gapVariation));
            maxGapShrinkRatio = gapVariationConfig.FindPropertyRelative(nameof(GapVariationConfig.maxGapShrinkRatio));

            offsetConfig = dangerConfig.FindPropertyRelative(nameof(DangerConfig.offset));
            offsetUtilizationRatio = offsetConfig.FindPropertyRelative(nameof(OffsetConfig.offsetUtilizationRatio));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorCommon.DisplayScriptReference(serializedObject);
            DrawGeneralSettings();
            DrawPlatformRecipe();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGeneralSettings() {
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(firstBatchCount);
            EditorGUILayout.Space(20f);
        }

        private void DrawPlatformRecipe() {
            EditorGUILayout.LabelField("Platform Recipe", EditorStyles.boldLabel);
            EditorGUILayout.Space(5f);
            DrawGapConfigs();
            DrawDangerConfigs();
        }

        private void DrawGapConfigs() {
            var headerOpt = new ArrayDrawUtils.HeaderOpt("Gap Configs", EditorStyles.boldLabel);
            var rectOpt = new ArrayDrawUtils.RectOpt(5);
            ArrayDrawUtils.DrawCustomArray(gapConfigs, headerOpt, rectOpt);
            EditorGUILayout.Space(20f);
        }

        private void DrawDangerConfigs() {
            EditorGUILayout.LabelField("Danger Config", EditorStyles.boldLabel);
            DrawPairConfig();
            EditorGUILayout.Space(10);
            DrawGapVariationConfig();
            EditorGUILayout.Space(10);
            DrawOffsetUtilConfig();
        }

        private void DrawPairConfig() {
            EditorGUILayout.LabelField("Pairs", EditorStyles.label);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(maxPairDensityRatio, new GUIContent(ObjectNames.NicifyVariableName(maxPairDensityRatio.name), DangerConfigDocs.PairCount_MaxPairDensityRatio));
            EditorGUILayout.PropertyField(minDangerPairCount, new GUIContent(ObjectNames.NicifyVariableName(minDangerPairCount.name), DangerConfigDocs.PairCount_MinDangerPairCount));
            EditorGUI.indentLevel--;
        }

        private void DrawGapVariationConfig() {
            EditorGUILayout.LabelField("Gap Variation", EditorStyles.label);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(maxGapShrinkRatio, new GUIContent(ObjectNames.NicifyVariableName(maxGapShrinkRatio.name), DangerConfigDocs.GapVariation_MaxGapShrinkRatio));
            EditorGUI.indentLevel--;
        }

        private void DrawOffsetUtilConfig() {
            EditorGUILayout.LabelField("Offset Utilization", EditorStyles.label);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(offsetUtilizationRatio, new GUIContent(ObjectNames.NicifyVariableName(offsetUtilizationRatio.name), DangerConfigDocs.Offset_OffsetUtilizationRatio));
            EditorGUI.indentLevel--;
        }
    }
}
#endif