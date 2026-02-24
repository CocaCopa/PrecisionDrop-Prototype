using System;
using System.Collections.Generic;
using CocaCopa.Core.Randomization;
using CocaCopa.Primitives;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.Platforms.Contracts;

namespace PrecisionDrop.LevelGeneration.Runtime {
    internal sealed class LevelGeneratorFlow : IDisposable {
        private readonly GenerationSettings genSettings;
        private readonly IPlatformBuilder generator;
        private readonly IGameFlow gameFlow;
        private readonly int totalSegments;

        private int FirstBatchCount => genSettings.firstBatchCount;

        private static bool AlignWithPrevious => RandomUtil.Int(0, 100) < 25;

        private int totalPassCount;
        private float rotationY;

        internal LevelGeneratorFlow(GenerationSettings genSettings, IGameFlow gameFlow, IPlatformBuilder generator) {
            this.totalSegments = generator.PlatformSegments;
            this.gameFlow = gameFlow;
            this.generator = generator;
            this.genSettings = genSettings;
        }

        public void Dispose() {
            gameFlow.OnPlayerPassedPlatform -= GameFlow_OnPlayerPassedPlatform;
        }

        internal void Initialize() {
            gameFlow.OnPlayerPassedPlatform += GameFlow_OnPlayerPassedPlatform;
            CreateFirstBatch();
        }

        private void CreateFirstBatch() {
            for (int i = 0; i < FirstBatchCount; i++) {
                rotationY = RandomUtil.Float(-50f, 50f);
                var gapConfig = genSettings.gapConfigs[0];
                GeneratePlatform(gapConfig);
            }
        }

        private void GameFlow_OnPlayerPassedPlatform() {
            totalPassCount++;

            if (AlignWithPrevious) { rotationY += RandomUtil.Float(-10f, 10f); }
            else { rotationY = RandomUtil.Float(20f, 340f); }

            var gapConfig = GenUtils.GetRandomGapConfig(genSettings.gapConfigs);
            GeneratePlatform(gapConfig);
        }

        private void GeneratePlatform(GapConfig gapConfig) {
            RangeInt[] gapPositions = GapGen.BuildGapRanges(
                totalSegments,
                gapConfig.totalGaps,
                GenUtils.ToBaseRange(gapConfig.gapRange)
            );

            RangeInt[] dangerPositions = CalculateDangerSections(gapPositions);

            var config = new PlatformConfig(rotationY, gapPositions, dangerPositions);
            generator.Create(config);
        }

        private RangeInt[] CalculateDangerSections(RangeInt[] gapRanges) {
            RangeInt dangerPairRange = new RangeInt(1, 2);

            var solidSections = GenUtils.GetSolidPlatforms(gapRanges, totalSegments);
            var danger = new List<RangeInt>();

            int fullDangerSectionCounter = 0;

            for (int i = 0; i < solidSections.Length; i++) {
                var solidSection = solidSections[i];
                var totalSolids = solidSection.max - solidSection.min;

                if (totalSolids < dangerPairRange.min) { continue; }

                if (PreferFullDangerSection(totalSolids, ref fullDangerSectionCounter)) {
                    danger.Add(solidSection);
                    continue;
                }

                var dangerPairs = DangerGen.CalculateDangerPairs(solidSection, dangerPairRange);
                danger.AddRange(dangerPairs);
            }

            return danger.ToArray();
        }

        private static bool PreferFullDangerSection(int totalSolids, ref int fullDangerSectionCounter) {
            if (fullDangerSectionCounter >= 2) { return false; }

            if (totalSolids <= 5 && RandomUtil.Int(0, 100) < 30) {
                fullDangerSectionCounter++;
                return true;
            }

            return false;
        }
    }
}