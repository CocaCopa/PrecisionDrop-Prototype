using System;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.Input.Contracts;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    internal sealed class TowerController : MonoBehaviour {
        [SerializeField] private GameObject unityInput;
        [SerializeField] private GameObject towerObj;
        [Space(10f)]
        [SerializeField] private Transform centralCylinder;
        [SerializeField] private float offsetOnPassPlatform;

        private IInputSource InputSource { get; set; }
        private IGameFlow gameFlow;

        internal void Install(IGameFlow gameFlow) {
            this.gameFlow = gameFlow;
        } 

        public void Init() {
            if (!unityInput.TryGetComponent<IInputSource>(out var source)) { throw new Exception($"GameObject '{unityInput.name}' does not contain a component implementing {nameof(IInputSource)}"); }
            InputSource = source;
            
            gameFlow.OnPlayerPassedPlatform += GameFlow_OnPlayerPassedPlatform;
        }

        private void GameFlow_OnPlayerPassedPlatform() {
            centralCylinder.position += Vector3.down * offsetOnPassPlatform;
        }

        private void LateUpdate() {
            if (!InputSource.IsHolding) { return; }

            Vector3 towerEuler = towerObj.transform.localEulerAngles;
            towerEuler.y += InputSource.MouseDragDelta.x;
            towerObj.transform.localEulerAngles = towerEuler;
        }
    }
}
