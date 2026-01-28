using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    [RequireComponent(typeof(Rigidbody))]
    internal sealed class PlayerSphere : MonoBehaviour {
        [SerializeField] private float jumpStrength;

        private Rigidbody sphereRb;

        private void Awake() {
            sphereRb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision) {
            sphereRb.linearVelocity = jumpStrength * Vector3.up;
        }
    }
}
