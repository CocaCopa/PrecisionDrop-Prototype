using UnityEngine;
using UnityEngine.Pool;

namespace CocaCopa.ObjectPooling.Unity.Runtime {
    internal static class PoolHelpers {
        /// <summary>
        /// Creates a hidden holder for inactive pooled objects to avoid hierarchy clutter;
        /// </summary>
        internal static Transform CreateHolderForPooledObjects(string name) {
            Transform holder = new GameObject(name).transform;
            holder.SetParent(null);
            holder.position = Vector3.zero;
            holder.localRotation = Quaternion.identity;
            holder.localScale = Vector3.one;
            // holder.hideFlags = HideFlags.HideInHierarchy;
            return holder;
        }

        internal static void Prewarm(ObjectPool<GameObject> pool, int count) {
            var warmed = new GameObject[count];

            for (int i = 0; i < count; i++) { warmed[i] = pool.Get(); }
            for (int i = 0; i < count; i++) { pool.Release(warmed[i]); }
        }
    }
}