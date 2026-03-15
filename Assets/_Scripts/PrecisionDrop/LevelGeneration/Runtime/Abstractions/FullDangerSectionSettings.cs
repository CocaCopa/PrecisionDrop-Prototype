using System;

namespace PrecisionDrop.LevelGeneration.Runtime {
    /// <summary>
    ///     Centralized documentation strings for FullDangerSectionSettings.
    ///     These can be reused in tooltips, inspectors, editors, or external docs.
    /// </summary>
    public static class FullDangerSectionDocs {
        public const string FullDangerSectionSettings =
            "Configuration controlling when an entire solid platform section becomes fully dangerous.\n\n" +
            "Instead of generating partial danger pairs, the whole section may be marked as danger.\n" +
            "This introduces occasional high-risk segments and increases gameplay tension.\n\n" +
            "Each field controls a different constraint of this behavior:\n" +
            "• MaxFullDangerSections: limits how many full-danger sections may appear.\n" +
            "• MaxSolidsForFullDanger: restricts this behavior to smaller platform sections.\n" +
            "• FullDangerChance: defines the probability of converting a section to full danger.";

        public const string MaxFullDangerSections =
            "Maximum number of solid sections that can become fully dangerous within a single platform.\n" +
            "This prevents excessive difficulty spikes caused by too many full-danger areas.";

        public const string MaxSolidsForFullDanger =
            "Maximum number of solid pieces allowed for a section to qualify as a full-danger candidate.\n" +
            "Larger sections will instead generate partial danger pairs.";

        public const string FullDangerChance =
            "Probability that a qualifying solid section will become a full-danger section.\n\n" +
            "Example: 30% means that roughly one out of three eligible sections will be converted into full danger.";
    }

    [Serializable]
    internal struct FullDangerSectionSettings {
        public int maxFullDangerSections;
        public int maxSolidsForFullDanger;
        public int fullDangerChance;

        public FullDangerSectionSettings(int maxFullDangerSections, int maxSolidsForFullDanger, int fullDangerChance) {
            this.maxFullDangerSections = maxFullDangerSections;
            this.maxSolidsForFullDanger = maxSolidsForFullDanger;
            this.fullDangerChance = fullDangerChance;
        }
    }
}