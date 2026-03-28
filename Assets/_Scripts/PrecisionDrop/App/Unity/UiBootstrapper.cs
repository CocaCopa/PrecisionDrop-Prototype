using System;
using PrecisionDrop.UserInterface.Gameplay;
using PrecisionDrop.UserInterface.Screens;
using UnityEngine;

namespace PrecisionDrop.App.Unity {
    internal sealed class UiBootstrapper : MonoBehaviour {
        [SerializeField] private LoseScreenUI loseScreenUI;
        [SerializeField] private ScoreUI scoreUI;
        [SerializeField] private WorldScorePopup worldScorePopup;
        [SerializeField] private ComboUI comboUI;

        private SessionFlowController sessionFlowController;
        private bool installed;
        private bool initialized;

        private void Awake() {
            ValidateSceneWiring();
        }

        internal void Install(UiAccess access) {
            if (installed) { throw new InvalidOperationException($"[{nameof(UiBootstrapper)}] {nameof(Install)}() called twice."); }

            installed = true;

            scoreUI.Install(access.GameFlow, access.Score);
            worldScorePopup.Install(access.Score);
            comboUI.Install(access.Combo);
            loseScreenUI.Install(access.Input, access.GameFlow, access.Score);

            sessionFlowController = new SessionFlowController();
            sessionFlowController.Install(loseScreenUI);
        }

        internal void Init() {
            if (!installed) { throw new InvalidOperationException($"[{nameof(UiBootstrapper)}] {nameof(Init)}() called before {nameof(Install)}()."); }

            if (initialized) {
                Debug.LogWarning($"[{nameof(UiBootstrapper)}] Already initialized.");
                return;
            }

            initialized = true;

            scoreUI.Init();
            worldScorePopup.Init();
            comboUI.Init();
            loseScreenUI.Init();
            sessionFlowController.Init();
        }

        private void ValidateSceneWiring() {
            if (!scoreUI) { throw new NullReferenceException(Msg(nameof(scoreUI))); }
            if (!worldScorePopup) { throw new NullReferenceException(Msg(nameof(worldScorePopup))); }
            if (!comboUI) { throw new NullReferenceException(Msg(nameof(comboUI))); }
            if (!loseScreenUI) { throw new NullReferenceException(Msg(nameof(loseScreenUI))); }
        }

        private string Msg(string fieldName) {
            return $"[{nameof(UiBootstrapper)}] Missing reference: {fieldName} on '{name}'.";
        }
    }
}