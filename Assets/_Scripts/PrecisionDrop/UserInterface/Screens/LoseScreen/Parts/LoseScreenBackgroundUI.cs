using UnityEngine;
using UnityEngine.UI;

namespace PrecisionDrop.UserInterface.Screens {
    internal sealed class LoseScreenBackgroundUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Image backgroundImg;

        [Header("Fade Settings")]
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float fadeSpeed = 1f;

        private float time;
        private float duration;
        private bool isFading;
        private bool fadeIn;
        private float targetAlpha;

        private Color cachedColor;

        private void Awake() {
            duration = fadeCurve.length > 0
                ? fadeCurve.keys[^1].time
                : 1f;

            cachedColor = backgroundImg.color;
            targetAlpha = cachedColor.a;

            HideInstant();
        }

        private void Update() {
            ProgressAnim();
        }

        private void ProgressAnim() {
            if (!isFading) { return; }

            time += Time.deltaTime * fadeSpeed;

            float t = Mathf.Clamp01(time / duration);
            float value = fadeCurve.Evaluate(t);

            float alpha = fadeIn
                ? Mathf.LerpUnclamped(0f, targetAlpha, value)
                : Mathf.LerpUnclamped(targetAlpha, 0f, value);

            SetAlpha(alpha);

            if (t >= 1f) {
                isFading = false;

                if (!fadeIn) { backgroundImg.gameObject.SetActive(false); }
            }
        }

        public void FadeIn() {
            StartFade(true);
        }

        public void FadeOut() {
            StartFade(false);
        }

        public void HideInstant() {
            isFading = false;
            time = 0f;
            SetAlpha(0f);
            backgroundImg.gameObject.SetActive(false);
        }

        private void StartFade(bool isFadeIn) {
            fadeIn = isFadeIn;
            time = 0f;
            isFading = true;

            if (fadeIn) { backgroundImg.gameObject.SetActive(true); }
        }

        private void SetAlpha(float alpha) {
            cachedColor.a = alpha;
            backgroundImg.color = cachedColor;
        }
    }
}