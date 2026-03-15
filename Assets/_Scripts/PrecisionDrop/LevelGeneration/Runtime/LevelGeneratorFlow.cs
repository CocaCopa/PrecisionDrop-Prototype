using System;
using System.Collections.Generic;
using CocaCopa.Core;
using CocaCopa.Core.Randomization;
using CocaCopa.Primitives;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.Platforms.Contracts;

namespace PrecisionDrop.LevelGeneration.Runtime {
    internal sealed class LevelGeneratorFlow : IDisposable {
        private readonly IGameFlow gameFlow;
        private readonly IPlatformBuilder generator;
        private readonly GenerationSettings genSettings;
        private readonly int totalSegments;

        private float rotationY;
        private int totalPassCount;

        internal LevelGeneratorFlow(GenerationSettings genSettings, IGameFlow gameFlow, IPlatformBuilder generator) {
            totalSegments = generator.PlatformSegments;
            this.gameFlow = gameFlow;
            this.generator = generator;
            this.genSettings = genSettings;
        }

        private int FirstBatchCount => genSettings.firstBatchConfig.batchCount;
        private RangeInt FirstBatchAlignmentOffset => genSettings.firstBatchConfig.alignmentOffset;
        private RangeInt GeneralAlignmentOffset => genSettings.generalSettings.alignmentOffset;
        private int DefaultPlatformGapConfig => genSettings.firstBatchConfig.gapConfigIndex;

        public void Dispose() {
            gameFlow.OnPlayerPassedPlatform -= GameFlow_OnPlayerPassedPlatform;
        }

        internal void Initialize() {
            gameFlow.OnPlayerPassedPlatform += GameFlow_OnPlayerPassedPlatform;
            CreateFirstBatch();
        }

        private void CreateFirstBatch() {
            for (int i = 0; i < FirstBatchCount; i++) {
                rotationY = RandomUtil.Float(FirstBatchAlignmentOffset.min, FirstBatchAlignmentOffset.max);
                GapConfig gapConfig = genSettings.gapConfigs[DefaultPlatformGapConfig];
                GeneratePlatform(gapConfig, false);
            }
        }

        private void GameFlow_OnPlayerPassedPlatform() {
            totalPassCount++;

            if (ShouldAlignWithPrevious()) { rotationY += RandomUtil.Float(GeneralAlignmentOffset.min, GeneralAlignmentOffset.max); }
            // Pick an offset from the remaining circular arc outside the alignment window.
            else { rotationY += RandomUtil.Float(GeneralAlignmentOffset.max, 360f + GeneralAlignmentOffset.min); }

            GapConfig gapConfig = GenUtils.GetRandomGapConfig(genSettings.gapConfigs);
            GeneratePlatform(gapConfig);
        }

        private void GeneratePlatform(GapConfig gapConfig, bool alsoGenerateDanger = true) {
            RangeInt[] gapPositions = GapGen.BuildGapRanges(
                totalSegments,
                gapConfig.totalGaps,
                GenUtils.ToBaseRange(gapConfig.gapRange)
            );

            RangeInt[] dangerPositions = alsoGenerateDanger ? CalculateDangerSections(gapPositions) : Array.Empty<RangeInt>();
            var config = new PlatformConfig(rotationY, gapPositions, dangerPositions);
            generator.Create(config);
        }

        private RangeInt[] CalculateDangerSections(RangeInt[] gapRanges) {
            RangeInt[] solidSections = GenUtils.GetSolidPlatforms(gapRanges, totalSegments);
            RangeInt dangerPairRange = genSettings.generalSettings.dangerPairRange;
            var danger = new List<RangeInt>();

            int fullDangerSectionCounter = 0;

            for (int i = 0; i < solidSections.Length; i++) {
                RangeInt solidSection = solidSections[i];
                int totalSolids = solidSection.max - solidSection.min;

                if (totalSolids < dangerPairRange.min) { continue; }

                if (PreferFullDangerSection(totalSolids, ref fullDangerSectionCounter)) {
                    danger.Add(solidSection);
                    continue;
                }

                RangeInt[] dangerPairs = DangerGen.CalculateDangerPairs(genSettings.dangerConfig, solidSection, dangerPairRange);
                danger.AddRange(dangerPairs);
            }

            return danger.ToArray();
        }

        private bool ShouldAlignWithPrevious() {
            return RandomUtil.Int(0, 100) < genSettings.generalSettings.alignWithPreviousChance;
        }

        private bool PreferFullDangerSection(int totalSolids, ref int fullDangerSectionCounter) {
            FullDangerSectionSettings fullDangerSettings = genSettings.fullDangerSectionSettings;
            if (fullDangerSectionCounter >= fullDangerSettings.maxFullDangerSections) { return false; }

            if (totalSolids <= fullDangerSettings.maxSolidsForFullDanger && RandomUtil.Int(0, 100) < fullDangerSettings.fullDangerChance) {
                fullDangerSectionCounter++;
                return true;
            }

            return false;
        }
    }
}