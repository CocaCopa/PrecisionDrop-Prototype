using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.Input.Contracts;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    internal sealed class TowerController : MonoBehaviour {
        [SerializeField] private GameObject towerObj;
        [Space(10f)]
        [SerializeField] private Transform centralCylinder;
        [SerializeField] private float offsetOnPassPlatform;

        private IInputSource inputSource;
        private IGameFlow gameFlow;

        internal void Install(IInputSource inputSourceRef, IGameFlow gameFlowRef) {
            inputSource = inputSourceRef;
            gameFlow = gameFlowRef;
        }

        public void Init() {
            gameFlow.OnPlayerPassedPlatform += GameFlow_OnPlayerPassedPlatform;
            gameFlow.OnPlayerHitDanger += GameFlow_OnPlayerHitDanger;
        }

        private void GameFlow_OnPlayerHitDanger() {
            // enabled = false;
        }

        private void GameFlow_OnPlayerPassedPlatform() {
            centralCylinder.position += Vector3.down * offsetOnPassPlatform;
        }

        private void LateUpdate() {
            RotateTower();
        }

        private void RotateTower() {
            Vector3 towerEuler = towerObj.transform.localEulerAngles;
            towerEuler.y += inputSource.MouseDragDelta.x;
            towerObj.transform.localEulerAngles = towerEuler;
        }
    }
}