using System;
using System.Collections.Generic;
using CocaCopa.Core;
using CocaCopa.Core.Randomization;
using CocaCopa.Primitives;
using CocaCopa.Logger.API;
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

            var gapConfig = GenerationHelpers.GetRandomGapConfig(genSettings.gapConfigs);
            GeneratePlatform(gapConfig);
        }

        private void GeneratePlatform(GapConfig gapConfig) {
            RangeInt[] gapRanges = GenerationHelpers.BuildGapRanges(
                totalSegments,
                gapConfig.totalGaps,
                GenerationHelpers.ToBaseRange(gapConfig.gapRange)
            );

            var dangerSections = CalculateDangerSections(gapRanges);

            var config = new PlatformConfig(rotationY, gapRanges, dangerSections);
            generator.Create(config);
        }

        private RangeInt[] CalculateDangerSections(RangeInt[] gapRanges) {
            const int splitThreshold = 15;
            RangeInt dangerPairRange = new RangeInt(1, 2);

            var solidSections = GenerationHelpers.GetSolidPlatforms(gapRanges, totalSegments);
            var danger = new List<RangeInt>();

            int fullDangerSectionCount = 0;

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (int i = 0; i < solidSections.Length; i++) {
                var solidSection = solidSections[i];
                var totalSolids = solidSection.max - solidSection.min;

                if (totalSolids < dangerPairRange.min) { continue; }

                // Full section is consisted of danger pieces.
                if (totalSolids <= 5 && fullDangerSectionCount < 2) {
                    if (RandomUtil.Int(0, 100) < 30) {
                        fullDangerSectionCount++;
                        danger.Add(new RangeInt(solidSection.min, solidSection.max));
                        continue;
                    }
                }
                
                int maxPairs = (int)(totalSolids * 0.25f);
                maxPairs = MathUtils.Max(2, maxPairs);
                int totalPairs = RandomUtil.Int(2, maxPairs);

                int totalDangerPieces = totalPairs * dangerPairRange.max;
                int safePieces = totalSolids - totalDangerPieces;
                int baseGap = safePieces / totalPairs;
                
                // Don't count the gap after the last danger section
                int gapCount = totalPairs - 1;

                // This calculates the number of safe pieces after the last danger section
                int remaining = safePieces % totalPairs;
                int lastGap = baseGap + remaining;
                
                int extraGap = lastGap / gapCount;
                int extraGapRemainder = lastGap % gapCount;

                int cursorIndex = solidSection.min;
                for (int j = 0; j < totalPairs; j++) {
                    int sectionStart = cursorIndex;
                    int sectionEnd = sectionStart + dangerPairRange.max;

                    cursorIndex = sectionEnd + baseGap + extraGap + (j < extraGapRemainder ? 1 : 0);
                    danger.Add(new RangeInt(sectionStart, sectionEnd));
                }
            }

            return danger.ToArray();
        }
    }
}