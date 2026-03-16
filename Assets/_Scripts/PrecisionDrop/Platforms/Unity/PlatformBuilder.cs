using System;
using System.Linq;
using CocaCopa.PrefabRegistry;
using PrecisionDrop.Platforms.Contracts;
using PrecisionDrop.Platforms.Unity.Presentation;
using UnityEngine;
using RangeInt = CocaCopa.Primitives.RangeInt;

namespace PrecisionDrop.Platforms.Unity {
    internal sealed class PlatformBuilder : MonoBehaviour, IPlatformBuilder {
        [SerializeField] private GameObject platformsHolder;
        [Space(10f)]
        [SerializeField] private int totalParts = 3;
        [SerializeField] [Range(2, 64)] private int segments = 36;
        [Tooltip("The gap between each platform.")]
        [SerializeField] private float platformGap;

        private const string PrefabGroupID = "Platform";
        private const string PlatformSegmentKey = "Segment";
        private const string PlatformPieceKey = "Piece";
        private const string PlatformPartKey = "Part";

        private PlatformTheme platformTheme;

        private float prevPlatformGap;

        internal event Action<Platform> OnPlatformGenerated;

        public int PlatformSegments => segments;

        public void Install(PlatformTheme theme) {
            platformTheme = theme;
        }

        public void Init() {
            prevPlatformGap = 0f;
        }

        public void Create(PlatformConfig config) {
            GameObject platformRoot = CreatePlatformRoot(out Platform platform);
            PlatformPart[] parts = CreatePartsParents(platformRoot.transform);
            PlatformPiece[] platformPieces = CreatePlatformPieces(parts, config);
            platform.Init(parts, platformPieces);
            prevPlatformGap += platformGap;
            OnPlatformGenerated?.Invoke(platform);
        }

        private GameObject CreatePlatformRoot(out Platform platform) {
            if (!PrefabRegistry.TryInstantiate(PrefabGroupID, PlatformSegmentKey, platformsHolder.transform, out GameObject platformObj)) { ThrowPrefabException(PlatformSegmentKey); }

            platformObj.transform.localPosition = Vector3.down * prevPlatformGap;
            return platformObj.TryGetComponent(out platform)
                ? platformObj
                : throw new NullReferenceException(
                    $"[{nameof(PlatformBuilder)}] Could not get {nameof(Platform)} component");
        }

        private PlatformPart[] CreatePartsParents(Transform root) {
            var parents = new PlatformPart[totalParts];
            for (int i = 0; i < totalParts; i++) {
                if (!PrefabRegistry.TryInstantiate(PrefabGroupID, PlatformPartKey, root, out GameObject parentObj)) { ThrowPrefabException(PlatformPartKey); }
                Transform parent = parentObj.transform;
                if (!parent.TryGetComponent<PlatformPart>(out PlatformPart part)) { ThrowComponentException(nameof(PlatformPart), "Part"); }

                parent.localPosition = Vector3.zero;
                parent.localEulerAngles = Vector3.zero;
                parent.name += $"{i + 1:0}";
                parents[i] = part;
            }

            return parents;
        }

        private PlatformPiece[] CreatePlatformPieces(PlatformPart[] parents, PlatformConfig config) {
            var pieces = new PlatformPiece[segments];

            float step = 360f / segments;
            int piecesPerParent = segments / parents.Length;
            int remainder = segments % parents.Length;
            int parentIndex = 0;
            int pieceIndex = 0;

            for (int i = 0; i < segments; i++) {
                int extra = parentIndex == 0 ? remainder : 0;
                if (pieceIndex == piecesPerParent + extra) {
                    pieceIndex = 0;
                    parentIndex++;
                }

                var type = PieceVariant.Normal;
                if (InZone(i, config.GapPositions)) { type = PieceVariant.Gap; }
                else if (InZone(i, config.DangerPositions)) { type = PieceVariant.Danger; }

                if (!PrefabRegistry.TryInstantiate(PrefabGroupID, PlatformPieceKey, parents[parentIndex].transform, out GameObject pieceObj)) { ThrowPrefabException(PlatformPieceKey); }
                if (!pieceObj.TryGetComponent<PlatformPiece>(out PlatformPiece platformPiece)) { ThrowComponentException(nameof(PlatformPiece), "Piece"); }

                Vector3 localPos = Vector3.zero;
                float y = step * (i + 1);
                var localEuler = new Vector3(0f, y + config.RotationY, 0f);
                platformPiece.Init(localPos, localEuler, type, GetMaterial(type, platformTheme));
                pieces[i] = platformPiece;
                pieceIndex++;
            }

            return pieces;
        }

        private static void ThrowPrefabException(string key) {
            throw new Exception(
                $"[{nameof(PlatformBuilder)}] Could not fetch prefab using key '{key}' and group ID '{PrefabGroupID}'");
        }

        private static void ThrowComponentException(string componentName, string objName) {
            throw new NullReferenceException(
                $"[{nameof(PlatformBuilder)}] Could not fetch '{nameof(PlatformPiece)}' component from {objName} object");
        }

        private static bool InZone(int index, RangeInt range) {
            return index >= range.min && index < range.max;
        }

        private static bool InZone(int index, RangeInt[] ranges) {
            return ranges.Any(r => index >= r.min && index < r.max);
        }

        private static Material GetMaterial(PieceVariant type, PlatformTheme theme) {
            return type == PieceVariant.Danger
                ? theme.DangerMat
                : theme.RegularMat;
        }
    }
}