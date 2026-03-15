#if UNITY_EDITOR
using CocaCopa.EditorUtils;
using PrecisionDrop.LevelGeneration.Runtime;
using PrecisionDrop.LevelGeneration.Unity;
using UnityEditor;
using UnityEngine;

namespace PrecisionDrop.LevelGeneration.EditorUtils {
    [CustomEditor(typeof(GenerationSettingsAsset))]
    internal sealed class GenSettingsAssetEditor : Editor {
        private SerializedProperty generationSettings;

        // General Settings
        private SerializedProperty generalSettings;
        private SerializedProperty alignWithPreviousChance;
        private SerializedProperty generalAlignmentOffset;

        // First Batch Config
        private SerializedProperty firstBatchConfig;
        private SerializedProperty batchCount;
        private SerializedProperty gapConfigIndex;
        private SerializedProperty firstBatchAlignmentOffset;

        // Full Danger Section Settings
        private SerializedProperty fullDangerSectionSettings;
        private SerializedProperty maxFullDangerSections;
        private SerializedProperty maxSolidsForFullDanger;
        private SerializedProperty fullDangerChance;

        // Platform Gap(s) Config
        private SerializedProperty gapConfigs;

        // Danger Config
        private SerializedProperty dangerConfig;

        // Danger - Pair Config
        private SerializedProperty pairCountConfig;
        private SerializedProperty dangerPairRange;
        private SerializedProperty maxPairDensityRatio;
        private SerializedProperty minDangerPairCount;

        // Danger - Gap Config
        private SerializedProperty gapVariationConfig;
        private SerializedProperty maxGapShrinkRatio;

        // Danger - Offset Utilization Config
        private SerializedProperty offsetConfig;
        private SerializedProperty offsetUtilizationRatio;
        private SerializedProperty edgeSnapChance;

        private void OnEnable() {
            FindProperties();
        }

        private void FindProperties() {
            generationSettings = serializedObject.FindProperty(nameof(generationSettings));

            gapConfigs = generationSettings.FindPropertyRelative(nameof(GenerationSettings.gapConfigs));
            dangerConfig = generationSettings.FindPropertyRelative(nameof(GenerationSettings.dangerConfig));

            FindGeneralProperties();
            FindFirstBatchProperties();
            FindFullDangerSectionProperties();
            FindDangerPairProperties();
            FindDangerGapProperties();
            FindDangerOffsetUtilizationProperties();
        }

        private void FindGeneralProperties() {
            generalSettings = generationSettings.FindPropertyRelative(nameof(GenerationSettings.generalSettings));
            dangerPairRange = generalSettings.FindPropertyRelative(nameof(GeneralSettings.dangerPairRange));
            alignWithPreviousChance = generalSettings.FindPropertyRelative(nameof(GeneralSettings.alignWithPreviousChance));
            generalAlignmentOffset = generalSettings.FindPropertyRelative(nameof(GeneralSettings.alignmentOffset));
        }

        private void FindFirstBatchProperties() {
            firstBatchConfig = generationSettings.FindPropertyRelative(nameof(GenerationSettings.firstBatchConfig));
            batchCount = firstBatchConfig.FindPropertyRelative(nameof(FirstBatchConfig.batchCount));
            gapConfigIndex = firstBatchConfig.FindPropertyRelative(nameof(FirstBatchConfig.gapConfigIndex));
            firstBatchAlignmentOffset = firstBatchConfig.FindPropertyRelative(nameof(FirstBatchConfig.alignmentOffset));
        }

        private void FindFullDangerSectionProperties() {
            fullDangerSectionSettings = generationSettings.FindPropertyRelative(nameof(GenerationSettings.fullDangerSectionSettings));
            maxFullDangerSections = fullDangerSectionSettings.FindPropertyRelative(nameof(FullDangerSectionSettings.maxFullDangerSections));
            maxSolidsForFullDanger = fullDangerSectionSettings.FindPropertyRelative(nameof(FullDangerSectionSettings.maxSolidsForFullDanger));
            fullDangerChance = fullDangerSectionSettings.FindPropertyRelative(nameof(FullDangerSectionSettings.fullDangerChance));
        }

        private void FindDangerPairProperties() {
            pairCountConfig = dangerConfig.FindPropertyRelative(nameof(DangerConfig.pair));
            maxPairDensityRatio = pairCountConfig.FindPropertyRelative(nameof(PairCountConfig.densityRatio));
            minDangerPairCount = pairCountConfig.FindPropertyRelative(nameof(PairCountConfig.minPairCount));
        }

        private void FindDangerGapProperties() {
            gapVariationConfig = dangerConfig.FindPropertyRelative(nameof(DangerConfig.gapVariation));
            maxGapShrinkRatio = gapVariationConfig.FindPropertyRelative(nameof(GapVariationConfig.shrinkRatio));
        }

        private void FindDangerOffsetUtilizationProperties() {
            offsetConfig = dangerConfig.FindPropertyRelative(nameof(DangerConfig.offset));
            offsetUtilizationRatio = offsetConfig.FindPropertyRelative(nameof(OffsetConfig.ratio));
            edgeSnapChance = offsetConfig.FindPropertyRelative(nameof(OffsetConfig.edgeSnapChance));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorCommon.DisplayScriptReference(serializedObject);
            DrawTooltipArea();
            DrawGeneralSettings();
            DrawFirstBatchSettings();
            DrawFullDangerSectionProperties();
            DrawPlatformRecipe();
            serializedObject.ApplyModifiedProperties();
        }

        // private static void DrawTooltipArea() {
        //     EditorGUILayout.Space(5f);
        //     EditorGUILayout.HelpBox("Hover over any header or field to see its description", MessageType.Info);
        //     EditorGUILayout.Space(5f);
        // }

        private static void DrawTooltipArea() {
            EditorGUILayout.Space(5f);
            Rect rect = EditorGUILayout.GetControlRect(false, 40f);

            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);

            var centered = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                richText = true
            };

            EditorGUI.LabelField(
                rect,
                "Hover over any <b>header</b> or <b>field</b> to see its description.",
                centered
            );
            EditorGUILayout.Space(5f);
        }

        private void DrawGeneralSettings() {
            EditorGUILayout.LabelField(new GUIContent("General Settings", GeneralSettingsDocs.GeneralSettings), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                DrawMinMaxSlider(dangerPairRange, 0, 5, GeneralSettingsDocs.DangerPairRange);
                var alignGuiContent = new GUIContent(ObjectNames.NicifyVariableName(alignWithPreviousChance.name), GeneralSettingsDocs.AlignWithPreviousChance);
                alignWithPreviousChance.intValue = (int)EditorGUILayout.Slider(alignGuiContent, alignWithPreviousChance.intValue, 0f, 100f);
                DrawMinMaxSlider(generalAlignmentOffset, -180, 180, GeneralSettingsDocs.AlignmentOffset);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(20f);
        }

        private void DrawFirstBatchSettings() {
            EditorGUILayout.LabelField(new GUIContent("First Batch", FirstBatchConfigDocs.FirstBatchConfig), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                EditorGUILayout.PropertyField(batchCount, new GUIContent(ObjectNames.NicifyVariableName(batchCount.name), FirstBatchConfigDocs.BatchCount));
                batchCount.intValue = Mathf.Max(0, batchCount.intValue);
                var gapConfigGuiContent = new GUIContent(ObjectNames.NicifyVariableName(gapConfigIndex.name), FirstBatchConfigDocs.GapConfigIndex);
                gapConfigIndex.intValue = (int)EditorGUILayout.Slider(gapConfigGuiContent, gapConfigIndex.intValue, 0, gapConfigs.arraySize - 1);
                DrawMinMaxSlider(firstBatchAlignmentOffset, -180, 180, FirstBatchConfigDocs.AlignmentOffset);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(20f);
        }

        private void DrawFullDangerSectionProperties() {
            EditorGUILayout.LabelField(new GUIContent("Full Danger Section", FullDangerSectionDocs.FullDangerSectionSettings), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                var chanceGuiContent = new GUIContent(ObjectNames.NicifyVariableName(fullDangerChance.name), FullDangerSectionDocs.FullDangerChance);
                fullDangerChance.intValue = (int)EditorGUILayout.Slider(chanceGuiContent, fullDangerChance.intValue, 0f, 100f);
                EditorGUILayout.PropertyField(maxFullDangerSections, new GUIContent(ObjectNames.NicifyVariableName(maxFullDangerSections.name), FullDangerSectionDocs.MaxFullDangerSections));
                maxFullDangerSections.intValue = Mathf.Max(0, maxFullDangerSections.intValue);
                EditorGUILayout.PropertyField(maxSolidsForFullDanger, new GUIContent(ObjectNames.NicifyVariableName(maxSolidsForFullDanger.name), FullDangerSectionDocs.MaxSolidsForFullDanger));
                maxSolidsForFullDanger.intValue = Mathf.Max(0, maxSolidsForFullDanger.intValue);
            }
            EditorGUI.indentLevel--;
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
            EditorGUILayout.LabelField(new GUIContent("Danger Config", DangerConfigDocs.DangerConfig), EditorStyles.boldLabel);
            DrawPairConfig();
            EditorGUILayout.Space(10);
            DrawGapVariationConfig();
            EditorGUILayout.Space(10);
            DrawOffsetUtilConfig();
        }

        private void DrawPairConfig() {
            EditorGUILayout.LabelField(new GUIContent("Pairs", DangerConfigDocs.PairCountConfig), EditorStyles.label);
            EditorGUI.indentLevel++;
            {
                var densityRatioGui = new GUIContent(ObjectNames.NicifyVariableName(maxPairDensityRatio.name), DangerConfigDocs.PairCount_MaxPairDensityRatio);
                maxPairDensityRatio.intValue = (int)EditorGUILayout.Slider(densityRatioGui, maxPairDensityRatio.intValue, 0f, 100f);
                EditorGUILayout.PropertyField(minDangerPairCount, new GUIContent(ObjectNames.NicifyVariableName(minDangerPairCount.name), DangerConfigDocs.PairCount_MinDangerPairCount));
                minDangerPairCount.intValue = Mathf.Max(0, minDangerPairCount.intValue);
            }
            EditorGUI.indentLevel--;
        }

        private void DrawGapVariationConfig() {
            EditorGUILayout.LabelField(new GUIContent("Gap Variation", DangerConfigDocs.GapVariationConfig), EditorStyles.label);
            EditorGUI.indentLevel++;
            {
                var guiContent = new GUIContent(ObjectNames.NicifyVariableName(maxGapShrinkRatio.name), DangerConfigDocs.GapVariation_MaxGapShrinkRatio);
                maxGapShrinkRatio.intValue = (int)EditorGUILayout.Slider(guiContent, maxGapShrinkRatio.intValue, 0f, 100f);
            }
            EditorGUI.indentLevel--;
        }

        private void DrawOffsetUtilConfig() {
            EditorGUILayout.LabelField(new GUIContent("Offset Utilization", DangerConfigDocs.OffsetConfig), EditorStyles.label);
            EditorGUI.indentLevel++;
            {
                DrawMinMaxSlider(offsetUtilizationRatio, 0, 100, DangerConfigDocs.Offset_OffsetUtilizationRatio);
                var edgeSnapGuiContent = new GUIContent(ObjectNames.NicifyVariableName(edgeSnapChance.name), DangerConfigDocs.Offset_EdgeSnapChance);
                edgeSnapChance.intValue = (int)EditorGUILayout.Slider(edgeSnapGuiContent, edgeSnapChance.intValue, 0f, 100f);
            }
            EditorGUI.indentLevel--;
        }

        private static void DrawMinMaxSlider(SerializedProperty rangeIntProperty, int rangeMin, int rangeMax, string tooltip) {
            SerializedProperty minProp = rangeIntProperty.FindPropertyRelative("min");
            SerializedProperty maxProp = rangeIntProperty.FindPropertyRelative("max");
            float min = minProp.intValue;
            float max = maxProp.intValue;
            var content = new GUIContent(ObjectNames.NicifyVariableName(rangeIntProperty.name), tooltip);
            EditorCommon.MinMaxSlider(content, ref min, ref max, rangeMin, rangeMax);
            minProp.intValue = (int)min;
            maxProp.intValue = (int)max;
        }
    }
}
#endif