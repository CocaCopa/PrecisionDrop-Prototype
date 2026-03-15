using System;
using CocaCopa.Primitives;

namespace PrecisionDrop.LevelGeneration.Runtime {
    /// <summary>
    ///     Centralized documentation strings for FirstBatchConfig.
    ///     Controls how the initial set of platforms is generated before the normal runtime flow begins.
    /// </summary>
    public static class FirstBatchConfigDocs {
        public const string FirstBatchConfig =
            "Configuration controlling the generation of the initial batch of platforms.\n\n" +
            "These platforms are created immediately when the level starts and establish the\n" +
            "initial playfield before dynamic generation begins.";

        public const string BatchCount =
            "Number of platforms generated during the initial batch.\n" +
            "These platforms are created before the player begins progressing normally.\n\n" +
            "Higher values extend the starting section of the level.";

        public const string GapConfigIndex =
            "Index of the GapConfig used for all platforms in the first batch.\n" +
            "This ensures the starting section follows a predictable gap layout.\n\n" +
            "Example: using a simpler gap configuration can create an easier opening.";

        public const string AlignmentOffset =
            "Rotation offset range applied between platforms in the first batch.\n" +
            "Each platform rotates relative to the previous one within this range.\n\n" +
            "Example: small values create consistent alignment, while larger values produce\n" +
            "more variation in the opening layout.";
    }

    [Serializable]
    internal struct FirstBatchConfig {
        public int batchCount;
        public int gapConfigIndex;
        public RangeInt alignmentOffset;

        public FirstBatchConfig(int batchCount, int gapConfigIndex, RangeInt alignmentOffset) {
            this.batchCount = batchCount;
            this.gapConfigIndex = gapConfigIndex;
            this.alignmentOffset = alignmentOffset;
        }
    }
}