using System;
using System.Collections.Generic;
using CocaCopa.Core;
using CocaCopa.Core.Randomization;
using CocaCopa.Primitives;

namespace PrecisionDrop.LevelGeneration.Runtime {
    internal static class DangerGen {
        /// <summary>
        ///     Returns a random float between 0 and 1
        /// </summary>
        private static float Rng01 => RandomUtil.Float(0f, 1f);

        /// <summary>
        ///     Generates a set of danger sections within a given solid platform range.<br />
        ///     <br />
        ///     The algorithm:<br />
        ///     - Randomly determines how many danger pairs will be created.<br />
        ///     - Randomly determines each pair's length within the provided range.<br />
        ///     - Evenly distributes safe gaps between pairs.<br />
        ///     - Randomly shrinks gaps to introduce variation.<br />
        ///     - Pushes all sections by a random offset based on removed gap space.<br />
        ///     - Occasionally snaps the first or last section to the platform edges.<br />
        ///     <br />
        ///     Ensures all generated danger sections remain within the bounds of the solid section.
        /// </summary>
        internal static RangeInt[] CalculateDangerPairs(DangerConfig dangerConfig, RangeInt solidSection, RangeInt dangerPairRange) {
            PlatformLayout platformLayout = GetPlatformLayout(dangerConfig.pairCount, solidSection, dangerPairRange);

            RangeInt[] dangerSections = GenerateUniformDangerSections(platformLayout);
            dangerSections = VaryDangerSectionGaps(dangerConfig.gapVariation, dangerSections, platformLayout, out int removedCount);

            OffsetConfig offsetConfig = dangerConfig.offset;
            int offset = (int)(removedCount * RandomUtil.Float(offsetConfig.offsetUtilizationRatio.min, offsetConfig.offsetUtilizationRatio.max));
            dangerSections = OffsetDangerSections(offset, dangerSections);
            if (Rng01 < 0.25f) {
                dangerSections[0] = SnapDangerToLeftEdge(dangerSections[0], solidSection);
                dangerSections[^1] = SnapDangerToRightEdge(dangerSections[^1], solidSection);
            }

            return dangerSections;
        }

        private static PlatformLayout GetPlatformLayout(PairCountConfig pairConfig, RangeInt solidSection, RangeInt dangerPairRange) {
            int totalSolids = solidSection.max - solidSection.min;

            int maxPairs = (int)(totalSolids * pairConfig.maxPairDensityRatio);
            maxPairs = MathUtils.Max(pairConfig.minDangerPairCount, maxPairs);
            int totalPairs = RandomUtil.Int(pairConfig.minDangerPairCount, maxPairs);
            if (totalPairs == 0) { return new PlatformLayout(totalSolids, totalSolids, Array.Empty<int>(), solidSection.min); }

            int totalDangerPieces = GetTotalDangerPieces(totalPairs, dangerPairRange, out int[] dangerPairs);
            int safePieces = totalSolids - totalDangerPieces;

            return new PlatformLayout(
                totalSolids,
                safePieces,
                dangerPairs,
                solidSection.min
            );
        }

        private static int GetTotalDangerPieces(int totalPairs, RangeInt pairRange, out int[] lengths) {
            int totalDangerPieces = 0;
            lengths = new int[totalPairs];
            for (int i = 0; i < totalPairs; i++) {
                int pairCount = RandomUtil.Int(pairRange.min, pairRange.max);
                totalDangerPieces += pairCount;
                lengths[i] = pairCount;
            }

            return totalDangerPieces;
        }

        private static RangeInt[] GenerateUniformDangerSections(PlatformLayout layout) {
            int cursorIndex = layout.StartIndex;
            int totalPairs = layout.DangerPairs.Length;

            var dangerSections = new List<RangeInt>();

            if (layout.SafePieces < 0 || totalPairs == 0) { return dangerSections.ToArray(); }

            int gap = DangerSectionsGap(layout.SafePieces, totalPairs, out int gapRemainder);

            for (int i = 0; i < totalPairs; i++) {
                int finalGap = gap + (i < gapRemainder ? 1 : 0);
                int sectionStart = cursorIndex;
                int sectionEnd = sectionStart + layout.DangerPairs[i];

                cursorIndex = sectionEnd + finalGap;
                dangerSections.Add(new RangeInt(sectionStart, sectionEnd));
            }

            return dangerSections.ToArray();
        }

        private static int DangerSectionsGap(int totalSafePieces, int dangerPairsCount, out int remainder) {
            int baseGap = totalSafePieces / dangerPairsCount;

            // Don't count the gap after the last danger section
            // int gapCount = MathUtils.Max(1, dangerPairsCount - 1);
            int gapCount = dangerPairsCount - 1;

            // This calculates the number of safe pieces after the last danger section
            int remaining = totalSafePieces % dangerPairsCount;
            int lastGap = baseGap + remaining;

            int extraGap = gapCount > 0 ? lastGap / gapCount : 0;
            remainder = gapCount > 0 ? lastGap % gapCount : 0;

            return baseGap + extraGap;
        }

        private static RangeInt[] VaryDangerSectionGaps(GapVariationConfig dangerGapConfig, RangeInt[] dangerSections, PlatformLayout layout, out int removedCount) {
            removedCount = 0;
            int totalPairs = layout.DangerPairs.Length;
            if (totalPairs == 0) { return dangerSections; }
            int cursorIndex = layout.StartIndex;
            var trimmedSections = new RangeInt[totalPairs];

            for (int i = 0; i < totalPairs; i++) {
                RangeInt section = dangerSections[i];
                int gap = GetPlatformGap(i);
                int shrunkGap = gap - (int)(gap * RandomUtil.Float(0f, dangerGapConfig.maxGapShrinkRatio));
                if (i != totalPairs - 1 || totalPairs == 1) { removedCount += gap - shrunkGap; }
                section.min = cursorIndex;
                section.max = section.min + layout.DangerPairs[i];

                cursorIndex = section.max + shrunkGap;
                trimmedSections[i] = section;
            }

            return trimmedSections;

            int GetPlatformGap(int platformIndex) {
                if (totalPairs == 1) { return layout.TotalPieces - layout.DangerPairs[0]; }

                return platformIndex + 1 < totalPairs
                    ? dangerSections[platformIndex + 1].min - dangerSections[platformIndex].max
                    : 0;
            }
        }

        private static RangeInt[] OffsetDangerSections(int offset, RangeInt[] dangerSections) {
            var danger = new RangeInt[dangerSections.Length];
            for (int i = 0; i < danger.Length; i++) {
                RangeInt section = dangerSections[i];
                section.min += offset;
                section.max += offset;

                danger[i] = section;
            }
            return danger;
        }

        private static RangeInt SnapDangerToLeftEdge(RangeInt sectionToSnap, RangeInt safeSection) {
            int dangerRange = sectionToSnap.max - sectionToSnap.min;
            sectionToSnap.min = safeSection.min;
            sectionToSnap.max = sectionToSnap.min + dangerRange;
            return sectionToSnap;
        }

        private static RangeInt SnapDangerToRightEdge(RangeInt sectionToSnap, RangeInt safeSection) {
            int dangerRange = sectionToSnap.max - sectionToSnap.min;
            sectionToSnap.max = safeSection.max;
            sectionToSnap.min = sectionToSnap.max - dangerRange;
            return sectionToSnap;
        }

        private readonly struct PlatformLayout {
            public int TotalPieces { get; }
            public int SafePieces { get; }
            public int[] DangerPairs { get; }
            public int StartIndex { get; }

            public override string ToString() {
                string pairStr = "";
                for (int i = 0; i < DangerPairs.Length; i++) {
                    int pair = DangerPairs[i];
                    if (i == 0) { pairStr += "{ "; }
                    pairStr += $"{pair}";
                    if (i < DangerPairs.Length - 1) { pairStr += ", "; }
                    if (i == DangerPairs.Length - 1) { pairStr += " }"; }
                }
                return $"Total: {TotalPieces} | " +
                       $"Safe: {SafePieces} | " +
                       $"Pairs: {DangerPairs.Length} {pairStr} | " +
                       $"Start: {StartIndex}";
            }

            public PlatformLayout(int totalPieces, int safePieces, int[] dangerPairs, int startIndex) {
                TotalPieces = totalPieces;
                SafePieces = safePieces;
                DangerPairs = (int[])dangerPairs.Clone();
                StartIndex = startIndex;
            }
        }
    }
}