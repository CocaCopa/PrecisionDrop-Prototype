using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.Input.Contracts;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    internal sealed class TowerController : MonoBehaviour {
        [SerializeField] private GameObject towerObj;

        [Space(10f)]
        [SerializeField] private Transform centralCylinder;
        [SerializeField] private float offsetOnPassPlatform;

        [Space(10f)]
        [SerializeField] [Min(0f)] private float rotationMultiplier = 1f;
        [SerializeField] [Range(0.1f, 1f)] private float deltaCompressionExponent = 0.75f;
        [SerializeField] [Min(0f)] private float maxRotationStep = 12f;

        private IInputSource inputSource;
        private IGameFlow gameFlow;

        internal void Install(IInputSource inputSourceRef, IGameFlow gameFlowRef) {
            inputSource = inputSourceRef;
            gameFlow = gameFlowRef;
        }

        public void Init() {
            gameFlow.OnPlayerPassedPlatform += GameFlow_OnPlayerPassedPlatform;
        }

        private void GameFlow_OnPlayerPassedPlatform() {
            centralCylinder.position += Vector3.down * offsetOnPassPlatform;
        }

        private void Update() {
            RotateTower();
        }

        private void RotateTower() {
            if (!inputSource.IsHolding) { return; }

            float rawDelta = inputSource.MouseDragDelta.x;
            float adjustedDelta = CompressDelta(rawDelta) * rotationMultiplier;
            adjustedDelta = Mathf.Clamp(adjustedDelta, -maxRotationStep, maxRotationStep);

            Vector3 towerEuler = towerObj.transform.localEulerAngles;
            towerEuler.y += adjustedDelta;
            towerObj.transform.localEulerAngles = towerEuler;
        }

        private float CompressDelta(float delta) {
            if (Mathf.Approximately(delta, 0f)) { return 0f; }

            float sign = Mathf.Sign(delta);
            float magnitude = Mathf.Abs(delta);
            float compressedMagnitude = Mathf.Pow(magnitude, deltaCompressionExponent);

            return sign * compressedMagnitude;
        }
    }
}