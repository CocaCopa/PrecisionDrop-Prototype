using System.Collections;
using UnityEngine;

namespace CocaCopa.Unity.Components {
    public class PulseAnimation : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private bool playOnAwake = true;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private float speed = 1f;
        [SerializeField] private float loopDelay = 0.5f;

        private RectTransform myRect;

        private float time;
        private float duration;
        private bool isDelaying;
        private bool isPlaying;

        private void Awake() {
            myRect = GetComponent<RectTransform>();
            duration = scaleCurve.length > 0
                ? scaleCurve.keys[^1].time
                : 1f;

            myRect.localScale = Vector3.one * scaleCurve.Evaluate(0f);

            if (playOnAwake) { Play(); }
        }

        private void Update() {
            if (!isPlaying) { return; }
            ProgressAnim();
        }

        public void Play() {
            if (isPlaying) { return; }

            isPlaying = true;
            time = 0f;
        }

        public void Stop() {
            if (!isPlaying) { return; }

            isPlaying = false;
            isDelaying = false;
            StopAllCoroutines();

            if (myRect) { myRect.localScale = Vector3.one * scaleCurve.Evaluate(0f); }
        }

        private void ProgressAnim() {
            if (!myRect || isDelaying) { return; }

            time += Time.deltaTime * speed;

            float pingPongTime = Mathf.PingPong(time, duration);
            float scale = scaleCurve.Evaluate(pingPongTime);

            myRect.localScale = Vector3.one * scale;

            if (time >= duration * 2f) {
                time = 0f;
                StartCoroutine(DelayLoop());
            }
        }

        private IEnumerator DelayLoop() {
            isDelaying = true;
            yield return new WaitForSeconds(loopDelay);
            isDelaying = false;
        }
    }
}