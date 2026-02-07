using System.Collections;
using CocaCopa.Core.Animation;
using PrecisionDrop.GameFlow.Contracts;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    internal sealed class CameraController : MonoBehaviour {
        [SerializeField] private Transform camTransform;
        [Space(10f)]
        [SerializeField] private float moveOffset;
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float moveSpeed;
        private IGameFlow gameFlow;

        private Coroutine moveCoroutine;
        private ValueAnimator moveAnimator;

        private Vector3 lastTargetPos;

        internal void Install(IGameFlow gameFlow) {
            this.gameFlow = gameFlow;
        }

        internal void Init() {
            gameFlow.OnPlayerPassedPlatform += GameFlow_OnPlayerPassedPlatform;
            moveAnimator = ValueAnimator.BySpeed(0f, 1f, moveSpeed, new Easing(moveCurve));
            lastTargetPos = camTransform.position;
        }

        private void GameFlow_OnPlayerPassedPlatform() {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveRoutine());
        }

        private IEnumerator MoveRoutine() {
            Vector3 startPos = camTransform.position;
            Vector3 targetPos = lastTargetPos + Vector3.down * moveOffset;
            lastTargetPos = targetPos;
            moveAnimator.ResetAnimator();
            yield return null;
            while (!moveAnimator.IsComplete) {
                float t = moveAnimator.Evaluate(Time.deltaTime);
                camTransform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            camTransform.position = targetPos;
        }
        
        private class Easing : IEasing {
            private readonly AnimationCurve curve;
            public float Evaluate(float t) => curve.Evaluate(t);
            public Easing(AnimationCurve curve) => this.curve = curve;
        }
    }
}
