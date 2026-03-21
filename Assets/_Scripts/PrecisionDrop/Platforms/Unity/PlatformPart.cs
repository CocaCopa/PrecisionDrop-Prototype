using System;
using CocaCopa.ObjectPooling;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity {
    public class PlatformPart : MonoBehaviour, IPoolable {
        [SerializeField] private Vector3 constForce;
        [Space(10)]
        [SerializeField] private RigidbodySettings rigidbodySettings;

        private Rigidbody rb;

        internal void Separate() {
            transform.SetParent(null, true);
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = rigidbodySettings.mass;
            rb.linearDamping = rigidbodySettings.linearDamping;
            rb.interpolation = rigidbodySettings.interpolate;
            rb.collisionDetectionMode = rigidbodySettings.collisionDetection;
        }

        private void FixedUpdate() {
            if (!rb) { return; }

            rb.AddForce(constForce, ForceMode.Force);
        }

        internal void Throw(float forceAmount, Vector3 direction) {
            Vector3 force = forceAmount * direction;
            rb.AddForce(force, ForceMode.Impulse);
        }

        public void ResetForReuse() {
            enabled = true;
        }

        public void PrepareForRelease() {
            Destroy(rb);
            enabled = false;
        }

        [Serializable]
        private struct RigidbodySettings {
            [Min(0.01f)] public float mass;
            [Min(0f)] public float linearDamping;
            public RigidbodyInterpolation interpolate;
            public CollisionDetectionMode collisionDetection;
        }
    }
}