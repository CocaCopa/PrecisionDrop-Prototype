using System;
using System.Threading;
using CocaCopa.Core.Randomization;
using CocaCopa.Primitives;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.Platforms.Contracts;

namespace PrecisionDrop.LevelGeneration.Runtime {
    internal sealed class LevelGeneratorFlow : IDisposable {
        private readonly GenerationSettings genSettings;
        private readonly IPlatformBuilder generator;
        private readonly IGameFlow gameFlow;

        private int FirstBatchCount => genSettings.firstBatchCount;

        private RangeInt GapPositions =>
            new RangeInt(0, RandomUtil.Int(genSettings.gapRange.min, genSettings.gapRange.max));

        private int GetRandomGapConfig() {
            float totalChance = 0f;
            for (int i = 0; i < genSettings.gapConfigs.Length; i++) { totalChance += genSettings.gapConfigs[i].chance; }

            if (totalChance < 100) {
                throw new Exception(
                    $"[{nameof(LevelGeneratorFlow)}] {nameof(GapConfig)} chances sum to {totalChance}%, but must be 100%."
                );
            }

            float roll = RandomUtil.Float(0f, totalChance);
            float cumulative = 0f;

            for (int i = 0; i < genSettings.gapConfigs.Length; i++) {
                cumulative += genSettings.gapConfigs[i].chance;
                if (roll <= cumulative) { return genSettings.gapConfigs[i].total; }
            }

            return genSettings.gapConfigs[^1].total;
        }

        private static bool AlignWithPrevious => RandomUtil.Int(0, 100) < 25;

        private int totalPassCount;
        private float rotationY;

        internal LevelGeneratorFlow(GenerationSettings genSettings, IGameFlow gameFlow, IPlatformBuilder generator) {
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
                RangeInt[] gapRanges = BuildGapRanges(36, 1, GapPositions);
                var config = new PlatformConfig(rotationY, gapRanges, new RangeInt(25, 30));
                generator.Create(config);
            }
        }

        private void GameFlow_OnPlayerPassedPlatform() {
            if (AlignWithPrevious) { rotationY += RandomUtil.Float(-10f, 10f); }
            else { rotationY = RandomUtil.Float(20f, 340f); }

            RangeInt[] gapRanges = BuildGapRanges(36, GetRandomGapConfig(), GapPositions);
            var config = new PlatformConfig(rotationY, gapRanges, new RangeInt(25, 30));
            generator.Create(config);
        }

        /// <summary>
        /// Repeats the defined <see cref="GapPositions"/> across the platform.
        /// </summary>
        /// <returns>The computed gap ranges</returns>
        private RangeInt[] BuildGapRanges(int segments, int totalGapSections, RangeInt baseGapRange) {
            int partSize = segments / totalGapSections;

            var ranges = new RangeInt[totalGapSections];

            for (int partIndex = 0; partIndex < totalGapSections; partIndex++) {
                int offset = partIndex * partSize;

                ranges[partIndex] = new RangeInt(
                    baseGapRange.min + offset,
                    baseGapRange.max + offset
                );
            }

            return ranges;
        }
    }
}