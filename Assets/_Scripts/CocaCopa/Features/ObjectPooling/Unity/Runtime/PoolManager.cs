using System;
using System.Collections.Generic;
using CocaCopa.ObjectPooling.Unity.Config;
using UnityEngine;

namespace CocaCopa.ObjectPooling.Unity.Runtime {
    internal sealed class PoolManager : IPoolRuntime {
        private readonly Dictionary<string, Dictionary<string, Pool>> objectPools = new(StringComparer.Ordinal);
        private readonly Dictionary<GameObject, Pool> rentedObjectOwners = new();
        private readonly PoolCatalog poolCatalog;

        internal PoolManager(PoolCatalog catalog) {
            poolCatalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        }

        internal void Initialize() {
            CreateObjectPools();
        }

        public GameObject Rent(string groupId, string objectId, Transform parent = null, bool worldPositionStays = false) {
            Pool pool = GetPoolOrThrow(groupId, objectId);
            GameObject pooledObj = pool.Rent(parent, worldPositionStays);

            if (pooledObj == null) {
                throw new InvalidOperationException(
                    $"[{nameof(PoolManager)}] {nameof(Rent)} failed: pool returned null for group '{groupId}' and object '{objectId}'."
                );
            }

            if (!rentedObjectOwners.TryAdd(pooledObj, pool)) {
                throw new InvalidOperationException(
                    $"[{nameof(PoolManager)}] {nameof(Rent)} failed: rented object '{pooledObj.name}' is already tracked as rented."
                );
            }

            return pooledObj;
        }

        public void Return(GameObject pooledObj) {
            if (pooledObj == null) {
                Debug.LogWarning($"[{nameof(PoolManager)}] {nameof(Return)} failed: pooled object is null.");
                return;
            }

            if (!rentedObjectOwners.Remove(pooledObj, out Pool ownerPool)) {
                FailReturn($"Object '{pooledObj.name}' is not tracked by {nameof(PoolManager)}.");
                return;
            }

            ownerPool.Return(pooledObj);
        }

        public void Prewarm(string groupId, string objectId) {
            Pool pool = GetPoolOrThrow(groupId, objectId);
            pool.ManualPrewarm();
        }

        private Pool GetPoolOrThrow(string groupId, string objectId) {
            if (string.IsNullOrWhiteSpace(groupId)) { throw new ArgumentException($"[{nameof(PoolManager)}] {nameof(groupId)} is null or empty.", nameof(groupId)); }
            if (string.IsNullOrWhiteSpace(objectId)) { throw new ArgumentException($"[{nameof(PoolManager)}] {nameof(objectId)} is null or empty.", nameof(objectId)); }

            if (!objectPools.TryGetValue(groupId, out Dictionary<string, Pool> groupPool)) {
                throw new KeyNotFoundException(
                    $"[{nameof(PoolManager)}] Group '{groupId}' was not found."
                );
            }

            if (!groupPool.TryGetValue(objectId, out Pool pool) || pool == null) {
                throw new KeyNotFoundException(
                    $"[{nameof(PoolManager)}] Object '{objectId}' was not found in group '{groupId}'."
                );
            }

            return pool;
        }

        private void CreateObjectPools() {
            IReadOnlyList<PoolGroup> groups = poolCatalog.Groups;

            for (int i = 0; i < groups.Count; i++) {
                PoolEntry[] entries = groups[i].entries;
                objectPools.Add(groups[i].groupId, new Dictionary<string, Pool>(StringComparer.Ordinal));

                for (int j = 0; j < entries.Length; j++) {
                    PoolEntry entry = entries[j];
                    if (groups[i].prewarmGroup) { entry.prewarm = PrewarmMode.Manual; }
                    var pool = new Pool(entry);
                    Dictionary<string, Pool> groupPool = objectPools[groups[i].groupId];
                    groupPool.Add(entry.id, pool);
                    if (groups[i].prewarmGroup) { pool.ManualPrewarm((int)(entry.maxPoolCount * (groups[i].prewarmPercentage / 100f))); }
                }
            }
        }

        private static void FailReturn(string message) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            throw new InvalidOperationException($"[{nameof(PoolManager)}] {nameof(Return)} failed: {message}");
#else
            Debug.LogError($"[{nameof(PoolManager)}] {nameof(Return)} failed: {message}");
#endif
        }
    }
}