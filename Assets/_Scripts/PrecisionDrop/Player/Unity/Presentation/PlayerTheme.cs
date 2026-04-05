using System;
using UnityEngine;

namespace PrecisionDrop.Player.Unity.Presentation {
    [Serializable]
    public struct PlayerTheme {
        [Header("Normal")]
        [SerializeField] private Color sphereColor;
        [SerializeField] private Color trailColor;
        [Header("Smash")]
        [SerializeField] private Color sphereSmashColor;
        [SerializeField] private Color trailSmashColor;
        [Header("Bounce")]
        [SerializeField] private Color bounceVfxColor;

        public Color SphereColor => sphereColor;
        public Color SphereSmashColor => sphereSmashColor;
        public Color TrailColor => trailColor;
        public Color TrailSmashColor => trailSmashColor;
        public Color BounceVfxColor => bounceVfxColor;

        public PlayerTheme(Color sphereColor, Color sphereSmashColor, Color trailColor, Color trailSmashColor, Color bounceVfxColor) {
            this.sphereColor = sphereColor;
            this.sphereSmashColor = sphereSmashColor;
            this.trailColor = trailColor;
            this.trailSmashColor = trailSmashColor;
            this.bounceVfxColor = bounceVfxColor;
        }
    }
}