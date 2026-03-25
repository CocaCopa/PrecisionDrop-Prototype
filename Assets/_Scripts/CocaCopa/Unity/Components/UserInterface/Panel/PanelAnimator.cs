using System;
using CocaCopa.Core.Animation;
using CocaCopa.Unity.Components.Panel;
using UnityEngine;

namespace CocaCopa.Unity.Components {
    [RequireComponent(typeof(RectTransform))]
    public class PanelAnimator : MonoBehaviour {
        [Header("Visibility")]
        [SerializeField] private bool startHidden = true;
        [SerializeField] private bool disableWhenHidden = true;

        [Header("Options")]
        [SerializeField] private AnimOptions animOptions;
        [SerializeField] private HideOffsets hideOffsets;

        [Header("Animation")]
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float moveSpeed = 1.5f;

        private RectTransform panel;
        private RectPositions hiddenPositions;
        private IEasing easing;
        private ValueAnimator valueAnimator;
        private Vector2 from;
        private Vector2 to;
        private bool isHiding;
        private bool positionsInitialized;

        public bool IsAnimating => valueAnimator != null && !valueAnimator.IsComplete;

        public void OverrideAnimOptions(AnimOptions opt) {
            animOptions = opt;
        }

        private void Awake() {
            easing = new CurveEasing(moveCurve);
            valueAnimator = ValueAnimator.BySpeed(0f, 1f, moveSpeed, easing);
            panel = GetComponent<RectTransform>();
        }

        private void Start() {
            EnsurePositionsInitialized();

            if (startHidden) {
                panel.anchoredPosition = GetHiddenPosition(hiddenPositions, false);
                valueAnimator.SetProgress(1f);
                isHiding = true;

                if (disableWhenHidden) { gameObject.SetActive(false); }
            }

            enabled = false;
        }

        private void CalcPositions() {
            Vector2 visible = panel.anchoredPosition;

            float panelHeight = panel.rect.height;
            float panelWidth = panel.rect.width;

            Vector2 hiddenTop = visible + Vector2.up * (panelHeight + hideOffsets.top);
            Vector2 hiddenBottom = visible + Vector2.down * (panelHeight + hideOffsets.bottom);
            Vector2 hiddenLeft = visible + Vector2.left * (panelWidth + hideOffsets.left);
            Vector2 hiddenRight = visible + Vector2.right * (panelWidth + hideOffsets.right);

            hiddenPositions = new RectPositions(visible, hiddenTop, hiddenBottom, hiddenLeft, hiddenRight);
            positionsInitialized = true;
        }

        private void EnsurePositionsInitialized() {
            if (positionsInitialized) { return; }

            Canvas.ForceUpdateCanvases();
            CalcPositions();
        }

        private void Update() {
            AnimatePanel();
            DisableWhenComplete();
        }

        private void AnimatePanel() {
            float t = valueAnimator.EvaluateUnclamped(Time.unscaledDeltaTime);
            panel.anchoredPosition = Vector2.LerpUnclamped(from, to, t);
        }

        private void DisableWhenComplete() {
            if (!valueAnimator.IsComplete) { return; }

            enabled = false;

            if (disableWhenHidden && isHiding) { gameObject.SetActive(false); }
        }

        public void Show() {
            EnsurePositionsInitialized();

            gameObject.SetActive(true);
            isHiding = false;

            valueAnimator.ResetAnimator();
            from = GetHiddenPosition(hiddenPositions, true);
            to = hiddenPositions.visible;

            enabled = true;
        }

        public void Hide() {
            EnsurePositionsInitialized();

            isHiding = true;

            valueAnimator.ResetAnimator();
            from = hiddenPositions.visible;
            to = GetHiddenPosition(hiddenPositions, false);

            enabled = true;
        }

        public void ShowInstant() {
            EnsurePositionsInitialized();

            gameObject.SetActive(true);
            isHiding = false;

            valueAnimator.ResetAnimator();
            valueAnimator.SetProgress(1f);

            panel.anchoredPosition = hiddenPositions.visible;

            enabled = false;
        }

        public void HideInstant() {
            EnsurePositionsInitialized();

            isHiding = true;

            valueAnimator.ResetAnimator();
            valueAnimator.SetProgress(1f);

            panel.anchoredPosition = GetHiddenPosition(hiddenPositions, false);

            enabled = false;

            if (disableWhenHidden) { gameObject.SetActive(false); }
        }

        private Vector2 GetHiddenPosition(RectPositions positions, bool isAppearing) {
            if (isAppearing) {
                return animOptions.appear switch {
                    UIAppear.Left => positions.hiddenLeft,
                    UIAppear.Right => positions.hiddenRight,
                    UIAppear.Top => positions.hiddenTop,
                    UIAppear.Bottom => positions.hiddenBottom,
                    _ => positions.hiddenLeft
                };
            }

            return animOptions.disappear switch {
                UIDisappear.Left => positions.hiddenLeft,
                UIDisappear.Right => positions.hiddenRight,
                UIDisappear.Top => positions.hiddenTop,
                UIDisappear.Bottom => positions.hiddenBottom,
                _ => positions.hiddenLeft
            };
        }

        private sealed class CurveEasing : IEasing {
            private readonly AnimationCurve curve;

            public CurveEasing(AnimationCurve curve) {
                this.curve = curve;
            }

            public float Evaluate(float t) {
                return curve.Evaluate(t);
            }
        }

        [Serializable]
        private struct HideOffsets {
            public float top;
            public float bottom;
            public float left;
            public float right;

            public HideOffsets(float top, float bottom, float left, float right) {
                this.top = top;
                this.bottom = bottom;
                this.left = left;
                this.right = right;
            }
        }
    }
}