using System;
using UnityEngine;

namespace CocaCopa.Unity.Components {
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class MoveAndFade : MonoBehaviour {
        [Header("Animation")]
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] [Min(0.01f)] private float animationSpeed = 1f;
        [SerializeField] private Vector3 moveDirection = Vector3.up * 50f;

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        private Vector3 defaultPosition;
        private float duration;
        private float time;

        public bool IsPlaying { get; private set; }

        public event Action OnAnimationCompleted;

        private void Awake() {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            defaultPosition = rectTransform.localPosition;
            duration = GetCurveDuration();
        }

        private void Update() {
            ProgressAnim();
        }

        private void ProgressAnim() {
            if (!IsPlaying) { return; }

            time += Time.deltaTime * animationSpeed;

            float t = duration <= 0f
                ? 1f
                : Mathf.Clamp01(time / duration);

            float curveValue = animationCurve.Evaluate(t);

            rectTransform.localPosition = defaultPosition + moveDirection * curveValue;
            canvasGroup.alpha = 1f - curveValue;

            if (t >= 1f) { Complete(); }
        }

        public void Play() {
            gameObject.SetActive(true);

            time = 0f;
            duration = GetCurveDuration();
            IsPlaying = true;

            rectTransform.localPosition = defaultPosition;
            canvasGroup.alpha = 1f;
        }

        private void Complete() {
            if (!IsPlaying) { return; }

            IsPlaying = false;
            OnAnimationCompleted?.Invoke();
        }

        private float GetCurveDuration() {
            if (animationCurve == null || animationCurve.length == 0) { return 1f; }

            return animationCurve.keys[^1].time;
        }
    }
}