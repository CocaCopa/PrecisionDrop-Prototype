using System.Collections.Generic;
using CocaCopa.Core;
using CocaCopa.Core.Randomization;
using CocaCopa.Logger.API;
using CocaCopa.Primitives;

namespace PrecisionDrop.LevelGeneration.Runtime {
    internal static class DangerGen {
        internal static List<RangeInt> CalculateDangerPairs(RangeInt solidSection, RangeInt dangerPairRange) {
            var dangerSections = new List<RangeInt>();
            var totalSolids = solidSection.max - solidSection.min;

            int maxPairs = (int)(totalSolids * 0.15f);
            maxPairs = MathUtils.Max(2, maxPairs);
            int totalPairs = RandomUtil.Int(2, maxPairs);
            totalPairs = 3;

            int totalDangerPieces = GetTotalDangerPieces(totalPairs, dangerPairRange, out int[] dangerPairs);

            int safePieces = totalSolids - totalDangerPieces;
            if (safePieces < 0) { return dangerSections; }

            int gap = DangerSectionsGap(safePieces, totalPairs, out int gapRemainder);
            int removedCount = 0;
            int cursorIndex = solidSection.min;

            for (int i = totalPairs - 1; i >= 0; i--) {
                int finalGap = gap + (i < gapRemainder ? 1 : 0);
                int shrunkGap = finalGap - (int)(finalGap * RandomUtil.Float(0f, 0.75f));
                if (i != totalPairs - 1) { removedCount += finalGap - shrunkGap; }

                int sectionStart = cursorIndex;
                int sectionEnd = sectionStart + dangerPairs[i];

                cursorIndex = sectionEnd + shrunkGap;
                dangerSections.Add(new RangeInt(sectionStart, sectionEnd));
            }

            int push = (int)(removedCount * RandomUtil.Float(0f, 1f));
            for (int i = 0; i < dangerSections.Count; i++) {
                var section = dangerSections[i];
                section.min += push;
                section.max += push;
                dangerSections[i] = section;
            }

            return dangerSections;
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

        private static int DangerSectionsGap(int totalSafePieces, int dangerPairsCount, out int remainder) {
            int baseGap = totalSafePieces / dangerPairsCount;

            // Don't count the gap after the last danger section
            int gapCount = dangerPairsCount - 1;

            // This calculates the number of safe pieces after the last danger section
            int remaining = totalSafePieces % dangerPairsCount;
            int lastGap = baseGap + remaining;

            int extraGap = lastGap / gapCount;
            remainder = lastGap % gapCount;

            return baseGap + extraGap;
        }
    }
}