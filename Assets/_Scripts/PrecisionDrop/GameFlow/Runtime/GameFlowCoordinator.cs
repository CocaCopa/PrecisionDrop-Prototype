using System;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.Platforms.Contracts;
using PrecisionDrop.Player.Contracts;

namespace PrecisionDrop.GameFlow.Runtime {
    internal sealed class GameFlowCoordinator : IGameFlow {
        private readonly IPlayerSphere playerSphere;
        private readonly IPlatformEventBus platformEventBus;

        public event Action OnPlayerPassedPlatform;
        public event Action OnPlayerBounced;

        private int passCounter;
    
        internal GameFlowCoordinator(IPlayerSphere playerSphere, IPlatformEventBus platformEventBus) {
            this.playerSphere = playerSphere;
            this.platformEventBus = platformEventBus;
        }

        internal void Init() {
            platformEventBus.OnPlatformCollision += PlatformEventBus_OnPlatformCollision;
            platformEventBus.OnPlatformPassed += PlatformEventBus_OnPlatformPassed;
        }

        private void PlatformEventBus_OnPlatformCollision() {
            playerSphere.Jump();
            OnPlayerBounced?.Invoke();
        }

        private void PlatformEventBus_OnPlatformPassed() {
            passCounter++;
            OnPlayerPassedPlatform?.Invoke();
        }
    }
}
