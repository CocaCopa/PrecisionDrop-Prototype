using UnityEngine;

namespace CocaCopa.ObjectPooling.Unity.Config {
    internal sealed class PoolSettings : ScriptableObject {
        [Header("The catalog used by Pool")]
        [SerializeField] private PoolCatalog poolCatalog;

        public PoolCatalog Catalog => poolCatalog;

#if UNITY_EDITOR
        public void SetCatalog(PoolCatalog newCatalog) {
            poolCatalog = newCatalog;
        }
#endif
    }
}