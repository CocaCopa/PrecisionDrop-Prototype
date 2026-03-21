using System;
using UnityEngine;

namespace CocaCopa.ObjectPooling {
    public static class PoolApi {
        private static IPoolRuntime runtime;

        internal static void Bind(IPoolRuntime poolManager) {
            runtime = poolManager ?? throw new ArgumentNullException(nameof(poolManager));
        }

        internal static void ResetState() {
            runtime = null;
        }

        public static GameObject Rent(string groupId, string objectId, Transform parent = null, bool worldPositionStays = false) {
            EnsureBound();
            return runtime.Rent(groupId, objectId, parent, worldPositionStays);
        }

        public static void Return(GameObject pooledObj) {
            EnsureBound();
            runtime.Return(pooledObj);
        }

        public static void Prewarm(string groupId, string prefabId) {
            EnsureBound();
            runtime.Prewarm(groupId, prefabId);
        }

        private static void EnsureBound() {
            if (runtime != null) { return; }

            throw new InvalidOperationException(
                $"[{nameof(PoolApi)}] Pool system has not been initialized."
            );
        }
    }
}