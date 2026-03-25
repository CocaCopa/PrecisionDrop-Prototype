using CocaCopa.SceneManagement;
using PrecisionDrop.UserInterface.Screens.Contracts;

namespace PrecisionDrop.App.Unity {
    internal class SessionFlowController {
        private ILoseScreen loseScreen;

        internal void Install(ILoseScreen loseScreenRef) {
            loseScreen = loseScreenRef;
        }

        internal void Init() {
            loseScreen.OnPlayerRequestedRestart += LoseScreen_OnPlayerRequestedRestart;
        }

        private void LoseScreen_OnPlayerRequestedRestart() {
            SceneTransitionApi.TransitionToScene(0, LoadMode.Single);
        }
    }
}