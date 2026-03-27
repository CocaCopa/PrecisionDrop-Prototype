using System;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.Platforms.Contracts;
using PrecisionDrop.Player.Contracts;

namespace PrecisionDrop.GameFlow.Runtime {
    internal sealed class GameFlowCoordinator : IGameFlow {
        private readonly PlayerAccess playerAccess;
        private readonly float smashThreshold;
        private readonly IPlatformEventBus platformEventBus;

        public event Action OnPlayerPassedPlatform;
        public event Action<SmashInfo> OnPlayerSmashedPlatform;
        public event Action OnPlayerBounced;
        public event Action OnPlayerHitDanger;

        private int passCounter;
        private bool gameOver;

        internal GameFlowCoordinator(PlayerAccess playerAccess, float smashThreshold, IPlatformEventBus platformEventBus) {
            this.playerAccess = playerAccess;
            this.smashThreshold = smashThreshold;
            this.platformEventBus = platformEventBus;
        }

        internal void Init() {
            gameOver = false;
            platformEventBus.OnPlatformCollision += PlatformEventBus_OnPlatformCollision;
            platformEventBus.OnPlatformPassed += PlatformEventBus_OnPlatformPassed;
        }

        private void PlatformEventBus_OnPlatformCollision(IPlatform platform, CollisionData colData) {
            if (gameOver) { return; }

            if (playerAccess.StateRead.CanSmash) { SmashCollision(platform, colData); }
            else { VariantBasedCollision(colData.PieceVariant); }

            passCounter = 0;
        }

        private void SmashCollision(IPlatform platform, CollisionData colData) {
            ForceBreakPlatform(platform);
            NormalCollision();
            OnPlayerSmashedPlatform?.Invoke(new SmashInfo(VariantToSmashType(colData.PieceVariant), colData.ContactPoint));
        }

        private void ForceBreakPlatform(IPlatform platform) {
            platform.Break();
            playerAccess.StateWrite.SetSmashState(false);
        }

        private void VariantBasedCollision(PieceVariant pieceVariant) {
            switch (pieceVariant) {
                case PieceVariant.Normal: NormalCollision(); break;
                case PieceVariant.Danger: DangerCollision(); break;
            }
        }

        private void NormalCollision() {
            playerAccess.Sphere.Jump();
            OnPlayerBounced?.Invoke();
        }

        private void DangerCollision() {
            gameOver = true;
            playerAccess.Sphere.Lose();
            OnPlayerHitDanger?.Invoke();
        }

        private void PlatformEventBus_OnPlatformPassed(IPlatform platform) {
            if (gameOver) { return; }

            passCounter++;
            if (passCounter >= smashThreshold && !playerAccess.StateRead.CanSmash) { playerAccess.StateWrite.SetSmashState(true); }
            platform.Break();
            OnPlayerPassedPlatform?.Invoke();
        }

        private static SmashType VariantToSmashType(PieceVariant variant) {
            return variant switch {
                PieceVariant.Normal => SmashType.Normal,
                PieceVariant.Danger => SmashType.Danger,
                _ => throw new NotImplementedException($"[{nameof(GameFlowCoordinator)}] Variant '{variant.ToString()}' does not match with a '{nameof(SmashType)}'")
            };
        }
    }
}