using System;
using System.Collections;
using CocaCopa.Core.Animation;
using CocaCopa.ObjectPooling;
using UnityEngine;

namespace PrecisionDrop.Player.Unity.Presentation {
    [RequireComponent(typeof(MeshRenderer))]
    internal sealed class PlayerVisuals : MonoBehaviour {
        [Header("Fall Effects")]
        [SerializeField] private TrailRenderer trailRenderer;

        [Header("Smash Visuals")]
        [SerializeField] private AnimationCurve smashColorCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float smashColorChangeTime;

        [Header("Pool Selection")]
        [SerializeField] private string poolId;
        [SerializeField] private string bounceId;

        private ValueAnimator smashColorChangeAnim;
        private MeshRenderer meshRenderer;
        private PlayerTheme theme;

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
            if (!hitObj) { throw new ArgumentNullException(nameof(hitObj), $"[{nameof(PlayerVisuals)}] {nameof(BounceEffect)} failed: {nameof(hitObj)} is null."); }

            GameObject bounceVfx = RentBounceVfxObj(hitObj, out float duration);
            bounceVfx.transform.position = transform.position;
            StartCoroutine(ReturnBounceVfxToPool(bounceVfx, duration));
        }

        private GameObject RentBounceVfxObj(Transform hitObj, out float vfxDuration) {
            GameObject vfx = PoolApi.Rent(poolId, bounceId, hitObj);

            if (!vfx.TryGetComponent(out ParticleSystem ps)) {
                throw new MissingComponentException(
                    $"[{nameof(PlayerVisuals)}] {nameof(RentBounceVfxObj)} failed: rented VFX is missing {nameof(ParticleSystem)}."
                );
            }
            vfxDuration = ps.main.duration + ps.main.startLifetime.constantMax;
            ParticleSystem.MainModule main = ps.main;
            main.startColor = theme.BounceVfxColor;

            return vfx;
        }

        private static IEnumerator ReturnBounceVfxToPool(GameObject vfx, float returnInSeconds) {
            yield return new WaitForSeconds(returnInSeconds);
            if (vfx) { PoolApi.Return(vfx); }
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