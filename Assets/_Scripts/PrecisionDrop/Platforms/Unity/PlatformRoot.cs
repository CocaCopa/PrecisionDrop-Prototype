using System;
using System.Collections;
using CocaCopa.ObjectPooling;
using PrecisionDrop.Platforms.Contracts;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity {
    public sealed class PlatformRoot : MonoBehaviour, IPlatform {
        [SerializeField] private PlatformAudio sfx;
        [SerializeField] private float breakForceAmount;

        private const float BounceCooldown = 0.5f;
        private float bounceTimer;

        internal event Action<PlatformRoot, CollisionData> OnCollidedPlatform;
        internal event Action<PlatformRoot> OnPassedPlatform;

        private PlatformPart[] platformParts;
        private PlatformPiece[] platformPieces;

        private int totalParts;
        private bool isBroken;
        private bool isPassed;

        private void Awake() {
            Application.targetFrameRate = 120;
        }

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

        private void Piece_OnPlayerCollided(CollisionData colData) {
            if (Time.time < bounceTimer) { return; }

            bounceTimer = Time.time + BounceCooldown;
            OnCollidedPlatform?.Invoke(this, colData);
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
                PlatformPiece piece = pieces[i];
                piece.OnPlayerCollided -= Piece_OnPlayerCollided;
            }
        }

        public void Break() {
            if (isBroken) { return; }

            isBroken = true;
            DisablePieceColliders();
            ThrowParts();
            StartCoroutine(ReturnPlatformToPool());
            sfx.PlayBreak();
        }

        private IEnumerator ReturnPlatformToPool() {
            yield return new WaitForSeconds(5f);
            for (int i = 0; i < platformPieces.Length; i++) {
                PlatformPiece piece = platformPieces[i];
                PoolApi.Return(piece.gameObject);
            }
            for (int i = 0; i < platformParts.Length; i++) {
                PlatformPart part = platformParts[i];
                PoolApi.Return(part.gameObject);
            }
            PoolApi.Return(gameObject);
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
                Vector3 throwDir = GetRandomDirectionForPart(part, transform.forward);
                part.Throw(breakForceAmount, throwDir);
            }
        }

        /// <summary>
        /// Returns a random direction inside the angular slice occupied by the actual chunk in space.
        /// </summary>
        private Vector3 GetRandomDirectionForPart(PlatformPart part, Vector3 referenceDir) {
            Vector3 platformCenter = transform.position;
            Vector3 partCenter = GetPartWorldCenter(part);

            Vector3 toPart = partCenter - platformCenter;
            toPart.y = 0f;

            if (toPart.sqrMagnitude <= 0.0001f) { return referenceDir.normalized; }

            referenceDir.y = 0f;
            referenceDir.Normalize();
            toPart.Normalize();

            float sliceAngle = 360f / totalParts;
            float centerAngle = Vector3.SignedAngle(referenceDir, toPart, Vector3.up);

            float startAngle = centerAngle - sliceAngle * 0.5f;
            float endAngle = centerAngle + sliceAngle * 0.5f;
            float randomAngle = UnityEngine.Random.Range(startAngle, endAngle);

            Vector3 direction = Quaternion.AngleAxis(randomAngle, Vector3.up) * referenceDir;
            return direction.normalized;
        }

        private static Vector3 GetPartWorldCenter(PlatformPart part) {
            Renderer[] renderers = part.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0) { return part.transform.position; }

            Bounds combinedBounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) { combinedBounds.Encapsulate(renderers[i].bounds); }

            return combinedBounds.center;
        }
    }
}