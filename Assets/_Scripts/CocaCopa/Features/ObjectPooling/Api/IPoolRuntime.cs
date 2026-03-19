using UnityEngine;

namespace CocaCopa.ObjectPooling {
    internal interface IPoolRuntime {
        GameObject Rent(string groupId, string objectId, Transform parent = null, bool worldPositionStays = false);
        void Return(GameObject pooledObj);
        void Prewarm(string groupId, string objectId);
    }
}