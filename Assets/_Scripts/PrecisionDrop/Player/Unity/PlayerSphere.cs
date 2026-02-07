using PrecisionDrop.Player.Contracts;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    [RequireComponent(typeof(Rigidbody))]
    internal sealed class PlayerSphere : MonoBehaviour, IPlayerSphere, IPlayerStateRead, IPlayerStateWrite {
        [SerializeField] private PlayerConfigAsset defaultConfig;

        private Rigidbody sphereRb;

        public bool CanSmash { get; private set; }

        public void SetSmashState(bool enable) {
            CanSmash = enable;
        }

        private void Awake() {
            sphereRb = GetComponent<Rigidbody>();
        }

        public void Jump() {
            sphereRb.linearVelocity = defaultConfig.JumpStrength * Vector3.up;
        }
    }
}
