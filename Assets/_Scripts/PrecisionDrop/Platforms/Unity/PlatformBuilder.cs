using System;
using PrecisionDrop.Platforms.Contracts;
using PrecisionDrop.Platforms.Unity.Presentation;
using UnityEngine;
using RangeInt = CocaCopa.Primitives.RangeInt;

namespace PrecisionDrop.Platforms.Unity {
    internal sealed class PlatformBuilder : MonoBehaviour, IPlatformBuilder {
        [SerializeField] private GameObject platformsHolder;
        [SerializeField] private GameObject platformPrefab;
        [SerializeField, Range(1, 64)] private int segments = 36;
        [Tooltip("The gap between each platform.")]
        [SerializeField] private float platformGap;

        private float prevPlatformGap;
        /// <summary>Used only for inspector naming.</summary>
        private int platformIndex;

        internal event Action<Platform> OnPlatformGenerated;

        private void Awake() {
            prevPlatformGap = 0f;
            platformsHolder.transform.SetParent(transform);
        }

        public void Create(PlatformConfig config) {
            GameObject platformRoot = CreatePlatformRoot(out Platform platform);
            PlatformPiece[] platformPieces = CreatePlatformPieces(platformRoot.transform, config);
            platform.Init(platformPieces);
            prevPlatformGap += platformGap;
            platformIndex++;
            OnPlatformGenerated?.Invoke(platform);
        }

        private GameObject CreatePlatformRoot(out Platform platform) {
            GameObject platformObj = new GameObject($"Platform_{platformIndex}");
            platformObj.transform.SetParent(platformsHolder.transform);
            platformObj.transform.localPosition = Vector3.down * prevPlatformGap;
            platform = platformObj.AddComponent<Platform>();
            return platformObj;
        }

        private PlatformPiece[] CreatePlatformPieces(Transform parent, PlatformConfig config) {
            PlatformPiece[] pieces = new PlatformPiece[segments];

            for (int i = 0; i < segments; i++) {
                PieceVariant type = PieceVariant.Normal;
                if (InZone(i, config.gapPositions)) { type = PieceVariant.Gap; }
                else if (InZone(i, config.dangerPositions)) { type = PieceVariant.Danger; }

                GameObject pieceObj = Instantiate(platformPrefab, parent);
                if (!pieceObj.TryGetComponent<PlatformPiece>(out var platformPiece)) {throw new NullReferenceException($"[{nameof(PlatformBuilder)}] Piece obj does not have '{nameof(PlatformPiece)}' component attached"); }

                Vector3 localPos = Vector3.zero;
                float step = 360f / segments;
                float y = step * (i + 1);
                Vector3 localEuler = new Vector3(0f, y + config.rotationY, 0f);
                platformPiece.Init(localPos, localEuler, type);
                pieces[i] = platformPiece;
            }

            return pieces;
        }

        private bool InZone(int index, RangeInt range) { return index >= range.min && index <= range.max; }
    }
}
