using System;
using PrecisionDrop.GameFlow.Contracts;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    public class PlayerSystem : MonoBehaviour {
        [SerializeField] private CameraController cameraController;
        
        private bool installed;
        private bool initialized;

        private void Awake() {
            if (cameraController == null) { throw new NullReferenceException($"[{nameof(PlayerSystem)}] {nameof(cameraController)} is not assigned."); }
        }

        public void Install(IGameFlow gameFlow) {
            if (cameraController == null) { return; }
            if (installed) { throw new InvalidOperationException($"[{nameof(PlayerSystem)}] {nameof(Install)}() called twice."); }

            installed = true;
            cameraController.Install(gameFlow);
        }

        public void Init() {
            if (!installed) { throw new InvalidOperationException($"[{nameof(PlayerSystem)}] {nameof(Init)}() called before {nameof(Install)}()."); }
            if (initialized) { Debug.LogWarning($"[{nameof(PlayerSystem)}] Already initialized."); return; }
        
            initialized = true;
            cameraController.Init();
        }
    }
}