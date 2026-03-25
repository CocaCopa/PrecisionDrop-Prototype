using System;

namespace PrecisionDrop.UserInterface.Screens.Contracts {
    public interface ILoseScreen {
        event Action OnPlayerRequestedRestart;
    }
}