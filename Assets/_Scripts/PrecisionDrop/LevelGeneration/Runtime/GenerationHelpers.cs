using System;
using System.Collections.Generic;
using CocaCopa.Core.Randomization;
using CocaCopa.Primitives;

namespace PrecisionDrop.LevelGeneration.Runtime {
    internal static class GenerationHelpers {
        /// <summary>
        /// Converts the given range into a new range that always starts from zero
        /// and ends at a randomly selected value within the original range.
        /// </summary>
        internal static RangeInt ToBaseRange(RangeInt range) {
            return new RangeInt(0, RandomUtil.Int(range.min, range.max));
        }

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

        /// <summary>
        /// Computes the solid platform sections by inverting the provided gap ranges
        /// within the total segment count.
        /// </summary>
        internal static RangeInt[] GetSolidPlatforms(RangeInt[] gapRanges, int segments = 36) {
            var inverted = new List<RangeInt>();
            int current = 0;

            for (int i = 0; i < gapRanges.Length; i++) {
                var range = gapRanges[i];
                if (current < range.min) { inverted.Add(new RangeInt(current, range.min)); }

                current = range.max;
            }

            if (current < segments) { inverted.Add(new RangeInt(current, segments)); }

            return inverted.ToArray();
        }

        /// <summary>
        /// Selects a gap configuration based on weighted chance distribution.<br/>
        /// The total chance across all configs must equal 100%.
        /// </summary>
        /// <returns>A randomly selected <see cref="GapConfig"/> according to its weight.</returns>
        internal static GapConfig GetRandomGapConfig(GapConfig[] configs) {
            float totalChance = 0f;
            for (int i = 0; i < configs.Length; i++) { totalChance += configs[i].chance; }

            if (totalChance < 100) {
                throw new Exception(
                    $"[{nameof(LevelGeneratorFlow)}] {nameof(GapConfig)} chances sum to {totalChance}%, but must be 100%."
                );
            }

            float roll = RandomUtil.Float(0f, totalChance);
            float cumulative = 0f;

            for (int i = 0; i < configs.Length; i++) {
                cumulative += configs[i].chance;
                if (roll <= cumulative) { return configs[i]; }
            }

            return configs[^1];
        }
    }
}