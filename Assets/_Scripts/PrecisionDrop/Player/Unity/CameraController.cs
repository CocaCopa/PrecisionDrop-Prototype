using PrecisionDrop.GameFlow.Contracts;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    internal sealed class CameraController : MonoBehaviour {
        private IGameFlow gameFlow;

        internal void Install(IGameFlow gameFlow) {
            this.gameFlow = gameFlow;
        }

        internal void Init() {
            gameFlow.OnPlayerPassedPlatform += GameFlow_OnPlayerPassedPlatform;
        }

        private void GameFlow_OnPlayerPassedPlatform() {
            Debug.Log("Camera should move");
        }
    }
}
