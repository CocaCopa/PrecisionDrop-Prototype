using System;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.Player.Contracts;
using PrecisionDrop.Player.Unity.Presentation;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    public class PlayerSystem : MonoBehaviour {
        [SerializeField] private CameraController cameraController;
        [SerializeField] private TowerController towerController;
        [SerializeField] private PlayerSphere playerSphere;
        [SerializeField] private PlayerVisuals playerVisuals;
        
        private bool installed;
        private bool initialized;
        
        public IPlayerSphere PlayerApi => (playerSphere as IPlayerSphere) ?? throw new NullReferenceException($"[{nameof(PlayerSystem)}] {nameof(IPlayerSphere)}");

        private void Awake() {
            if (cameraController == null) { throw new NullReferenceException($"[{nameof(PlayerSystem)}] {nameof(cameraController)} is not assigned."); }
            if (towerController == null) { throw new NullReferenceException($"[{nameof(PlayerSystem)}] {nameof(towerController)} is not assigned."); }
            if (playerVisuals == null) { throw new NullReferenceException($"[{nameof(PlayerSystem)}] {nameof(playerVisuals)} is not assigned."); }
            if (playerSphere == null) { throw new NullReferenceException($"[{nameof(PlayerSystem)}] {nameof(playerSphere)} is not assigned."); }
        }

        public void Install(IGameFlow gameFlow, PlayerTheme theme) {
            if (installed) { throw new InvalidOperationException($"[{nameof(PlayerSystem)}] {nameof(Install)}() called twice."); }
            if (gameFlow is null) { throw new ArgumentNullException($"[{nameof(PlayerSystem)}] {nameof(gameFlow)}"); }
            if (!theme.IsValid) { throw new ArgumentNullException($"[{nameof(PlayerSystem)}] {nameof(theme)}"); }

            installed = true;

            cameraController.Install(gameFlow);
            towerController.Install(gameFlow);
            playerVisuals.Install(theme);
        }


        public void Init() {
            if (!installed) { throw new InvalidOperationException($"[{nameof(PlayerSystem)}] {nameof(Init)}() called before {nameof(Install)}()."); }
            if (initialized) { Debug.LogWarning($"[{nameof(PlayerSystem)}] Already initialized."); return; }
        
            initialized = true;
            cameraController.Init();
            towerController.Init();
            playerVisuals.ApplyTheme();
        }
    }
}