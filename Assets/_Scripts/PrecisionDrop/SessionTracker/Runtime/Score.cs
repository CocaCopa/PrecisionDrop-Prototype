using System;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.SessionTracker.Contracts;

namespace PrecisionDrop.SessionTracker.Unity {
    internal sealed class Score : IScore {
        private readonly IGameFlow gameFlow;
        public int CurrentScore { get; private set; }
        public event Action OnScoreChanged;

        internal Score(IGameFlow gameFlow) {
            this.gameFlow = gameFlow;
        }

        internal void Init() {
            gameFlow.OnPlayerPassedPlatform += GameFlow_OnPlayerPassedPlatform;
            CurrentScore = 0;
        }

        private void GameFlow_OnPlayerPassedPlatform() {
            CurrentScore += 20;
            OnScoreChanged?.Invoke();
        }
    }
}