using System;
using UnityEngine;

namespace PrecisionDrop.Player.Unity.Presentation {
    [Serializable]
    public struct PlayerTheme {
        [SerializeField] private Color sphereColor;
        [SerializeField] private Color smashColor;
        [SerializeField] private Color trailColor;
        [SerializeField] private string bounceVfxId;

        public Color SphereColor => sphereColor;
        public Color SmashColor => smashColor;
        public Color TrailColor => trailColor;
        public string BounceVfxId => bounceVfxId;

        public PlayerTheme(Color sphereColor, Color smashColor, Color trailColor, string bounceVfxId) {
            this.sphereColor = sphereColor;
            this.smashColor = smashColor;
            this.trailColor = trailColor;
            this.bounceVfxId = bounceVfxId;
        }
    }
}