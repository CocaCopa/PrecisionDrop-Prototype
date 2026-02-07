using System;
using System.Collections.Generic;
using PrecisionDrop.Platforms.Contracts;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity {
    internal sealed class Platform : MonoBehaviour, IPlatform {
        [SerializeField, Min(1)] private int totalParts;
        [SerializeField] private float breakForceAmount;
        
        private const float BounceCooldown = 0.15f;
        private float bounceTimer;

        public event Action<Platform> OnPassedPlatform;
        public event Action<Platform> OnCollidedPlatform;

        private PlatformPart[] platformParts;
        private PlatformPiece[] platformPieces;

        internal int TotalParts => totalParts;

        internal void Init(PlatformPart[] parts, PlatformPiece[] pieces) {
            this.platformParts = parts;
            this.platformPieces = pieces;
            for (int i = 0; i < pieces.Length; i++) {
                var piece = pieces[i];
                HookPieceEvents(piece);
            }
        }

        private void Piece_OnPlayerCollided() {
            if (Time.time < bounceTimer) { return; }
            bounceTimer = Time.time + BounceCooldown;
            OnCollidedPlatform?.Invoke(this);
        }

        private void Piece_OnPlayerPassed() {
            for (int i = 0; i < platformPieces.Length; i++) {
                var piece = platformPieces[i];
                UnhookPieceEvents(piece);
            }
            Break();
            OnPassedPlatform?.Invoke(this);
        }

        private void HookPieceEvents(PlatformPiece piece) {
            piece.OnPlayerCollided += Piece_OnPlayerCollided;
            piece.OnPlayerPassed += Piece_OnPlayerPassed;
        }

        private void UnhookPieceEvents(PlatformPiece piece) {
            piece.OnPlayerCollided -= Piece_OnPlayerCollided;
            piece.OnPlayerPassed -= Piece_OnPlayerPassed;
        }
       
        public void Break() {
            DisablePieceColliders();
            ThrowParts();
        }

        private void DisablePieceColliders() {
            for (int i = 0; i < platformPieces.Length; i++) {
                var piece = platformPieces[i];
                piece.DisableCollider();
            }
        }

        private void ThrowParts() {
            
            for (int i = 0; i < platformParts.Length; i++) {
                var part = platformParts[i];
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
        private Vector3 GetRandomDirectionsPerPart(int partIndex, Vector3 referenceDir)
        {
            float sliceAngle = 360f / totalParts;
            float startAngle = partIndex * sliceAngle;
            float endAngle   = (partIndex + 1) * sliceAngle;

            float randomAngle = UnityEngine.Random.Range(startAngle, endAngle);

            return Quaternion.Euler(0f, randomAngle, 0f) * referenceDir.normalized;
        }
    }
}
