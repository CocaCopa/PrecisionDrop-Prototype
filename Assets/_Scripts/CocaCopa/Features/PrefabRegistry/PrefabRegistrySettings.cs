using UnityEngine;

namespace CocaCopa.PrefabRegistry {
    internal sealed class PrefabRegistrySettings : ScriptableObject {
        [Tooltip("The catalog used by PrefabRegistry.")]
        [SerializeField] private PrefabCatalog catalog;

        public PrefabCatalog Catalog => catalog;

#if UNITY_EDITOR
        public void SetCatalog(PrefabCatalog newCatalog) {
            catalog = newCatalog;
        }
#endif
    }
}