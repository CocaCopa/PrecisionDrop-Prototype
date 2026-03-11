using UnityEngine;

namespace PrecisionDrop.Player.Unity.Presentation {
    public readonly struct PlayerTheme {
        public readonly Material SphereMat;
        public readonly Material TrailMat;
        public readonly string BounceVfxId;

        public bool IsValid => SphereMat != null;

        public PlayerTheme(Material sphereMat, Material trailMat, string bounceVfxId) {
            SphereMat = sphereMat;
            TrailMat = trailMat;
            BounceVfxId = bounceVfxId;
        }
    }
}