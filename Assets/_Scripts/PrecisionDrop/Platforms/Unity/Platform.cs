using System;
using PrecisionDrop.Platforms.Contracts;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity {
    internal sealed class Platform : MonoBehaviour, IPlatform {
        [SerializeField] private float breakForceAmount;

        private const float BounceCooldown = 0.15f;
        private float bounceTimer;

        internal event Action<Platform, PieceVariant> OnCollidedPlatform;
        internal event Action<Platform> OnPassedPlatform;

        private PlatformPart[] platformParts;
        private PlatformPiece[] platformPieces;

        private int totalParts;
        private bool isBroken;
        private bool isPassed;

        private void OnTriggerEnter(Collider other) {
            if (isPassed) { return; }
            isPassed = true;
            Platform_OnPlayerPassed();
        }

        internal void Init(PlatformPart[] parts, PlatformPiece[] pieces) {
            totalParts = parts.Length;
            platformParts = parts;
            platformPieces = pieces;
            isBroken = false;
            isPassed = false;
            HookPieceEvents(platformPieces);
        }

        private void Piece_OnPlayerCollided(PieceVariant pieceVariant) {
            if (Time.time < bounceTimer) { return; }

            bounceTimer = Time.time + BounceCooldown;
            OnCollidedPlatform?.Invoke(this, pieceVariant);
        }

        private void Platform_OnPlayerPassed() {
            UnhookPieceEvents(platformPieces);
            OnPassedPlatform?.Invoke(this);
        }

        private void HookPieceEvents(PlatformPiece[] pieces) {
            for (int i = 0; i < pieces.Length; i++) {
                PlatformPiece piece = pieces[i];
                piece.OnPlayerCollided += Piece_OnPlayerCollided;
            }
        }

        private void UnhookPieceEvents(PlatformPiece[] pieces) {
            for (int i = 0; i < pieces.Length; i++) {
                PlatformPiece piece = platformPieces[i];
                piece.OnPlayerCollided -= Piece_OnPlayerCollided;
            }
        }

        public void Break() {
            if (isBroken) { return; }
            isBroken = true;
            DisablePieceColliders();
            ThrowParts();
        }

        private void DisablePieceColliders() {
            for (int i = 0; i < platformPieces.Length; i++) {
                PlatformPiece piece = platformPieces[i];
                piece.DisableCollider();
            }
        }

        private void ThrowParts() {
            for (int i = 0; i < platformParts.Length; i++) {
                PlatformPart part = platformParts[i];
                part.Separate();
                Vector3 throwDir = GetRandomDirectionsPerPart(i, Vector3.forward);
                part.Throw(breakForceAmount, -throwDir);
            }
        }

        /// <summary>
        /// Returns a random direction vector within the angular slice defined by <paramref name="partIndex"/>,
        /// where the full 360° circle is evenly divided into <c>totalParts</c> slices.
        /// The slice angles are measured around the Y axis, using <paramref name="referenceDir"/> as the 0° direction.
        /// </summary>
        /// <param name="partIndex">
        /// Index of the slice (0-based) from which the direction will be generated.
        /// </param>
        /// <param name="referenceDir">
        /// The direction that represents 0°. This vector is normalized internally.
        /// </param>
        /// <returns>
        /// A normalized direction vector lying within the angular bounds of the specified slice.
        /// </returns>
        private Vector3 GetRandomDirectionsPerPart(int partIndex, Vector3 referenceDir) {
            float sliceAngle = 360f / totalParts;
            float startAngle = partIndex * sliceAngle;
            float endAngle = (partIndex + 1) * sliceAngle;

            float randomAngle = UnityEngine.Random.Range(startAngle, endAngle);

            return Quaternion.Euler(0f, randomAngle, 0f) * referenceDir.normalized;
        }
    }
}