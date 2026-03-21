using System;
using CocaCopa.ObjectPooling.Unity.Config;
using CocaCopa.ObjectPooling.Unity.Runtime;
using UnityEngine;

namespace CocaCopa.ObjectPooling.Bootstrap {
    public static class PoolBootstrap {
        private static PoolCatalog catalog;
        private static PoolManager poolManager;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() {
            catalog = null;
            poolManager = null;
            PoolApi.ResetState();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() {
            EnsureInitialized();

            poolManager = new PoolManager(catalog);
            poolManager.Initialize();
            PoolApi.Bind(poolManager);
        }

        private static void EnsureInitialized() {
            if (catalog != null) { return; }

            PoolSettings[] settingsAssets = Resources.LoadAll<PoolSettings>(string.Empty);

            if (settingsAssets.Length == 0) { throw new Exception($"[{nameof(PoolBootstrap)}] Missing {nameof(PoolSettings)} asset in any Resources folder."); }
            if (settingsAssets.Length > 1) { throw new Exception($"[{nameof(PoolBootstrap)}] Multiple {nameof(PoolSettings)} assets found in Resources. Only one is allowed."); }

            PoolSettings settings = settingsAssets[0];

            if (settings.Catalog == null) {
                throw new Exception(
                    $"[{nameof(PoolBootstrap)}] {nameof(PoolSettings)} '{settings.name}' has no catalog assigned."
                );
            }

            catalog = settings.Catalog;
        }
    }
}