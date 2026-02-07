using System;
using PrecisionDrop.Platforms.Contracts;
using PrecisionDrop.Platforms.Unity.Presentation;
using UnityEngine;
using RangeInt = CocaCopa.Primitives.RangeInt;

namespace PrecisionDrop.Platforms.Unity {
    internal sealed class PlatformBuilder : MonoBehaviour, IPlatformBuilder {
        [SerializeField] private GameObject platformPrefab;
        [SerializeField] private GameObject platformPart;
        [SerializeField] private GameObject piecePrefab;
        [SerializeField] private GameObject platformsHolder;
        [SerializeField, Range(1, 64)] private int segments = 36;
        [Tooltip("The gap between each platform.")]
        [SerializeField] private float platformGap;

        private PlatformTheme platformTheme;

        private float prevPlatformGap;

        internal event Action<Platform> OnPlatformGenerated;

        public void Install(PlatformTheme theme) {
            platformTheme = theme;
        }
        
        public void Init() {
            prevPlatformGap = 0f;
            platformsHolder.transform.SetParent(transform);
        }

        public void Create(PlatformConfig config) {
            GameObject platformRoot = CreatePlatformRoot(out Platform platform);
            PlatformPart[] parts = CreatePartsParents(platformRoot.transform, platform.TotalParts);
            PlatformPiece[] platformPieces = CreatePlatformPieces(parts, config);
            platform.Init(parts, platformPieces);
            prevPlatformGap += platformGap;
            OnPlatformGenerated?.Invoke(platform);
        }

        private GameObject CreatePlatformRoot(out Platform platform) {
            GameObject platformObj = Instantiate(platformPrefab, platformsHolder.transform, false);
            platformObj.transform.localPosition = Vector3.down * prevPlatformGap;
            return platformObj.TryGetComponent(out platform) 
                ? platformObj
                : throw new NullReferenceException($"[{nameof(PlatformBuilder)}] Could not get {nameof(Platform)} component");
        }

        private PlatformPart[] CreatePartsParents(Transform root, int totalParts) {
            PlatformPart[] parents = new PlatformPart[totalParts];
            for (int i = 0; i < totalParts; i++) {
                var parent = Instantiate(platformPart, root).transform;
                if (!parent.TryGetComponent<PlatformPart>(out var part)) { throw new NullReferenceException($"[{nameof(PlatformBuilder)}] Part obj does not have '{nameof(PlatformPart)}' component attached"); }
                
                parent.localPosition = Vector3.zero;
                parent.localEulerAngles = Vector3.zero;
                parent.name += $"{i+1:0}";
                parents[i] = part;
            }

            return parents;
        }

        private PlatformPiece[] CreatePlatformPieces(PlatformPart[] parents, PlatformConfig config) {
            PlatformPiece[] pieces = new PlatformPiece[segments];

            if (segments % parents.Length != 0) { throw new Exception($"[{nameof(PlatformBuilder)}] Uneven platform distribution is currently not supported."); }
            int piecesPerParent = segments / parents.Length;
            int parentIndex = 0;
            int pieceIndex = 0;

            for (int i = 0; i < segments; i++) {
                if (pieceIndex == piecesPerParent) {
                    pieceIndex = 0;
                    parentIndex++;
                }

                PieceVariant type = PieceVariant.Normal;
                if (InZone(i, config.gapPositions)) { type = PieceVariant.Gap; }
                else if (InZone(i, config.dangerPositions)) { type = PieceVariant.Danger; }
                
                GameObject pieceObj = Instantiate(piecePrefab, parents[parentIndex].transform);
                if (!pieceObj.TryGetComponent<PlatformPiece>(out var platformPiece)) { throw new NullReferenceException($"[{nameof(PlatformBuilder)}] Piece obj does not have '{nameof(PlatformPiece)}' component attached"); }

                Vector3 localPos = Vector3.zero;
                float step = 360f / segments;
                float y = step * (i + 1);
                Vector3 localEuler = new Vector3(0f, y + config.rotationY, 0f);
                platformPiece.Init(localPos, localEuler, type, GetMaterial(type, platformTheme));
                pieces[i] = platformPiece;
                pieceIndex++;
            }

            return pieces;
        }

        private static bool InZone(int index, RangeInt range) { return index >= range.min && index <= range.max; }

        private static Material GetMaterial(PieceVariant type, PlatformTheme theme) {
            return type == PieceVariant.Danger
                ? theme.dangerMat
                : theme.regularMat;
        }
    }
}
