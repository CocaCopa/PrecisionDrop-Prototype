using System;
using UnityEngine;

namespace PrecisionDrop.Player.Unity.Presentation {
    [Serializable]
    public struct PlayerTheme {
        [SerializeField] private Color sphereColor;
        [SerializeField] private Color sphereSmashColor;
        [SerializeField] private Color trailColor;
        [SerializeField] private Color trailSmashColor;
        [SerializeField] private string bounceVfxId;

        public Color SphereColor => sphereColor;
        public Color SphereSmashColor => sphereSmashColor;
        public Color TrailColor => trailColor;
        public Color TrailSmashColor => trailSmashColor;
        public string BounceVfxId => bounceVfxId;

        public PlayerTheme(Color sphereColor, Color sphereSmashColor, Color trailColor, Color trailSmashColor, string bounceVfxId) {
            this.sphereColor = sphereColor;
            this.sphereSmashColor = sphereSmashColor;
            this.trailColor = trailColor;
            this.trailSmashColor = trailSmashColor;
            this.bounceVfxId = bounceVfxId;
        }
    }
}