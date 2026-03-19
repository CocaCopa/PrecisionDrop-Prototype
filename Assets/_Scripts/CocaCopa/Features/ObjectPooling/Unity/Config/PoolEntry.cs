using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CocaCopa.ObjectPooling.Unity.Config {
    internal static class PoolEntryDocs {
        internal const string Id =
            "Unique identifier for this pooled object inside its group. Used together with the group id when renting or prewarming a pool.";

        internal const string Prefab =
            "The prefab that will be instantiated and managed by this pool.";

        internal const string MaxPoolCount =
            "Maximum number of inactive objects this pool is allowed to keep. Extra returned objects beyond this limit are not retained by the pool.";

        internal const string Prewarm =
            "Controls whether this pool is prewarmed automatically during initialization or only when manually triggered. Ignored if the parent group uses group prewarm.";

        internal const string PrewarmCount =
            "Number of objects to create upfront when this pool is manually or automatically prewarmed at the entry level.";
    }

    [Serializable]
    public struct PoolEntry {
        [FormerlySerializedAs("key")] public string id;
        public GameObject prefab;
        public int maxPoolCount;
        public PrewarmMode prewarm;
        public int prewarmCount;
    }
}