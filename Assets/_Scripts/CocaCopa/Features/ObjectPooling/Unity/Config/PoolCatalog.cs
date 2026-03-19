using System;
using System.Collections.Generic;
using UnityEngine;

namespace CocaCopa.ObjectPooling.Unity.Config {
    internal sealed class PoolCatalog : ScriptableObject {
        [SerializeField] private PoolGroup[] groups = Array.Empty<PoolGroup>();

        public IReadOnlyList<PoolGroup> Groups => groups;

#if UNITY_EDITOR
        private void OnValidate() {
            try { ValidateOrThrow(groups); }
            catch (Exception e) { Debug.LogError(e.Message, this); }
        }
#endif

        private static void ValidateOrThrow(PoolGroup[] groups) {
            var outer = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

            if (groups == null) { return; }

            for (int gi = 0; gi < groups.Length; gi++) {
                PoolGroup g = groups[gi];
                g.entries ??= Array.Empty<PoolEntry>();

                if (string.IsNullOrWhiteSpace(g.groupId)) {
                    throw new InvalidOperationException(
                        $"[{nameof(PoolCatalog)}] group at index {gi} has empty {nameof(g.groupId)}."
                    );
                }

                if (!outer.TryAdd(g.groupId, new HashSet<string>(StringComparer.Ordinal))) {
                    throw new InvalidOperationException(
                        $"[{nameof(PoolCatalog)}] duplicate {nameof(g.groupId)} '{g.groupId}'."
                    );
                }

                HashSet<string> ids = outer[g.groupId];

                for (int ei = 0; ei < g.entries.Length; ei++) {
                    PoolEntry e = g.entries[ei];

                    if (string.IsNullOrWhiteSpace(e.id)) {
                        throw new InvalidOperationException(
                            $"[{nameof(PoolCatalog)}] group '{g.groupId}' has entry with empty {nameof(e.id)} at index {ei}."
                        );
                    }

                    if (e.prefab == null) {
                        throw new InvalidOperationException(
                            $"[{nameof(PoolCatalog)}] group '{g.groupId}' entry '{e.id}' has null {nameof(e.prefab)}."
                        );
                    }

                    if (!ids.Add(e.id)) {
                        throw new InvalidOperationException(
                            $"[{nameof(PoolCatalog)}] group '{g.groupId}' has duplicate entry id '{e.id}'."
                        );
                    }
                }
            }
        }
    }
}