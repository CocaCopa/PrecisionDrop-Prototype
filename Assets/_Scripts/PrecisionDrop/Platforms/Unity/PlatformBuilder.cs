using PrecisionDrop.Platforms.Contracts;
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

        private void Awake() {
            prevPlatformGap = 0f;
            platformsHolder.transform.SetParent(transform);
        }

        public void Create(PlatformConfig config) {
            Vector3 euler = Vector3.zero;
            Vector3 extraRotation = new Vector3(0f, config.rotationY, 0f);
            for (int i = 0; i < segments; i++) {
                euler.y += 360 / segments;
                if (InZone(i, config.gapPositions)) { continue; }

                bool isDanger = InZone(i, config.dangerPositions);
                GameObject platformObj = Instantiate(platformPrefab, platformsHolder.transform);
                platformObj.transform.localPosition = Vector3.zero;
                platformObj.transform.localPosition += Vector3.down * prevPlatformGap;
                platformObj.transform.localEulerAngles = euler + extraRotation;
            }
            prevPlatformGap += platformGap;
        }

        private bool InZone(int index, RangeInt range) { return index >= range.min && index <= range.max; }
    }
}
