using System;
using CocaCopa.ObjectPooling;
using PrecisionDrop.Platforms.Contracts;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity {
    [RequireComponent(typeof(Collider))]
    public sealed class PlatformPiece : MonoBehaviour, IPoolable {
        private Collider pieceCollider;
        private MeshRenderer pieceRenderer;
        private PieceVariant pieceVariant;

        internal event Action<PieceVariant> OnPlayerCollided;
        internal event Action OnPlayerPassed;

        private void Awake() {
            pieceCollider = GetComponent<Collider>();
            pieceRenderer = GetComponentInChildren<MeshRenderer>();

            if (pieceCollider == null) { throw new NullReferenceException($"[{nameof(PlatformPiece)}] Component: '{nameof(Collider)}' not serialized"); }
            if (pieceRenderer == null) { throw new NullReferenceException($"[{nameof(PlatformPiece)}] Component: '{nameof(MeshRenderer)}' not serialized"); }
        }

        private void OnCollisionEnter(Collision _) {
            OnPlayerCollided?.Invoke(pieceVariant);
        }

        private void OnTriggerEnter(Collider other) {
            OnPlayerPassed?.Invoke();
        }

        internal void DisableCollider() {
            pieceCollider.enabled = false;
        }

        internal void Init(Vector3 localPos, Vector3 localEuler, PieceVariant type, Color matColor) {
            pieceVariant = type;
            pieceRenderer.material.color = matColor;
            transform.localPosition = localPos;
            transform.localEulerAngles = localEuler;

            switch (type) {
                case PieceVariant.Normal: break;
                case PieceVariant.Gap: PieceType_Gap(); break;
                case PieceVariant.Danger: break;
                default: throw new NotImplementedException($"[{nameof(PlatformPiece)}]");
            }
        }

        private void PieceType_Gap() {
            pieceRenderer.enabled = false;
            pieceCollider.isTrigger = true;
        }

        public void ResetForReuse() { }

        public void PrepareForRelease() {
            pieceRenderer.enabled = true;
            pieceCollider.enabled = true;
            pieceCollider.isTrigger = false;
        }
    }
}