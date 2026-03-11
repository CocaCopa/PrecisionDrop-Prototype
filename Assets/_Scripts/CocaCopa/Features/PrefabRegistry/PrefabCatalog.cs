using System;
using System.Collections.Generic;
using UnityEngine;

namespace CocaCopa.PrefabRegistry {
    internal sealed class PrefabCatalog : ScriptableObject {
        [SerializeField] private PrefabGroup[] groups = Array.Empty<PrefabGroup>();

        private Dictionary<string, Dictionary<string, GameObject>> map;

        public GameObject GetPrefab(string groupId, string key) {
            if (string.IsNullOrWhiteSpace(groupId)) { throw new ArgumentException("groupId is null/empty."); }
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentException("key is null/empty."); }

            EnsureBuilt();
            if (!map.TryGetValue(groupId, out Dictionary<string, GameObject> groupMap)) { throw new KeyNotFoundException($"Prefab group '{groupId}' not found in catalog '{name}'."); }
            if (!groupMap.TryGetValue(key, out GameObject prefab) || prefab == null) { throw new KeyNotFoundException($"Prefab key '{key}' not found in group '{groupId}' (catalog '{name}')."); }

            return prefab;
        }

        public bool TryGetPrefab(string groupId, string key, out GameObject prefab) {
            prefab = null;
            if (string.IsNullOrWhiteSpace(groupId) || string.IsNullOrWhiteSpace(key)) { return false; }

            EnsureBuilt();
            if (!map.TryGetValue(groupId, out Dictionary<string, GameObject> groupMap)) { return false; }
            if (!groupMap.TryGetValue(key, out prefab)) { return false; }
            return prefab != null;
        }

        public IReadOnlyList<PrefabGroup> Groups => groups;

        private void EnsureBuilt() {
            if (map != null) { return; }
            map = BuildMapOrThrow(groups, this);
        }

        public void InvalidateCache() {
            map = null;
        }

#if UNITY_EDITOR
        private void OnValidate() {
            try { BuildMapOrThrow(groups, this); }
            catch (Exception e) { Debug.LogError(e.Message, this); }

            // Force rebuild next time in playmode.
            InvalidateCache();
        }
#endif

        private static Dictionary<string, Dictionary<string, GameObject>> BuildMapOrThrow(PrefabGroup[] groups, UnityEngine.Object context) {
            var outer = new Dictionary<string, Dictionary<string, GameObject>>(StringComparer.Ordinal);

            if (groups == null) { return outer; }

            for (int gi = 0; gi < groups.Length; gi++) {
                PrefabGroup g = groups[gi];
                g.entries ??= Array.Empty<PrefabEntry>();

                if (string.IsNullOrWhiteSpace(g.groupId)) { throw new Exception($"PrefabCatalog '{context.name}': group at index {gi} has empty groupId."); }

                if (outer.ContainsKey(g.groupId)) { throw new Exception($"PrefabCatalog '{context.name}': duplicate groupId '{g.groupId}'."); }

                var inner = new Dictionary<string, GameObject>(StringComparer.Ordinal);

                for (int ei = 0; ei < g.entries.Length; ei++) {
                    PrefabEntry e = g.entries[ei];

                    if (string.IsNullOrWhiteSpace(e.key)) {
                        throw new Exception(
                            $"PrefabCatalog '{context.name}': group '{g.groupId}' has entry with empty key at index {ei}."
                        );
                    }

                    if (e.prefab == null) {
                        throw new Exception(
                            $"PrefabCatalog '{context.name}': group '{g.groupId}' key '{e.key}' has NULL prefab."
                        );
                    }

                    if (!inner.TryAdd(e.key, e.prefab)) {
                        throw new Exception(
                            $"PrefabCatalog '{context.name}': group '{g.groupId}' has duplicate key '{e.key}'."
                        );
                    }
                }

                outer.Add(g.groupId, inner);
            }

            return outer;
        }

        [Serializable]
        public struct PrefabGroup {
            public string groupId;
            public PrefabEntry[] entries;
        }

        [Serializable]
        public struct PrefabEntry {
            public string key;
            public GameObject prefab;
        }
    }
}