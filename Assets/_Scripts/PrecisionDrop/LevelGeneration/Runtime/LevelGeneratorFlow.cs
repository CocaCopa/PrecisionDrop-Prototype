using System;
using System.Collections.Generic;
using System.Threading;
using CocaCopa.Core.Randomization;
using CocaCopa.Logger.API;
using CocaCopa.Primitives;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.Platforms.Contracts;

namespace PrecisionDrop.LevelGeneration.Runtime {
    internal sealed class LevelGeneratorFlow : IDisposable {
        private readonly GenerationSettings genSettings;
        private readonly IPlatformBuilder generator;
        private readonly IGameFlow gameFlow;

        private int FirstBatchCount => genSettings.firstBatchCount;

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
                var gapConfig = genSettings.gapConfigs[0];
                GeneratePlatform(gapConfig);
            }
        }

        private void GameFlow_OnPlayerPassedPlatform() {
            if (AlignWithPrevious) { rotationY += RandomUtil.Float(-10f, 10f); }
            else { rotationY = RandomUtil.Float(20f, 340f); }

            var gapConfig = GenerationHelpers.GetRandomGapConfig(genSettings.gapConfigs);
            GeneratePlatform(gapConfig);
        }

        private void GeneratePlatform(GapConfig gapConfig) {
            RangeInt[] gapRanges = GenerationHelpers.BuildGapRanges(
                36,
                gapConfig.totalGaps,
                GenerationHelpers.ToBaseRange(gapConfig.gapRange)
            );

            var config = new PlatformConfig(rotationY, gapRanges, new RangeInt(25, 30));
            generator.Create(config);
        }
    }
}