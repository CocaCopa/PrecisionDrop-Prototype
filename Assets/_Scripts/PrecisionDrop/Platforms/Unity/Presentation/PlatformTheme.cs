using UnityEngine;

namespace PrecisionDrop.Platforms.Unity.Presentation {
    public struct PlatformTheme {
        public Material regularMat;
        public Material dangerMat;

        public bool IsValid => regularMat != null && dangerMat != null;

        public PlatformTheme(Material regularMat, Material dangerMat) {
            this.regularMat = regularMat;
            this.dangerMat = dangerMat;
        }
    }
}