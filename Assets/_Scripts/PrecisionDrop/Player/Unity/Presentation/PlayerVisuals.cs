using System;
using System.Collections;
using CocaCopa.Core.Animation;
using CocaCopa.PrefabRegistry;
using UnityEngine;

namespace PrecisionDrop.Player.Unity.Presentation {
    [RequireComponent(typeof(MeshRenderer))]
    internal sealed class PlayerVisuals : MonoBehaviour {
        [Header("Fall Effects")]
        [SerializeField] private TrailRenderer trailRenderer;

        [Header("Smash Visuals")]
        [SerializeField] private AnimationCurve smashColorCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float smashColorChangeTime;

        private const string PlayerGroupId = "Player_Bounce";

        private MeshRenderer meshRenderer;
        private PlayerTheme theme;

        private ValueAnimator smashColorChangeAnim;

        private void Awake() {
            meshRenderer = GetComponent<MeshRenderer>();
            smashColorChangeAnim = ValueAnimator.ByDuration(0f, 1f, smashColorChangeTime, new Easing(smashColorCurve));
        }

        internal void Install(PlayerTheme themeRef) {
            theme = themeRef;
        }

        internal void ApplyTheme() {
            meshRenderer.material.color = theme.SphereColor;
            trailRenderer.material.color = theme.TrailColor;
        }

        internal void BounceEffect(Transform hitObj) {
            if (hitObj == null) { throw new NullReferenceException($"[{nameof(PlayerVisuals)}] {nameof(hitObj)}"); }

            if (PrefabRegistry.TryInstantiate(PlayerGroupId, theme.BounceVfxId, hitObj, out GameObject bounceVfx)) {
                Vector3 spherePosition = transform.position;
                bounceVfx.transform.position = spherePosition;
            }
        }

        internal void SmashState(bool enable) {
            Color sphereCurrent = enable ? theme.SphereColor : theme.SphereSmashColor;
            Color sphereTarget = enable ? theme.SphereSmashColor : theme.SphereColor;
            Color trailCurrent = enable ? theme.TrailColor : theme.TrailSmashColor;
            Color trailTarget = enable ? theme.TrailSmashColor : theme.TrailColor;
            StartCoroutine(SmashStateRoutine(new SmashColors(sphereCurrent, sphereTarget), new SmashColors(trailCurrent, trailTarget)));
        }

        private IEnumerator SmashStateRoutine(SmashColors sphereColors, SmashColors trailColors) {
            smashColorChangeAnim.ResetAnimator();
            while (!smashColorChangeAnim.IsComplete) {
                float t = smashColorChangeAnim.Evaluate(Time.deltaTime);
                meshRenderer.material.color = Color.Lerp(sphereColors.CurrentColor, sphereColors.TargetColor, t);
                trailRenderer.material.color = Color.Lerp(trailColors.CurrentColor, trailColors.TargetColor, t);
                yield return null;
            }
        }

        private readonly struct SmashColors {
            public readonly Color CurrentColor;
            public readonly Color TargetColor;

            public SmashColors(Color currentColor, Color targetColor) {
                CurrentColor = currentColor;
                TargetColor = targetColor;
            }
        }

        private class Easing : IEasing {
            private readonly AnimationCurve curve;

            public Easing(AnimationCurve curve) {
                this.curve = curve;
            }

            public float Evaluate(float t) {
                return curve.Evaluate(t);
            }
        }
    }
}