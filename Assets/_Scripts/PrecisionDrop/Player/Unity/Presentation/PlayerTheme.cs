using UnityEngine;

namespace PrecisionDrop.Player.Unity.Presentation {
    public readonly struct PlayerTheme {
        public readonly Material SphereMat;
        public readonly Material TrailMat;

        public bool IsValid => SphereMat != null;

        public PlayerTheme(Material sphereMat, Material trailMat) {
            SphereMat = sphereMat;
            TrailMat = trailMat;
        }
    }
}