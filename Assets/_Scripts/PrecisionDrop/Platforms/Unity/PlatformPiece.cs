using System;
using PrecisionDrop.Platforms.Unity.Presentation;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity {
    [RequireComponent(typeof(Collider))]
    internal sealed class PlatformPiece : MonoBehaviour {
        private MeshRenderer pieceRenderer;
        private Collider pieceCollider;

        internal event Action OnPlayerCollided;
        internal event Action OnPlayerPassed;

        private void Awake() {
            pieceCollider = GetComponent<Collider>();
            pieceRenderer = GetComponentInChildren<MeshRenderer>();

            if (pieceCollider == null) { throw new NullReferenceException($"[{nameof(PlatformPiece)}] Component: '{nameof(Collider)}' not serialized"); }
            if (pieceRenderer == null) { throw new NullReferenceException($"[{nameof(PlatformPiece)}] Component: '{nameof(MeshRenderer)}' not serialized"); }
        }

        internal void DisableCollider() {
            pieceCollider.enabled = false;
        }

        internal void Init(Vector3 localPos, Vector3 localEuler, PieceVariant type, Material mat) {
            pieceRenderer.material = mat;
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
            pieceRenderer.enabled = false;
            pieceCollider.isTrigger = true;
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
