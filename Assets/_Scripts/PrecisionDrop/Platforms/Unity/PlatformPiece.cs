using System;
using PrecisionDrop.Platforms.Unity.Presentation;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity {
    [RequireComponent(typeof(Collider))]
    internal sealed class PlatformPiece : MonoBehaviour {
        private MeshRenderer platformRenderer;
        private Collider platformCollider;

        internal event Action OnPlayerCollided;
        internal event Action OnPlayerPassed;

        private void Awake() {
            platformCollider = GetComponent<Collider>();
            platformRenderer = GetComponentInChildren<MeshRenderer>();

            if (platformCollider == null) { throw new NullReferenceException($"[{nameof(PlatformPiece)}] Component: '{nameof(Collider)}' not serialized"); }
            if (platformRenderer == null) { throw new NullReferenceException($"[{nameof(PlatformPiece)}] Component: '{nameof(MeshRenderer)}' not serialized"); }
        }

        internal void Init(Vector3 localPos, Vector3 localEuler, PieceVariant type) {
            transform.localPosition = localPos;
            transform.localEulerAngles = localEuler;

            switch (type) {
                case PieceVariant.Normal: break;
                case PieceVariant.Gap: PieceType_Gap(); break;
                case PieceVariant.Danger: PieceType_Danger(); break;
                default: throw new NotImplementedException($"[{nameof(PlatformPiece)}]");
            }
        }

        private void PieceType_Gap() {
            platformRenderer.enabled = false;
            platformCollider.isTrigger = true;
        }

        private void PieceType_Danger() {

        }

        private void OnCollisionEnter(Collision collision) {
            OnPlayerCollided?.Invoke();
        }

        private void OnTriggerEnter(Collider other) {
            OnPlayerPassed?.Invoke();
        }
    }
}
