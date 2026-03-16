using System;

namespace PrecisionDrop.SessionTracker.Contracts {
    public interface IScore {
        public int CurrentScore { get; }
        public event Action OnScoreChanged;
    }
}