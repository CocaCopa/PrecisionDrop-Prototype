using UnityEngine;

namespace PrecisionDrop.Player.Unity.Presentation {
    public struct PlayerTheme {
        public Material mat;
        
        public bool IsValid => mat != null;

        public PlayerTheme(Material mat) {
            this.mat = mat;
        }
    }
}