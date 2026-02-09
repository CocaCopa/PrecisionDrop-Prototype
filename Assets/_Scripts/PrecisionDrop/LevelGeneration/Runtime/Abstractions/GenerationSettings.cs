using System;
using CocaCopa.Primitives;

namespace PrecisionDrop.LevelGeneration.Runtime {
    [Serializable]
    internal struct GenerationSettings {
        public int platformSegments;
        public int firstBatchCount;
        public int[] totalGapSections;
        public RangeInt gapRange;

        public GenerationSettings(int platformSegments, int firstBatchCount, int[] totalGapSections, RangeInt gapRange) {
            this.platformSegments = platformSegments;
            this.firstBatchCount = firstBatchCount;
            this.totalGapSections = totalGapSections;
            this.gapRange = gapRange;
        }
    }
}
