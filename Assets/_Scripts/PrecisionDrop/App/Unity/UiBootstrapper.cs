using PrecisionDrop.SessionTracker.Contracts;
using PrecisionDrop.UserInterface.Gameplay;
using UnityEngine;

namespace PrecisionDrop.App.Unity {
    internal sealed class UiBootstrapper : MonoBehaviour {
        [SerializeField] private ScoreUI scoreUI;

        internal void Install(IScore score) {
            scoreUI.Install(score);
        }

        internal void Init() {
            scoreUI.Init();
        }
    }
}