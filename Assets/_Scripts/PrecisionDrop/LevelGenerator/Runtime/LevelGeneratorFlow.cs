using CocaCopa.Core.Randomization;
using CocaCopa.Primitives;
using PrecisionDrop.Platforms.Contracts;

namespace PrecisionDrop.LevelManagment.Runtime {
    internal sealed class LevelGeneratorFlow {
        private readonly IPlatformBuilder generator;

        private int startCount;
        private int startAligned;

        internal LevelGeneratorFlow(IPlatformBuilder generator) {
            this.generator = generator;
        }
        float test;
        internal void Initialize() {
            startCount = 10;
            startAligned = 10;

            for (int i = 0; i < startCount; i++) {
                if (i == 0) { test = 0f; }
                else if (i < startAligned) { test = 0f + RandomUtil.Float(-25f, 25f); }
                var config = new PlatformConfig(test, new RangeInt(0, 4), new RangeInt(0, 0), false);
                generator.Create(config);
            }
        }
    }
}
