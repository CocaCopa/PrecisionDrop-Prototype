using System;
using UnityEngine;

namespace CocaCopa.PrefabRegistry {
    public static class PrefabRegistry {
        private const string SettingsResourceName = "PrefabRegistrySettings";

        private static PrefabCatalog catalog;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() {
            catalog = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() {
            EnsureInitialized();
        }

        private static void EnsureInitialized() {
            if (catalog != null) { return; }

            PrefabRegistrySettings[] settingsAssets = Resources.LoadAll<PrefabRegistrySettings>(string.Empty);

            if (settingsAssets.Length == 0) { throw new Exception("Missing PrefabRegistrySettings asset in any Resources folder."); }
            if (settingsAssets.Length > 1) { throw new Exception("Multiple PrefabRegistrySettings assets found in Resources. Only one is allowed."); }

            PrefabRegistrySettings settings = settingsAssets[0];

            if (settings.Catalog == null) { throw new Exception($"PrefabRegistrySettings '{settings.name}' has no catalog assigned."); }

            catalog = settings.Catalog;
            catalog.InvalidateCache();
        }

        private static GameObject GetPrefab(string groupId, string key) {
            EnsureInitialized();
            return catalog.GetPrefab(groupId, key);
        }

        private static bool TryGetPrefab(string groupId, string key, out GameObject prefab) {
            EnsureInitialized();
            return catalog.TryGetPrefab(groupId, key, out prefab);
        }

        private static GameObject Instantiate(string groupId, string key, Transform parent = null) {
            GameObject prefab = GetPrefab(groupId, key);
            return InstantiateInternal(prefab, parent);
        }

        public static bool TryInstantiate(string groupId, string key, Transform parent, out GameObject instance) {
            instance = null;
            if (!TryGetPrefab(groupId, key, out GameObject prefab)) { return false; }

            instance = InstantiateInternal(prefab, parent);
            return true;
        }

        public static GameObject InstantiateEnum(string groupId, Enum key, Transform parent = null) {
            return key == null
                ? throw new ArgumentNullException(nameof(key))
                : Instantiate(groupId, key.ToString(), parent);
        }

        private static GameObject InstantiateInternal(GameObject prefab, Transform parent) {
            GameObject obj = UnityEngine.Object.Instantiate(prefab, parent);
            obj.name = obj.name.Replace("(Clone)", " - Clone");
            return obj;
        }
    }
}