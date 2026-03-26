using System;
using PrecisionDrop.UserInterface.Gameplay;
using PrecisionDrop.UserInterface.Screens;
using UnityEngine;

namespace PrecisionDrop.App.Unity {
    internal sealed class UiBootstrapper : MonoBehaviour {
        [SerializeField] private LoseScreenUI loseScreenUI;
        [SerializeField] private ScoreUI scoreUI;
        [SerializeField] private ComboUI comboUI;

        private SessionFlowController sessionFlowController;

        private void Awake() {
            ValidateSceneWiring();
        }

        internal void Install(AppBootstrapper ab) {
            scoreUI.Install(ab.GameFlowRef, ab.ScoreRef);
            comboUI.Install(ab.ComboRef);
            loseScreenUI.Install(ab.InputSourceRef, ab.GameFlowRef, ab.ScoreRef);

            sessionFlowController = new SessionFlowController();
            sessionFlowController.Install(loseScreenUI);
        }

        internal void Init() {
            scoreUI.Init();
            comboUI.Init();
            loseScreenUI.Init();
            sessionFlowController.Init();
        }

        private void ValidateSceneWiring() {
            if (!scoreUI) { throw new NullReferenceException(Msg(nameof(scoreUI))); }
            if (!comboUI) { throw new NullReferenceException(Msg(nameof(comboUI))); }
            if (!loseScreenUI) { throw new NullReferenceException(Msg(nameof(loseScreenUI))); }
        }

        private string Msg(string fieldName) {
            return $"[{nameof(AppBootstrapper)}] Missing reference: {fieldName} on '{name}'.";
        }
    }
}