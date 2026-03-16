using System;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.Platforms.Contracts;
using PrecisionDrop.Player.Contracts;

namespace PrecisionDrop.GameFlow.Runtime {
    internal sealed class GameFlowCoordinator : IGameFlow {
        private readonly PlayerAccess playerAccess;
        private readonly IPlatformEventBus platformEventBus;

        public event Action OnPlayerPassedPlatform;
        public event Action OnPlayerBounced;
        public event Action OnPlayerHitDanger;

        private int passCounter;
        private bool gameOver;

        internal GameFlowCoordinator(PlayerAccess playerAccess, IPlatformEventBus platformEventBus) {
            this.playerAccess = playerAccess;
            this.platformEventBus = platformEventBus;
        }

        internal void Init() {
            gameOver = false;
            platformEventBus.OnPlatformCollision += PlatformEventBus_OnPlatformCollision;
            platformEventBus.OnPlatformPassed += PlatformEventBus_OnPlatformPassed;
        }

        private void PlatformEventBus_OnPlatformCollision(IPlatform platform, PieceVariant pieceVariant) {
            if (gameOver) { return; }
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (pieceVariant) {
                case PieceVariant.Normal: NormalCollision(platform); break;
                case PieceVariant.Danger: DangerCollision(); break;
            }
        }

        private void NormalCollision(IPlatform platform) {
            playerAccess.Sphere.Jump();
            OnPlayerBounced?.Invoke();
            if (passCounter > 2) {
                passCounter = 0;
                platform.Break();
            }
            passCounter = 0;
        }

        private void DangerCollision() {
            gameOver = true;
            playerAccess.Sphere.Lose();
            OnPlayerHitDanger?.Invoke();
        }

        private void PlatformEventBus_OnPlatformPassed(IPlatform platform) {
            playerAccess.StateWrite.SetSmashState(true);
            platform.Break();
            passCounter++;
            OnPlayerPassedPlatform?.Invoke();
        }
    }
}