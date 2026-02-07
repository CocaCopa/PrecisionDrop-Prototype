using System;
using PrecisionDrop.Input.Contracts;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    internal sealed class TowerRotator : MonoBehaviour {
        [SerializeField] private GameObject unityInput;
        [SerializeField] private GameObject towerObj;

        private IInputSource InputSource { get; set; }

        private void Awake() {
            if (!unityInput.TryGetComponent<IInputSource>(out var source)) { throw new Exception($"GameObject '{unityInput.name}' does not contain a component implementing {nameof(IInputSource)}"); }
            InputSource = source;
        }

        private void LateUpdate() {
            if (!InputSource.IsHolding) { return; }

            Vector3 towerEuler = towerObj.transform.localEulerAngles;
            towerEuler.y += InputSource.MouseDragDelta.x;
            towerObj.transform.localEulerAngles = towerEuler;
        }
    }
}
