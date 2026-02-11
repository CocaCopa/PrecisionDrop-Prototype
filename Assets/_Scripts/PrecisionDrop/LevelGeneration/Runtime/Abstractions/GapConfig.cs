using System;
using CocaCopa.Primitives;

namespace PrecisionDrop.LevelGeneration.Runtime {
    [Serializable]
    public struct GapConfig {
        public int totalGaps;
        public RangeInt gapRange;
        public float chance;
    }
}