using System;

namespace PrecisionDrop.LevelGeneration.Runtime {
    [Serializable]
    internal struct GenerationSettings {
        public int firstBatchCount;
        public GapConfig[] gapConfigs;
        public DangerConfig dangerConfig;

        public GenerationSettings(int firstBatchCount, GapConfig[] gapConfigs, DangerConfig dangerConfig) {
            this.firstBatchCount = firstBatchCount;
            this.gapConfigs = gapConfigs;
            this.dangerConfig = dangerConfig;
        }
    }
}
