using System;
using CocaCopa.Primitives;

namespace PrecisionDrop.LevelGeneration.Runtime {
    /// <summary>
    ///     Centralized documentation strings for GeneralSettings.
    ///     Controls general rotation behavior when generating new platforms.
    /// </summary>
    public static class GeneralSettingsDocs {
        public const string GeneralSettings =
            "Configuration controlling how newly generated platforms align relative to previous ones.\n\n" +
            "These settings influence the overall rotational flow of the level and how predictable\n" +
            "platform alignment feels during gameplay.";

        public const string DangerPairRange =
            "The number of danger pieces generated in a danger section. The value is randomly selected between the specified minimum and maximum.";

        public const string AlignWithPreviousChance =
            "Chance that the next platform will align relative to the previous platform's rotation.\n\n" +
            "Higher values produce smoother rotational flow, while lower values increase randomness\n" +
            "by allowing more independent rotations.";

        public const string AlignmentOffset =
            "Rotation offset range applied when determining the next platform's orientation.\n\n" +
            "If alignment with the previous platform is chosen, the offset is applied relative to\n" +
            "the previous rotation. Otherwise a new rotation is chosen within the allowed range.";
    }

    [Serializable]
    internal struct GeneralSettings {
        public RangeInt dangerPairRange;
        public int alignWithPreviousChance;
        public RangeInt alignmentOffset;
    }
}