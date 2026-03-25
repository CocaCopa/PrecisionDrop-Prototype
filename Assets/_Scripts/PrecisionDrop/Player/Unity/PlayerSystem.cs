using System;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.Input.Contracts;
using PrecisionDrop.Player.Contracts;
using PrecisionDrop.Player.Unity.Presentation;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    public class PlayerSystem : MonoBehaviour {
        [SerializeField] private PlayerConfigAsset playerConfig;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private TowerController towerController;
        [SerializeField] private PlayerSphere playerSphere;
        [SerializeField] private PlayerVisuals playerVisuals;

        private bool installed;
        private bool initialized;

        public PlayerAccess PlayerApi => new(playerSphere, playerSphere, playerSphere);
        public PlayerConfigAsset PlayerConfig => playerConfig;

        private void Awake() {
            if (playerConfig == null) { throw NullReferenceException(nameof(playerConfig)); }
            if (cameraController == null) { throw NullReferenceException(nameof(cameraController)); }
            if (towerController == null) { throw NullReferenceException(nameof(towerController)); }
            if (playerVisuals == null) { throw NullReferenceException(nameof(playerVisuals)); }
            if (playerSphere == null) { throw NullReferenceException(nameof(playerSphere)); }
        }

        public void Install(IInputSource inputSource, IGameFlow gameFlow, PlayerTheme theme) {
            if (installed) { throw new InvalidOperationException($"[{nameof(PlayerSystem)}] {nameof(Install)}() called twice."); }
            if (gameFlow is null) { throw new ArgumentNullException(nameof(gameFlow)); }

            installed = true;
            playerSphere.Install(playerConfig.JumpStrength);
            cameraController.Install(gameFlow);
            towerController.Install(inputSource, gameFlow);
            playerVisuals.Install(theme);
        }

        public void Init() {
            if (!installed) { throw new InvalidOperationException($"[{nameof(PlayerSystem)}] {nameof(Init)}() called before {nameof(Install)}()."); }

            if (initialized) {
                Debug.LogWarning($"[{nameof(PlayerSystem)}] Already initialized.");
                return;
            }

            initialized = true;
            cameraController.Init();
            towerController.Init();
            playerVisuals.ApplyTheme();
        }

        private NullReferenceException NullReferenceException(string fieldName) {
            return new NullReferenceException(
                $"[{nameof(PlayerSystem)}] {fieldName} is not assigned."
            );
        }
    }
}