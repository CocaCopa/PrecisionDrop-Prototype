using System;

namespace PrecisionDrop.GameFlow.Contracts {
    public interface IGameFlow {
        event Action OnPlayerPassedPlatform;
        event Action OnPlayerBounced;
    }
}