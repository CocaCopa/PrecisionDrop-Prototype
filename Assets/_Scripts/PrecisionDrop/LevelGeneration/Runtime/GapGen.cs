using System;
using System.Collections.Generic;
using CocaCopa.Core;
using CocaCopa.Core.Randomization;
using CocaCopa.Primitives;

namespace PrecisionDrop.LevelGeneration.Runtime {
    internal static class GapGen {
        /// <summary>
        /// Repeats the given gap range across the generated platform.
        /// </summary>
        /// <returns>The computed gap ranges</returns>
        internal static RangeInt[] BuildGapRanges(int segments, int gaps, RangeInt range) {
            int partSize = segments / gaps;

            var ranges = new RangeInt[gaps];

            for (int partIndex = 0; partIndex < gaps; partIndex++) {
                int offset = partIndex * partSize;

                ranges[partIndex] = new RangeInt(range.min + offset, range.max + offset);
            }

            return ranges;
        }
    }
}