using System;
using System.Linq;
using CocaCopa.ObjectPooling;
using PrecisionDrop.Platforms.Contracts;
using PrecisionDrop.Platforms.Unity.Presentation;
using UnityEngine;
using RangeInt = CocaCopa.Primitives.RangeInt;

namespace PrecisionDrop.Platforms.Unity {
    internal sealed class PlatformBuilder : MonoBehaviour, IPlatformBuilder {
        [Header("Pool Selection")]
        [SerializeField] private string poolId;
        [SerializeField] private string rootId;
        [SerializeField] private string partId;
        [SerializeField] private string pieceId;

        [Header("Holder")]
        [SerializeField] private GameObject platformsHolder;

        [Header("Settings")]
        [SerializeField] private int totalParts = 3;
        [SerializeField] [Range(2, 64)] private int segments = 36;
        [Tooltip("The gap between each platform.")]
        [SerializeField] private float platformGap;

        private PlatformTheme platformTheme;

        private float prevPlatformGap;

        internal event Action<PlatformRoot> OnPlatformGenerated;

        public int PlatformSegments => segments;

        public void Install(PlatformTheme theme) {
            platformTheme = theme;
        }

        public void Init() {
            prevPlatformGap = 0f;
        }

        public void Create(PlatformConfig config) {
            GameObject platformRoot = CreatePlatformRoot(out PlatformRoot platform);
            PlatformPart[] parts = CreatePartsParents(platformRoot.transform);
            PlatformPiece[] platformPieces = CreatePlatformPieces(parts, config);
            platform.Init(parts, platformPieces);
            prevPlatformGap += platformGap;
            OnPlatformGenerated?.Invoke(platform);
        }

        private GameObject CreatePlatformRoot(out PlatformRoot platform) {
            GameObject platformObj = PoolApi.Rent(poolId, rootId, platformsHolder.transform);

            platformObj.transform.localPosition = Vector3.down * prevPlatformGap;
            return platformObj.TryGetComponent(out platform)
                ? platformObj
                : throw new NullReferenceException(
                    $"[{nameof(PlatformBuilder)}] Could not get {nameof(PlatformRoot)} component");
        }

        private PlatformPart[] CreatePartsParents(Transform root) {
            var parents = new PlatformPart[totalParts];
            for (int i = 0; i < totalParts; i++) {
                GameObject parentObj = PoolApi.Rent(poolId, partId, root);
                Transform parent = parentObj.transform;
                if (!parent.TryGetComponent(out PlatformPart part)) { ThrowComponentException(nameof(PlatformPart), "Part"); }

                parent.localPosition = Vector3.zero;
                parent.localEulerAngles = Vector3.zero;
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

                GameObject pieceObj = PoolApi.Rent(poolId, pieceId, parents[parentIndex].transform);

                if (!pieceObj.TryGetComponent(out PlatformPiece platformPiece)) { ThrowComponentException(nameof(PlatformPiece), "Piece"); }

                Vector3 localPos = Vector3.zero;
                float y = step * (i + 1);
                var localEuler = new Vector3(0f, y + config.RotationY, 0f);
                platformPiece.Init(localPos, localEuler, type, GetColor(type, platformTheme));
                pieces[i] = platformPiece;
                pieceIndex++;
            }

            return pieces;
        }

        private static void ThrowComponentException(string componentName, string objName) {
            throw new NullReferenceException(
                $"[{nameof(PlatformBuilder)}] Could not fetch '{componentName}' component from {objName} object");
        }

        private static bool InZone(int index, RangeInt[] ranges) {
            return ranges.Any(r => index >= r.min && index < r.max);
        }

        private static Color GetColor(PieceVariant type, PlatformTheme theme) {
            return type == PieceVariant.Danger
                ? theme.DangerColor
                : theme.RegularColor;
        }
    }
}