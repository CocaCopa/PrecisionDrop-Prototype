using System;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity {
    internal sealed class Platform : MonoBehaviour {
        private const float BounceCooldown = 0.15f;
        private float bounceTimer;

        public event Action<Platform> OnPassedPlatform;
        public event Action<Platform> OnCollidedPlatform;

        private PlatformPiece[] platformPieces;

        internal void Init(PlatformPiece[] pieces) {
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
    }
}
