using PrecisionDrop.Player.Contracts;
using PrecisionDrop.Player.Unity.Presentation;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerSphere : MonoBehaviour, IPlayerSphere, IPlayerStateRead, IPlayerStateWrite {
        [SerializeField] private PlayerVisuals visuals;

        private PlayerConfigAsset config;
        private Rigidbody sphereRb;

        private bool canJump;
        private bool test;
        private float jumpStrength;

        private void Awake() {
            sphereRb = GetComponent<Rigidbody>();
            canJump = true;
        }

        public void Install(float jumpStrengthValue) {
            jumpStrength = jumpStrengthValue;
        }

        public void Jump() {
            if (!canJump) { return; }
            sphereRb.linearVelocity = jumpStrength * Vector3.up;
            visuals.BounceEffect(GetCollidedObj());
        }

        public bool CanSmash { get; private set; }

        public void SetSmashState(bool enable) {
            CanSmash = enable;
            visuals.SmashState(enable);
        }

        public void Lose() {
            canJump = false;

            sphereRb.interpolation = RigidbodyInterpolation.None;
            transform.SetParent(GetCollidedObj().parent);
        }

        private Transform GetCollidedObj() {
            return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit)
                ? hit.transform
                : null;
        }
    }
}