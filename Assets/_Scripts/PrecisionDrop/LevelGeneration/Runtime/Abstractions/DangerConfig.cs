using System;
using CocaCopa.Primitives;

namespace PrecisionDrop.LevelGeneration.Runtime {
    /// <summary>
    ///     Centralized documentation strings for DangerConfig and related sub-configs.
    ///     These can be reused in tooltips, inspectors, editors, or external docs.
    /// </summary>
    public static class DangerConfigDocs {
        public const string DangerConfig =
            "Configuration controlling how danger sections are generated within a solid platform.\n\n" +
            "Each sub-config maps directly to a specific phase of the generation algorithm:\n" +
            "• PairCount: determines how many danger pairs exist.\n" +
            "• GapVariation: controls how uniformly or unevenly danger sections are spaced.\n" +
            "• Offset: controls how much the final layout can shift within available free space and whether it can snap to platform edges.";

        public const string PairCountConfig =
            "Controls how many danger pairs are generated relative to platform size.\n" +
            "This directly affects overall difficulty density.";

        public const string PairCount_MaxPairDensityRatio =
            "Maximum number of danger pairs allowed, expressed as a percentage of the total platform piece count.\n" +
            "Higher values increase danger density.\n\n" +
            "Example: 12% means the maximum pair count is 12% of the total platform piece count.";

        public const string PairCount_MinDangerPairCount =
            "Minimum number of danger pairs guaranteed regardless of platform size.\n" +
            "Ensures very small platforms still contain meaningful danger.";

        public const string GapVariationConfig =
            "Controls how much the safe gaps between danger sections can be reduced.\n" +
            "This affects spacing consistency and layout unpredictability.";

        public const string GapVariation_MaxGapShrinkRatio =
            "Maximum percentage a safe gap can shrink.\n" +
            "Higher values produce more uneven and chaotic layouts.\n\n" +
            "Example: 75% means a gap may shrink by up to 75% of its original size.";

        public const string OffsetConfig =
            "Controls how much the entire danger layout can shift after gap shrinking.\n" +
            "This uses the free space created by gap reduction and can also snap outer danger sections to platform edges.";

        public const string Offset_OffsetUtilizationRatio =
            "Percentage of removed gap space that can be used to offset the layout.\n" +
            "Higher values increase layout drift and unpredictability.\n\n" +
            "Example: 0% keeps the layout fixed, while 100% allows full offset.";

        public const string Offset_EdgeSnapChance =
            "Probability that the outermost danger sections will snap to both edges of the solid section.\n" +
            "This creates more extreme layouts by forcing danger to appear directly at platform boundaries.\n\n" +
            "Example: 25% means roughly one in four layouts will snap the first and last danger sections to the platform edges.";
    }

    /// <summary>
    ///     Configuration controlling how danger sections are generated within a solid platform.<br /><br />
    ///     Each sub-config maps directly to a specific phase of the generation algorithm:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 PairCount: determines how many danger pairs exist.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 GapVariation: controls how uniformly or unevenly danger sections are spaced.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Offset: controls how much the final layout can shift within available free space and whether it can snap to platform edges.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
    [Serializable]
    public struct DangerConfig {
        /// <summary>
        ///     Controls how many danger pairs are generated relative to platform size.
        /// </summary>
        public PairCountConfig pair;

        /// <summary>
        ///     Controls how safe gaps between danger sections can shrink.
        /// </summary>
        public GapVariationConfig gapVariation;

        /// <summary>
        ///     Controls how the final danger layout shifts after gap shrinking and whether it can snap to platform edges.
        /// </summary>
        public OffsetConfig offset;
    }

    /// <summary>
    ///     Controls how many danger pairs are generated relative to platform size.<br />
    ///     This directly affects overall difficulty density.
    /// </summary>
    [Serializable]
    public struct PairCountConfig {
        /// <summary>
        ///     Maximum number of danger pairs allowed, expressed as a percentage of the total platform piece count.<br />
        ///     Higher values increase danger density.
        /// </summary>
        public int densityRatio;

        /// <summary>
        ///     Minimum number of danger pairs guaranteed regardless of platform size.<br />
        ///     Ensures very small platforms still contain meaningful danger.
        /// </summary>
        public int minPairCount;
    }

    /// <summary>
    ///     Controls how much the safe gaps between danger sections can be reduced.<br />
    ///     This affects spacing consistency and layout unpredictability.
    /// </summary>
    [Serializable]
    public struct GapVariationConfig {
        /// <summary>
        ///     Maximum percentage a safe gap can shrink.<br />
        ///     Higher values produce more uneven and chaotic layouts.
        /// </summary>
        public int shrinkRatio;
    }

    /// <summary>
    ///     Controls how much the entire danger layout can shift after gap shrinking.<br />
    ///     This uses the free space created by gap reduction and can also snap outer danger sections to platform edges.
    /// </summary>
    [Serializable]
    public struct OffsetConfig {
        /// <summary>
        ///     Percentage of removed gap space that can be used to offset the layout.<br />
        ///     Higher values increase layout drift and unpredictability.
        /// </summary>
        public RangeInt ratio;

        /// <summary>
        ///     Probability that the outermost danger sections will snap to both edges of the platform.<br />
        ///     This occasionally forces danger to appear directly at the platform boundaries,
        ///     producing more extreme or aggressive layouts.
        /// </summary>
        public int edgeSnapChance;
    }
}