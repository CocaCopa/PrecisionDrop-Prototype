using PrecisionDrop.Player.Contracts;
using PrecisionDrop.Player.Unity.Presentation;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    [RequireComponent(typeof(Rigidbody))]
    internal sealed class PlayerSphere : MonoBehaviour, IPlayerSphere, IPlayerStateRead, IPlayerStateWrite {
        [SerializeField] private PlayerConfigAsset defaultConfig;
        [SerializeField] private PlayerVisuals visuals;

        private bool canJump;
        private Rigidbody sphereRb;
        private bool test;

        private void Awake() {
            sphereRb = GetComponent<Rigidbody>();
            canJump = true;
        }

        public void Jump() {
            if (!canJump) { return; }
            sphereRb.linearVelocity = defaultConfig.JumpStrength * Vector3.up;
        }

        public bool CanSmash { get; private set; }

        public void SetSmashState(bool enable) {
            CanSmash = enable;
        }

        public void Lose() {
            canJump = false;
        }
    }
}