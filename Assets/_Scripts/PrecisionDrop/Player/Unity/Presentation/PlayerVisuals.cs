using UnityEngine;

namespace PrecisionDrop.Player.Unity.Presentation {
    [RequireComponent(typeof(MeshRenderer))]
    internal sealed class PlayerVisuals : MonoBehaviour {
        private MeshRenderer meshRenderer;
        private PlayerTheme theme;

        private void Awake() {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        internal void Install(PlayerTheme theme) {
            this.theme = theme;
        }

        internal void ApplyTheme() {
            meshRenderer.material = theme.mat;
        }
    }
}