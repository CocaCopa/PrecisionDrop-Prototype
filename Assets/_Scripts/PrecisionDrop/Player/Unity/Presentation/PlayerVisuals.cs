using System;
using CocaCopa.PrefabRegistry;
using UnityEngine;

namespace PrecisionDrop.Player.Unity.Presentation {
    [RequireComponent(typeof(MeshRenderer))]
    internal sealed class PlayerVisuals : MonoBehaviour {
        [SerializeField] private TrailRenderer trailRenderer;

        private const string PlayerGroupId = "Player_Bounce";

        private MeshRenderer meshRenderer;
        private PlayerTheme theme;

        private void Awake() {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        internal void Install(PlayerTheme theme) {
            this.theme = theme;
        }

        internal void ApplyTheme() {
            meshRenderer.material = theme.SphereMat;
            trailRenderer.material = theme.TrailMat;
        }

        internal void BounceEffect(Transform hitObj) {
            if (hitObj == null) { throw new NullReferenceException($"[{nameof(PlayerVisuals)}] {nameof(hitObj)}"); }

            if (PrefabRegistry.TryInstantiate(PlayerGroupId, theme.BounceVfxId, hitObj, out GameObject bounceVfx)) {
                Vector3 spherePosition = transform.position;
                bounceVfx.transform.position = spherePosition;
            }
        }
    }
}