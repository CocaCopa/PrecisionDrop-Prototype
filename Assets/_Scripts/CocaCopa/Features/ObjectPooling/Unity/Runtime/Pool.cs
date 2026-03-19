using System;
using System.Collections.Generic;
using CocaCopa.ObjectPooling.Unity.Config;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace CocaCopa.ObjectPooling.Unity.Runtime {
    internal sealed class Pool {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private const bool PoolCollectionCheck = true;
#else
        private const bool PoolCollectionCheck = false;
#endif

        private readonly Transform unusedObjHolder;
        private readonly ObjectPool<GameObject> objPool;
        private readonly HashSet<GameObject> rentedObjects = new();
        private readonly int prewarmCount;

        private bool poolIsWarmed;

        internal Pool(PoolEntry entry) {
            unusedObjHolder = PoolHelpers.CreateHolderForPooledObjects($"[{nameof(Pool)}] {entry.prefab.name}");
            prewarmCount = entry.prewarmCount;

            Object.DontDestroyOnLoad(unusedObjHolder.gameObject);

            objPool = new ObjectPool<GameObject>(
                () => CreateFunc(entry.prefab),
                PoolGet,
                PoolRelease,
                Object.Destroy,
                PoolCollectionCheck,
                entry.prewarmCount,
                entry.maxPoolCount
            );

            if (entry.prewarm != PrewarmMode.Automatic) { return; }

            ManualPrewarm();
        }

        internal void ManualPrewarm() {
            ManualPrewarm(prewarmCount);
        }

        internal void ManualPrewarm(int count) {
            if (poolIsWarmed) {
                Debug.LogWarning($"[{nameof(Pool)}] Pool is already warmed ({unusedObjHolder.name})");
                return;
            }

            poolIsWarmed = true;
            PoolHelpers.Prewarm(objPool, count);
        }

        private GameObject CreateFunc(GameObject prefab) {
            return Object.Instantiate(prefab, unusedObjHolder);
        }

        private static void PoolGet(GameObject obj) {
            if (obj == null) { return; }

            if (obj.TryGetComponent(out IPoolable poolable)) { poolable.ResetForReuse(); }

            obj.SetActive(true);
        }

        private void PoolRelease(GameObject obj) {
            if (obj == null) { return; }

            if (obj.TryGetComponent(out IPoolable poolable)) { poolable.PrepareForRelease(); }

            obj.SetActive(false);
            obj.transform.SetParent(unusedObjHolder, false);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }

        public GameObject Rent(Transform parent, bool worldPositionStays = false) {
            GameObject pooledObj = objPool.Get();

            if (pooledObj == null) {
                FailRent($"{nameof(objPool)}.{nameof(ObjectPool<GameObject>.Get)} returned null.");
                return null;
            }

            if (!rentedObjects.Add(pooledObj)) {
                FailRent($"{nameof(objPool)} returned an object already marked as rented.");
                return null;
            }

            if (parent == null) { return pooledObj; }

            Transform objTransform = pooledObj.transform;
            objTransform.SetParent(parent, worldPositionStays);

            if (worldPositionStays) { return pooledObj; }

            objTransform.localPosition = Vector3.zero;
            objTransform.localRotation = Quaternion.identity;
            objTransform.localScale = Vector3.one;

            return pooledObj;
        }

        public void Return(GameObject pooledObj) {
            if (pooledObj == null) {
                Debug.LogWarning($"[{nameof(Pool)}] {nameof(Return)} failed: pooled object is null.");
                return;
            }

            if (!rentedObjects.Remove(pooledObj)) {
                FailReturn("object does not belong to this pool or has already been returned.");
                return;
            }

            try { objPool.Release(pooledObj); }
            catch (Exception e) {
                rentedObjects.Add(pooledObj);

                Debug.LogError($"[{nameof(Pool)}] {nameof(Return)} failed: {e}");
            }
        }

        private static void FailRent(string reason) {
            Fail($"{nameof(Rent)} failed: {reason}");
        }

        private static void FailReturn(string reason) {
            Fail($"{nameof(Return)} failed: {reason}");
        }

        private static void Fail(string message) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            throw new InvalidOperationException($"[{nameof(Pool)}] {message}");
#else
            Debug.LogError($"[{nameof(Pool)}] {message}");
#endif
        }
    }
}