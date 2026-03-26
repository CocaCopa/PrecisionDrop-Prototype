using UnityEngine;

namespace CocaCopa.Unity.Components {
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class PopAnimation : MonoBehaviour {
        private enum State {
            Idle,
            Pop,
            DelayBeforeFade,
            Fade
        }

        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] [Range(0f, 2f)] private float startScaleMultiplier = 0.5f;
        [SerializeField] [Min(0.01f)] private float scaleSpeed = 1f;

        [SerializeField] private float delayBeforeFade = 0.5f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        [SerializeField] [Min(0.01f)] private float fadeSpeed = 1f;

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        private Vector3 defaultScale;
        private Vector3 fromScale;

        private float scaleTime;
        private float scaleDuration;

        private float fadeTime;
        private float fadeDuration;

        private float delayTimer;

        private State currentState;

        private void Awake() {
            Init();
            gameObject.SetActive(false);
            enabled = false;
        }

        private void Init() {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            defaultScale = rectTransform.localScale;

            scaleDuration = scaleCurve.length > 0
                ? scaleCurve.keys[^1].time
                : 1f;

            fadeDuration = fadeCurve.length > 0
                ? fadeCurve.keys[^1].time
                : 1f;
        }

        private void Update() {
            StateUpdate();
        }

        private void StateUpdate() {
            switch (currentState) {
                case State.Pop:
                    UpdatePop();
                    break;

                case State.DelayBeforeFade:
                    UpdateDelay();
                    break;

                case State.Fade:
                    UpdateFade();
                    break;
            }
        }

        private void UpdatePop() {
            scaleTime += Time.deltaTime * scaleSpeed;

            float t = Mathf.Clamp01(scaleTime / scaleDuration);
            float curveValue = scaleCurve.Evaluate(t);

            rectTransform.localScale = Vector3.LerpUnclamped(fromScale, defaultScale, curveValue);

            if (t >= 1f) {
                rectTransform.localScale = defaultScale;
                currentState = State.DelayBeforeFade;
                delayTimer = 0f;
            }
        }

        private void UpdateDelay() {
            delayTimer += Time.deltaTime;

            if (!(delayTimer >= delayBeforeFade)) { return; }

            fadeTime = 0f;
            currentState = State.Fade;
        }

        private void UpdateFade() {
            fadeTime += Time.deltaTime * fadeSpeed;

            float t = Mathf.Clamp01(fadeTime / fadeDuration);
            float alpha = fadeCurve.Evaluate(t);

            canvasGroup.alpha = alpha;

            if (t >= 1f) {
                currentState = State.Idle;
                enabled = false;
                gameObject.SetActive(false);
            }
        }

        public void Play() {
            scaleTime = 0f;
            fadeTime = 0f;
            delayTimer = 0f;

            float multiplier = 1f + startScaleMultiplier;
            fromScale = defaultScale * multiplier;

            rectTransform.localScale = fromScale;
            canvasGroup.alpha = 1f;

            currentState = State.Pop;
            gameObject.SetActive(true);
            enabled = true;
        }
    }
}