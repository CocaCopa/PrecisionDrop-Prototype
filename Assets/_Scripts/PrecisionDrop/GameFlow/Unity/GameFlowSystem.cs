using System;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.GameFlow.Runtime;
using PrecisionDrop.Platforms.Contracts;
using PrecisionDrop.Player.Contracts;
using UnityEngine;

namespace PrecisionDrop.GameFlow.Unity {
    public sealed class GameFlowSystem : MonoBehaviour {
        private GameFlowCoordinator coordinator;
        private bool installed;
        private bool initialized;

        public IGameFlow Api => installed ? coordinator : throw new InvalidOperationException($"[{nameof(GameFlowSystem)}] API accessed before {nameof(Install)}().");
        
        public void Install(IPlayerSphere playerSphere, IPlatformEventBus eventBus) {
            if (installed) { throw new InvalidOperationException($"[{nameof(GameFlowSystem)}] {nameof(Install)}() called twice."); }
            if (eventBus is null) { throw new ArgumentNullException(nameof(eventBus)); }

            coordinator = new GameFlowCoordinator(playerSphere, eventBus);
            installed = true;
        }

        public void Init() {
            if (!installed) { throw new InvalidOperationException($"[{nameof(GameFlowSystem)}] {nameof(Init)}() called before Install()."); }
            if (initialized) { Debug.LogWarning($"[{nameof(GameFlowSystem)}] Already initialized."); return; }

            initialized = true;
            coordinator.Init();
        }
    }
}
