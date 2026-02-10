using System;
using CocaCopa.Primitives;

namespace PrecisionDrop.LevelGeneration.Runtime {
    [Serializable]
    internal struct GenerationSettings {
        public int platformSegments;
        public int firstBatchCount;
        public GapConfig[] gapConfigs;
        public RangeInt gapRange;

        public GenerationSettings(int platformSegments, int firstBatchCount, GapConfig[] gapConfigs, RangeInt gapRange) {
            this.platformSegments = platformSegments;
            this.firstBatchCount = firstBatchCount;
            this.gapConfigs = gapConfigs;
            this.gapRange = gapRange;
        }
    }
}
