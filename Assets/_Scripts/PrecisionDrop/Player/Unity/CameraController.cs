using CocaCopa.Core.Animation;
using PrecisionDrop.GameFlow.Contracts;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    internal sealed class CameraController : MonoBehaviour {
        [SerializeField] private Transform camTransform;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float verticalOffset;
        [SerializeField] private float followSpeed = 10f;

        private IGameFlow gameFlow;

        private float moveDuration;
        private float elapsed;
        private bool isMoving;

        internal void Install(IGameFlow gameFlow) {
            this.gameFlow = gameFlow;
        }

        internal void Init() {
            Vector3 camStartPos = camTransform.position;
            camStartPos.y = playerTransform.position.y + verticalOffset;

            camTransform.position = camStartPos;
        }

        private void LateUpdate() {
            float desiredY = playerTransform.position.y + verticalOffset;

            if (!(desiredY < camTransform.position.y)) { return; }

            float newY = Mathf.MoveTowards(
                camTransform.position.y,
                desiredY,
                followSpeed * Time.deltaTime
            );

            Vector3 pos = camTransform.position;
            pos.y = newY;
            camTransform.position = pos;
        }
    }
}