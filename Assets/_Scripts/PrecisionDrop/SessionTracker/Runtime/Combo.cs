using System;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.SessionTracker.Contracts;

namespace PrecisionDrop.SessionTracker.Unity {
    internal sealed class Combo : ICombo {
        private readonly IGameFlow gameFlow;

        public event Action<int> OnComboCounterUpdated;
        public event Action OnComboCounterReset;
        private int comboCounter;

        internal Combo(IGameFlow gameFlow) {
            this.gameFlow = gameFlow;
        }

        internal void Init() {
            comboCounter = 0;
            gameFlow.OnPlayerPassedPlatform += GameFlow_OnPlayerPassedPlatform;
            gameFlow.OnPlayerBounced += GameFlow_OnPlayerBounced;
        }

        private void GameFlow_OnPlayerBounced() {
            comboCounter = 0;
            OnComboCounterReset?.Invoke();
        }

        private void GameFlow_OnPlayerPassedPlatform() {
            comboCounter++;
            OnComboCounterUpdated?.Invoke(comboCounter);
        }
    }
}