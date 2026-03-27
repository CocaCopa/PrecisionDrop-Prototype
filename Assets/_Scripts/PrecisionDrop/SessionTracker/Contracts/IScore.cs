using System;

namespace PrecisionDrop.SessionTracker.Contracts {
    public interface IScore {
        public int CurrentScore { get; }
        public event Action<ScoreInfo> OnScoreChanged;
        public event Action<ScorePopupInfo> OnScorePopupAvailable;
    }
}