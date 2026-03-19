using System;

namespace CocaCopa.ObjectPooling.Unity.Config {
    internal static class PoolGroupDocs {
        internal const string GroupId =
            "Unique identifier for this pool group. Used together with an entry id to locate a specific pool.";

        internal const string PrewarmGroup =
            "If enabled, the group controls prewarming for all entries inside it. Entry-level prewarm settings are overridden and each pool is automatically prewarmed using the group percentage.";

        internal static string PrewarmPercentage =
            $"Percentage of each entry's {nameof(PoolEntry.maxPoolCount)} to prewarm when group prewarm is enabled. For example, 50 means each pool is warmed with half of its maximum size.";

        internal const string Entries =
            "The pool entries that belong to this group. Each entry defines one pooled prefab and its individual pool settings.";
    }

    [Serializable]
    public struct PoolGroup {
        public string groupId;
        public bool prewarmGroup;
        public int prewarmPercentage;
        public PoolEntry[] entries;
    }
}