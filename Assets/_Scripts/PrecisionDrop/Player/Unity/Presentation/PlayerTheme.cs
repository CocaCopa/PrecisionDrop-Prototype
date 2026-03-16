using System;
using UnityEngine;

namespace PrecisionDrop.Player.Unity.Presentation {
    [Serializable]
    public struct PlayerTheme {
        [SerializeField] private Material sphereMat;
        [SerializeField] private Material smashMat;
        [SerializeField] private Material trailMat;
        [SerializeField] private string bounceVfxId;

        public Material SphereMat => sphereMat;
        public Material SmashMat => smashMat;
        public Material TrailMat => trailMat;
        public string BounceVfxId => bounceVfxId;

        public PlayerTheme(Material sphereMat, Material smashMat, Material trailMat, string bounceVfxId) {
            this.sphereMat = sphereMat;
            this.smashMat = smashMat;
            this.trailMat = trailMat;
            this.bounceVfxId = bounceVfxId;
        }
    }
}