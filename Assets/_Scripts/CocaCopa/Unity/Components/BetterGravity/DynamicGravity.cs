using System;
using CocaCopa.Core.Animation;
using CocaCopa.Unity.Components.BetterGravity;
using UnityEngine;

namespace CocaCopa.Unity.Components {
    [DisallowMultipleComponent, RequireComponent(typeof(Rigidbody))]
    public class DynamicGravity : MonoBehaviour {
        [SerializeField] private GravitySource source;
        [SerializeField] private Vector3 localGravity = new Vector3(0f, -9.81f, 0f);
        [SerializeField, Min(0f)] private float maxFallSpeed;
        [SerializeField] private ScaleSettings gravityUp = new ScaleSettings(AnimationCurve.Linear(0f, 0f, 1f, 1f), 0f, 0f);
        [SerializeField] private ScaleSettings gravityDown = new ScaleSettings(AnimationCurve.Linear(0f, 0f, 1f, 1f), 0f, 0f);
        [SerializeField] float upMultiplier;
        [SerializeField] float downMultiplier;

        private Rigidbody playerRb;
        private ValueAnimator upAnimator;
        private ValueAnimator downAnimator;

        private Vector3 GravityVector {
            get => source switch {
                GravitySource.Global => Physics.gravity,
                GravitySource.Local => localGravity,
                _ => throw new NotImplementedException()
            };
        }
        private float UpOffset => gravityUp.curveOffset / 100f;
        private float DownOffset => gravityDown.curveOffset / 100f;
        private Vector3 LinearVelocity { get => playerRb.linearVelocity; set => playerRb.linearVelocity = value; }
        private float GravityMultiplier => Vector3.Dot(LinearVelocity, GravityVector) < 0f ? upMultiplier : downMultiplier;

        private Vector3 GravityDir => GravityVector.normalized;
        private float FallSpeed => Vector3.Dot(LinearVelocity, GravityDir);

#if UNITY_EDITOR
        internal void UpdateScaleSettings_EditorOnly() => CreateGravityAnimators();
#endif

        private void Awake() {
            playerRb = GetComponent<Rigidbody>();
            CreateGravityAnimators();
        }

        private void CreateGravityAnimators() {
            upAnimator = ValueAnimator.ByDuration(0f, 1f, gravityUp.duration, new Easing(gravityUp.curve));
            downAnimator = ValueAnimator.ByDuration(0f, 1f, gravityDown.duration, new Easing(gravityDown.curve));
            downAnimator.SetProgress(DownOffset);
            upAnimator.SetProgress(UpOffset);
        }

        private void FixedUpdate() {
            UpdateAccelMultipliers();

            Vector3 accel = CalculateGravityAccel();
            playerRb.AddForce(accel, ForceMode.Acceleration);
        }

        private void UpdateAccelMultipliers() {
            if (Vector3.Dot(LinearVelocity, GravityVector) < 0f) {
                upMultiplier = upAnimator.EvaluateUnclamped(Time.fixedDeltaTime);
                if (downAnimator.Progress != DownOffset) { downAnimator.SetProgress(DownOffset); }
            }
            else {
                downMultiplier = downAnimator.EvaluateUnclamped(Time.fixedDeltaTime);
                if (upAnimator.Progress != UpOffset) { upAnimator.SetProgress(UpOffset); }
            }
        }

        private Vector3 CalculateGravityAccel() {
            Vector3 gravityAccel = GravityVector * GravityMultiplier;

            if (maxFallSpeed > 0f) {
                if (FallSpeed >= maxFallSpeed) {
                    gravityAccel = Vector3.zero;
                }
                else {
                    float accelAlongGravity = Vector3.Dot(gravityAccel, GravityDir);
                    float predicted = FallSpeed + accelAlongGravity * Time.fixedDeltaTime;

                    if (predicted > maxFallSpeed && accelAlongGravity > 0f) {
                        float allowedDeltaV = maxFallSpeed - FallSpeed;
                        float allowedAccelAlong = allowedDeltaV / Time.fixedDeltaTime;
                        gravityAccel = GravityDir * allowedAccelAlong;
                    }
                }
            }

            return gravityAccel;
        }

        private class Easing : IEasing {
            private readonly AnimationCurve curve;
            public float Evaluate(float t) => curve.Evaluate(t);
            public Easing(AnimationCurve curve) => this.curve = curve;
        }
    }
}
