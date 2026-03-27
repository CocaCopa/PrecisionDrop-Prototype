using System;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.SessionTracker.Contracts;

namespace PrecisionDrop.SessionTracker.Unity {
    internal sealed class Score : IScore {
        private readonly IGameFlow gameFlow;
        public int CurrentScore { get; private set; }
        public event Action<ScoreInfo> OnScoreChanged;
        public event Action<ScorePopupInfo> OnScorePopupAvailable;

        private ScoreInfo currentInfo;

        internal Score(IGameFlow gameFlow) {
            this.gameFlow = gameFlow;
        }

        internal void Init() {
            gameFlow.OnPlayerPassedPlatform += GameFlow_OnPlayerPassedPlatform;
            gameFlow.OnPlayerSmashedPlatform += GameFlow_OnPlayerSmashedPlatform;
            currentInfo = new ScoreInfo(ScoreType.Normal, 0, 0);
            CurrentScore = 0;
        }

        private void GameFlow_OnPlayerPassedPlatform() {
            const int scoreGain = 20;
            CurrentScore += scoreGain;
            currentInfo = new ScoreInfo(ScoreType.Normal, scoreGain, currentInfo.Total + scoreGain);
            OnScoreChanged?.Invoke(currentInfo);
        }

        private void GameFlow_OnPlayerSmashedPlatform(SmashInfo info) {
            int scoreGain = info.SmashType switch {
                SmashType.Normal => 10,
                SmashType.Danger => 20,
                _ => throw new NotImplementedException($"[{nameof(Score)}] Could not read smash type")
            };

            CurrentScore += scoreGain;
            currentInfo = new ScoreInfo(ScoreType.Smash, scoreGain, currentInfo.Total + scoreGain);
            OnScoreChanged?.Invoke(currentInfo);
            var popupInfo = new ScorePopupInfo(currentInfo, info.CollisionPoint);
            OnScorePopupAvailable?.Invoke(popupInfo);
        }
    }
}